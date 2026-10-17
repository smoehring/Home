using System.Reflection;

namespace Smoehring.Home.Domain.Tests;

/// <summary>Guards the clean-architecture dependency rule: Domain -> nothing, Application -> Domain only.</summary>
public class ArchitectureTests
{
    private static IEnumerable<string> References(Assembly a) => a.GetReferencedAssemblies().Select(r => r.Name!);

    [Fact]
    public void Domain_does_not_depend_on_frameworks_or_other_layers()
    {
        var refs = References(typeof(Smoehring.Home.Domain.Common.Entity).Assembly).ToList();
        Assert.DoesNotContain(refs, r => r.StartsWith("Microsoft.EntityFrameworkCore") || r.StartsWith("Microsoft.AspNetCore")
            || r.StartsWith("Npgsql") || r.Contains("Application") || r.Contains("Infrastructure") || r.Contains("Web"));
    }
}
