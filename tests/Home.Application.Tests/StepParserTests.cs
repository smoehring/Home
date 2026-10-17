using Smoehring.Home.Application.Recipes;

namespace Smoehring.Home.Application.Tests;

public class StepParserTests
{
    [Fact]
    public void Parses_name_duration_kind_and_one_based_dependencies()
    {
        var steps = StepParser.Parse("Mix | 10 | active |\nRise | 60 | wait | 1\nBake | 40 | active | 1, 2");
        Assert.Equal(3, steps.Count);
        Assert.Equal(TimeSpan.FromMinutes(60), steps[1].Duration);
        Assert.False(steps[1].IsActiveWork);
        Assert.Equal([0], steps[1].DependsOnIndexes);
        Assert.Equal([0, 1], steps[2].DependsOnIndexes);
    }

    [Fact]
    public void Kind_defaults_to_active() => Assert.True(StepParser.Parse("Mix | 5")[0].IsActiveWork);

    [Fact]
    public void Blank_lines_are_ignored() => Assert.Single(StepParser.Parse("\n\nMix | 5\n\n"));

    [Theory]
    [InlineData("Mix")]
    [InlineData("Mix | abc")]
    [InlineData("Mix | -5")]
    [InlineData("Mix | 5 | active | x")]
    public void Invalid_lines_throw(string text) => Assert.Throws<FormatException>(() => StepParser.Parse(text));
}
