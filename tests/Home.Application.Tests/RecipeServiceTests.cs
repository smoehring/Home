using Smoehring.Home.Application.Common;
using Smoehring.Home.Application.Recipes;
using Smoehring.Home.Domain.Common;

namespace Smoehring.Home.Application.Tests;

public class RecipeServiceTests
{
    private readonly TestDb _db = new();
    private RecipeService Sut => new(_db, _db.Clock);

    private static StepInput S(string name, int min, params int[] deps) => new(name, TimeSpan.FromMinutes(min), true, null, null, deps);

    [Fact]
    public async Task Create_adds_first_iteration()
    {
        var id = await Sut.CreateAsync("Bread", 4, null);
        var item = Assert.Single(await Sut.ListAsync());
        Assert.Equal(id, item.Uuid);
        Assert.Equal(1, item.IterationCount);
    }

    [Theory]
    [InlineData("", 2)]
    [InlineData("Bread", 0)]
    public async Task Create_validates_input(string title, int servings) =>
        await Assert.ThrowsAsync<DomainException>(() => Sut.CreateAsync(title, servings, null));

    [Fact]
    public async Task Steps_produce_a_gantt_plan_with_dependencies()
    {
        var id = await Sut.CreateAsync("Bread", 4, null);
        await Sut.SetStepsAsync(id, [S("Mix", 10), S("Rise", 60, 0), S("Bake", 40, 1), S("Preheat", 20)]);
        var plan = (await Sut.PlanAsync(id, null))!;
        Assert.Equal(TimeSpan.FromMinutes(110), plan.Total);
        Assert.Equal(TimeSpan.FromMinutes(70), plan.Rows.Single(r => r.Name == "Bake").Start);
        Assert.Equal(TimeSpan.Zero, plan.Rows.Single(r => r.Name == "Preheat").Start);
    }

    [Fact]
    public async Task Replacing_steps_overwrites_the_previous_ones()
    {
        var id = await Sut.CreateAsync("Bread", 4, null);
        await Sut.SetStepsAsync(id, [S("A", 5), S("B", 5)]);
        await Sut.SetStepsAsync(id, [S("C", 5)]);
        Assert.Equal("C", Assert.Single((await Sut.PlanAsync(id, null))!.Rows).Name);
    }

    [Fact]
    public async Task Circular_dependencies_are_rejected_and_nothing_is_saved()
    {
        var id = await Sut.CreateAsync("Bread", 4, null);
        await Sut.SetStepsAsync(id, [S("Keep", 5)]);
        await Assert.ThrowsAsync<DomainException>(() => Sut.SetStepsAsync(id, [S("A", 5, 1), S("B", 5, 0)]));
        Assert.Equal("Keep", Assert.Single((await Sut.PlanAsync(id, null))!.Rows).Name);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(-1)]
    public async Task Invalid_dependency_index_is_rejected(int dep)
    {
        var id = await Sut.CreateAsync("Bread", 4, null);
        await Assert.ThrowsAsync<DomainException>(() => Sut.SetStepsAsync(id, [S("A", 5, dep)]));
    }

    [Fact]
    public async Task Self_dependency_is_rejected()
    {
        var id = await Sut.CreateAsync("Bread", 4, null);
        await Assert.ThrowsAsync<DomainException>(() => Sut.SetStepsAsync(id, [S("A", 5, 0)]));
    }

    [Fact]
    public async Task Steps_need_names_and_non_negative_durations()
    {
        var id = await Sut.CreateAsync("Bread", 4, null);
        await Assert.ThrowsAsync<DomainException>(() => Sut.SetStepsAsync(id, [S(" ", 5)]));
        await Assert.ThrowsAsync<DomainException>(() => Sut.SetStepsAsync(id, [S("A", -5)]));
    }

    [Fact]
    public async Task New_iteration_copies_steps_and_keeps_history()
    {
        var id = await Sut.CreateAsync("Bread", 4, null);
        await Sut.SetStepsAsync(id, [S("Mix", 10), S("Bake", 40, 0)]);
        Assert.Equal(2, await Sut.NewIterationAsync(id, "less salt"));
        var latest = (await Sut.PlanAsync(id, null))!;
        Assert.Equal(2, latest.Number);
        Assert.Equal(2, latest.Rows.Count);
        Assert.Equal(TimeSpan.FromMinutes(50), latest.Total);
        Assert.Equal(1, (await Sut.PlanAsync(id, 1))!.Number);
        Assert.Null(await Sut.PlanAsync(id, 9));
    }

    [Fact]
    public async Task Unknown_recipe_is_reported() =>
        await Assert.ThrowsAsync<NotFoundException>(() => Sut.PlanAsync(Guid.NewGuid(), null));
}
