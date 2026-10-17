using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Domain.Recipes;

/// <summary>
/// Computes start/end offsets for recipe steps from their durations and dependencies.
/// </summary>
public static class GanttScheduler
{
    public static IReadOnlyList<ScheduledStep> Schedule(IReadOnlyCollection<RecipeStep> steps)
    {
        var order = TopologicalOrder(steps);
        var result = new Dictionary<RecipeStep, ScheduledStep>();
        foreach (var step in order)
        {
            var start = TimeSpan.Zero;
            foreach (var dep in step.Dependencies)
            {
                var prerequisite = result[dep.DependsOn ?? throw new DomainException($"Step '{step.Name}' has an unresolved dependency.")];
                var candidate = dep.Type == DependencyType.StartToStart ? prerequisite.Start : prerequisite.End;
                if (candidate > start) start = candidate;
            }
            if (step.StartOffset is { } pinned && pinned > start) start = pinned;
            result[step] = new ScheduledStep(step, start, start + step.Duration);
        }
        return order.Select(s => result[s]).ToList();
    }

    /// <summary>Total elapsed time of the recipe.</summary>
    public static TimeSpan TotalDuration(IEnumerable<ScheduledStep> schedule) =>
        schedule.Select(s => s.End).DefaultIfEmpty(TimeSpan.Zero).Max();

    /// <summary>Start time so that the recipe is finished at <paramref name="finishAt"/>.</summary>
    public static DateTimeOffset StartTimeFor(DateTimeOffset finishAt, IEnumerable<ScheduledStep> schedule) =>
        finishAt - TotalDuration(schedule);

    /// <summary>Throws a <see cref="DomainException"/> when the dependencies contain a cycle.</summary>
    public static void EnsureAcyclic(IReadOnlyCollection<RecipeStep> steps) => TopologicalOrder(steps);

    private static List<RecipeStep> TopologicalOrder(IReadOnlyCollection<RecipeStep> steps)
    {
        var remaining = steps.OrderBy(s => s.SortOrder).ToList();
        var done = new HashSet<RecipeStep>();
        var ordered = new List<RecipeStep>();
        while (remaining.Count > 0)
        {
            var ready = remaining.Where(s => s.Dependencies.All(d => d.DependsOn is not null && done.Contains(d.DependsOn))).ToList();
            if (ready.Count == 0)
                throw new DomainException("Recipe steps contain a circular or unresolved dependency.");
            foreach (var s in ready) { ordered.Add(s); done.Add(s); remaining.Remove(s); }
        }
        return ordered;
    }
}
