using Smoehring.Home.Domain.Recipes;

namespace Smoehring.Home.Domain.Tests;

public class RecipeTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void First_iteration_is_number_one()
    {
        var recipe = new Recipe();
        Assert.Equal(1, recipe.CreateIteration("start", Now).Number);
    }

    [Fact]
    public void New_iteration_deep_copies_steps_dependencies_and_ingredients()
    {
        var recipe = new Recipe();
        var first = recipe.CreateIteration(null, Now);
        var a = new RecipeStep { Name = "a", Duration = TimeSpan.FromMinutes(5), SortOrder = 0 };
        var b = new RecipeStep { Name = "b", Duration = TimeSpan.FromMinutes(5), SortOrder = 1 };
        b.Dependencies.Add(new RecipeStepDependency { DependsOn = a });
        first.Steps.AddRange([a, b]);
        first.Ingredients.Add(new RecipeIngredient { Name = "flour", Quantity = 500, Unit = "g" });

        var second = recipe.CreateIteration("more flour", Now);

        Assert.Equal(2, second.Number);
        Assert.Equal(2, second.Steps.Count);
        Assert.DoesNotContain(second.Steps, s => first.Steps.Contains(s));
        var copiedB = second.Steps.Single(s => s.Name == "b");
        Assert.Same(second.Steps.Single(s => s.Name == "a"), copiedB.Dependencies.Single().DependsOn);
        Assert.Equal("flour", second.Ingredients.Single().Name);
        Assert.Equal(2, recipe.LatestIteration!.Number);
    }

    [Fact]
    public void Iteration_schedule_uses_its_own_steps()
    {
        var recipe = new Recipe();
        var it = recipe.CreateIteration(null, Now);
        it.Steps.Add(new RecipeStep { Name = "bake", Duration = TimeSpan.FromMinutes(40) });
        Assert.Equal(TimeSpan.FromMinutes(40), it.Schedule().Single().End);
    }
}
