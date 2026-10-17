using Smoehring.Home.Domain.Common;
using Smoehring.Home.Domain.Recipes;

namespace Smoehring.Home.Domain.Tests;

public class GanttSchedulerTests
{
    private static RecipeStep Step(string name, int minutes, int order, TimeSpan? offset = null) =>
        new() { Name = name, Duration = TimeSpan.FromMinutes(minutes), SortOrder = order, StartOffset = offset };

    private static void DependsOn(RecipeStep step, RecipeStep on, DependencyType type = DependencyType.FinishToStart) =>
        step.Dependencies.Add(new RecipeStepDependency { DependsOn = on, Type = type });

    [Fact]
    public void Sequential_steps_start_after_their_prerequisite()
    {
        var a = Step("a", 10, 0); var b = Step("b", 20, 1); DependsOn(b, a);
        var plan = GanttScheduler.Schedule([a, b]);
        Assert.Equal(TimeSpan.FromMinutes(10), plan.Single(s => s.Step == b).Start);
        Assert.Equal(TimeSpan.FromMinutes(30), GanttScheduler.TotalDuration(plan));
    }

    [Fact]
    public void Independent_steps_run_in_parallel()
    {
        var plan = GanttScheduler.Schedule([Step("a", 30, 0), Step("b", 45, 1)]);
        Assert.All(plan, s => Assert.Equal(TimeSpan.Zero, s.Start));
        Assert.Equal(TimeSpan.FromMinutes(45), GanttScheduler.TotalDuration(plan));
    }

    [Fact]
    public void Step_waits_for_the_longest_prerequisite()
    {
        var a = Step("a", 10, 0); var b = Step("b", 40, 1); var c = Step("c", 5, 2);
        DependsOn(c, a); DependsOn(c, b);
        Assert.Equal(TimeSpan.FromMinutes(40), GanttScheduler.Schedule([a, b, c]).Single(s => s.Step == c).Start);
    }

    [Fact]
    public void Start_to_start_dependency_begins_with_the_prerequisite()
    {
        var a = Step("a", 60, 0, TimeSpan.FromMinutes(5)); var b = Step("b", 10, 1); DependsOn(b, a, DependencyType.StartToStart);
        Assert.Equal(TimeSpan.FromMinutes(5), GanttScheduler.Schedule([a, b]).Single(s => s.Step == b).Start);
    }

    [Fact]
    public void Pinned_offset_delays_a_step()
    {
        var plan = GanttScheduler.Schedule([Step("a", 10, 0, TimeSpan.FromMinutes(90))]);
        Assert.Equal(TimeSpan.FromMinutes(90), plan[0].Start);
    }

    [Fact]
    public void Dependencies_declared_later_in_sort_order_are_still_resolved()
    {
        var a = Step("a", 10, 5); var b = Step("b", 10, 0); DependsOn(b, a);
        var plan = GanttScheduler.Schedule([b, a]);
        Assert.Equal("a", plan[0].Step.Name);
        Assert.Equal(TimeSpan.FromMinutes(10), plan[1].Start);
    }

    [Fact]
    public void Circular_dependencies_are_rejected()
    {
        var a = Step("a", 10, 0); var b = Step("b", 10, 1); DependsOn(a, b); DependsOn(b, a);
        Assert.Throws<DomainException>(() => GanttScheduler.Schedule([a, b]));
        Assert.Throws<DomainException>(() => GanttScheduler.EnsureAcyclic([a, b]));
    }

    [Fact]
    public void Empty_recipe_has_zero_duration() =>
        Assert.Equal(TimeSpan.Zero, GanttScheduler.TotalDuration(GanttScheduler.Schedule([])));

    [Fact]
    public void Start_time_is_calculated_backwards_from_the_finish_time()
    {
        var plan = GanttScheduler.Schedule([Step("a", 90, 0)]);
        var finish = new DateTimeOffset(2026, 1, 1, 19, 0, 0, TimeSpan.Zero);
        Assert.Equal(new DateTimeOffset(2026, 1, 1, 17, 30, 0, TimeSpan.Zero), GanttScheduler.StartTimeFor(finish, plan));
    }
}
