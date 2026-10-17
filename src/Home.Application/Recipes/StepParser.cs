
namespace Smoehring.Home.Application.Recipes;

/// <summary>Parses the simple text format "name | minutes | active|wait | 1,2" (1-based line references).</summary>
public static class StepParser
{
    public static IReadOnlyList<StepInput> Parse(string text)
    {
        var result = new List<StepInput>();
        foreach (var raw in text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = raw.Split('|', StringSplitOptions.TrimEntries);
            if (parts.Length < 2 || !int.TryParse(parts[1], out var minutes) || minutes < 0)
                throw new FormatException($"Invalid step line: '{raw}'.");
            var active = parts.Length < 3 || !parts[2].Equals("wait", StringComparison.OrdinalIgnoreCase);
            var deps = parts.Length > 3 && parts[3].Length > 0
                ? parts[3].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => int.TryParse(d, out var n) ? n - 1 : throw new FormatException($"Invalid dependency in '{raw}'.")).ToList()
                : [];
            result.Add(new StepInput(parts[0], TimeSpan.FromMinutes(minutes), active, null, null, deps));
        }
        return result;
    }
}
