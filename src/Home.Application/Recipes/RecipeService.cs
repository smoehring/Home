using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Application.Abstractions;
using Smoehring.Home.Application.Common;
using Smoehring.Home.Domain.Common;
using Smoehring.Home.Domain.Recipes;

namespace Smoehring.Home.Application.Recipes;

public sealed record RecipeListItem(Guid Uuid, string Title, int Servings, int IterationCount);
public sealed record StepInput(string Name, TimeSpan Duration, bool IsActiveWork, string? Instructions, TimeSpan? StartOffset, IReadOnlyList<int> DependsOnIndexes);
public sealed record GanttRow(string Name, TimeSpan Start, TimeSpan End, bool IsActiveWork);
public sealed record IterationPlan(int Number, IReadOnlyList<GanttRow> Rows, TimeSpan Total);

public class RecipeService(IApplicationDbContextFactory dbFactory, TimeProvider clock)
{
    public async Task<IReadOnlyList<RecipeListItem>> ListAsync(CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        return await db.Recipes.AsNoTracking().Where(r => r.DeletedAt == null).OrderBy(r => r.Title)
            .Select(r => new RecipeListItem(r.Uuid, r.Title, r.Servings, r.Iterations.Count)).ToListAsync(ct);
    }

    public async Task<Guid> CreateAsync(string title, int servings, string? description, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("A recipe needs a title.");
        if (servings < 1) throw new DomainException("Servings must be at least 1.");
        await using var db = dbFactory.Create();
        var recipe = new Recipe { Title = title.Trim(), Servings = servings, Description = description };
        recipe.CreateIteration("Initial version", clock.GetUtcNow());
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync(ct);
        return recipe.Uuid;
    }

    /// <summary>Starts a new iteration as a copy of the latest one.</summary>
    public async Task<int> NewIterationAsync(Guid uuid, string? changeNotes, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var recipe = await LoadFull(db, uuid, ct);
        var iteration = recipe.CreateIteration(changeNotes, clock.GetUtcNow());
        await db.SaveChangesAsync(ct);
        return iteration.Number;
    }

    /// <summary>Replaces the steps of the latest iteration. Dependencies reference other steps by list index.</summary>
    public async Task SetStepsAsync(Guid uuid, IReadOnlyList<StepInput> inputs, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var recipe = await LoadFull(db, uuid, ct);
        var iteration = recipe.LatestIteration ?? throw new DomainException("The recipe has no iteration.");

        var steps = inputs.Select((s, i) =>
        {
            if (string.IsNullOrWhiteSpace(s.Name)) throw new DomainException("Every step needs a name.");
            if (s.Duration < TimeSpan.Zero) throw new DomainException("A step duration must not be negative.");
            return new RecipeStep { Name = s.Name.Trim(), Duration = s.Duration, IsActiveWork = s.IsActiveWork, Instructions = s.Instructions, StartOffset = s.StartOffset, SortOrder = i };
        }).ToList();
        for (var i = 0; i < inputs.Count; i++)
            foreach (var dep in inputs[i].DependsOnIndexes)
            {
                if (dep < 0 || dep >= steps.Count || dep == i) throw new DomainException($"Step {i + 1} has an invalid dependency.");
                steps[i].Dependencies.Add(new RecipeStepDependency { DependsOn = steps[dep] });
            }
        GanttScheduler.EnsureAcyclic(steps);

        db.RecipeIterations.Entry(iteration).Collection(i => i.Steps).Load();
        iteration.Steps.Clear();
        iteration.Steps.AddRange(steps);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IterationPlan?> PlanAsync(Guid uuid, int? iterationNumber, CancellationToken ct = default)
    {
        await using var db = dbFactory.Create();
        var recipe = await LoadFull(db, uuid, ct);
        var iteration = iterationNumber is { } n ? recipe.Iterations.FirstOrDefault(i => i.Number == n) : recipe.LatestIteration;
        if (iteration is null) return null;
        var schedule = iteration.Schedule();
        return new IterationPlan(iteration.Number,
            schedule.Select(s => new GanttRow(s.Step.Name, s.Start, s.End, s.Step.IsActiveWork)).ToList(),
            GanttScheduler.TotalDuration(schedule));
    }

    private static async Task<Recipe> LoadFull(IApplicationDbContext db, Guid uuid, CancellationToken ct) =>
        await db.Recipes
            .Include(r => r.Iterations).ThenInclude(i => i.Steps).ThenInclude(s => s.Dependencies).ThenInclude(d => d.DependsOn)
            .Include(r => r.Iterations).ThenInclude(i => i.Ingredients)
            .FirstOrDefaultAsync(r => r.Uuid == uuid && r.DeletedAt == null, ct) ?? throw new NotFoundException("Recipe");
}
