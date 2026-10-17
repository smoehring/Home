using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Domain.Recipes;

public enum DependencyType { FinishToStart = 0, StartToStart = 1 }

public class Recipe : AuditableEntity
{
    public Guid Uuid { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public int Servings { get; set; } = 1;
    public string? SourceUrl { get; set; }
    public List<RecipeIteration> Iterations { get; set; } = [];

    public RecipeIteration? LatestIteration => Iterations.OrderByDescending(i => i.Number).FirstOrDefault();

    /// <summary>Creates the next iteration as a copy of the latest one (or an empty first iteration).</summary>
    public RecipeIteration CreateIteration(string? changeNotes, DateTimeOffset now)
    {
        var previous = LatestIteration;
        var next = previous?.CopyAsNext(changeNotes, now) ?? new RecipeIteration { Number = 1, CreatedAt = now, ChangeNotes = changeNotes };
        Iterations.Add(next);
        return next;
    }
}

public class RecipeIteration : Entity
{
    public int RecipeId { get; set; }
    public int Number { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string? ChangeNotes { get; set; }
    public int? Rating { get; set; }
    public DateOnly? CookedOn { get; set; }
    public int? BasedOnIterationId { get; set; }
    public List<RecipeStep> Steps { get; set; } = [];
    public List<RecipeIngredient> Ingredients { get; set; } = [];

    internal RecipeIteration CopyAsNext(string? changeNotes, DateTimeOffset now)
    {
        var copy = new RecipeIteration { Number = Number + 1, CreatedAt = now, ChangeNotes = changeNotes, BasedOnIterationId = Id == 0 ? null : Id };
        var map = new Dictionary<RecipeStep, RecipeStep>();
        foreach (var s in Steps.OrderBy(s => s.SortOrder))
        {
            var c = new RecipeStep
            {
                Name = s.Name, Instructions = s.Instructions, Duration = s.Duration,
                StartOffset = s.StartOffset, IsActiveWork = s.IsActiveWork, SortOrder = s.SortOrder
            };
            map[s] = c;
            copy.Steps.Add(c);
        }
        foreach (var s in Steps)
            foreach (var d in s.Dependencies)
                if (map.TryGetValue(d.DependsOn!, out var target))
                    map[s].Dependencies.Add(new RecipeStepDependency { DependsOn = target, Type = d.Type });
        foreach (var i in Ingredients)
            copy.Ingredients.Add(new RecipeIngredient { Name = i.Name, Quantity = i.Quantity, Unit = i.Unit });
        return copy;
    }

    public IReadOnlyList<ScheduledStep> Schedule() => GanttScheduler.Schedule(Steps);
}

public class RecipeStep : Entity
{
    public int IterationId { get; set; }
    public string Name { get; set; } = "";
    public string? Instructions { get; set; }
    public TimeSpan Duration { get; set; }
    /// <summary>Pins the step start relative to the recipe start; null = derived from dependencies.</summary>
    public TimeSpan? StartOffset { get; set; }
    public bool IsActiveWork { get; set; } = true;
    public int SortOrder { get; set; }
    public List<RecipeStepDependency> Dependencies { get; set; } = [];
}

public class RecipeStepDependency : Entity
{
    public int StepId { get; set; }
    public int DependsOnStepId { get; set; }
    public RecipeStep? DependsOn { get; set; }
    public DependencyType Type { get; set; }
}

public class RecipeIngredient : Entity
{
    public int IterationId { get; set; }
    public string Name { get; set; } = "";
    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
}

public sealed record ScheduledStep(RecipeStep Step, TimeSpan Start, TimeSpan End);
