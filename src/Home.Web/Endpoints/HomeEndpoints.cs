using Smoehring.Home.Application.Assets;

namespace Smoehring.Home.Web.Endpoints;

public static class HomeEndpoints
{
    public static IEndpointRouteBuilder MapHomeEndpoints(this IEndpointRouteBuilder app)
    {
        // Files are private: served only to authenticated users via the fallback policy.
        app.MapGet("/files/{uuid:guid}", async (Guid uuid, AssetService assets, HttpContext http, CancellationToken ct) =>
        {
            var result = await assets.OpenFileAsync(uuid, ct);
            if (result is not { } r) return Results.NotFound();
            http.Response.Headers.XContentTypeOptions = "nosniff";
            http.Response.Headers.CacheControl = "private, max-age=3600";
            var contentType = string.IsNullOrWhiteSpace(r.File.ContentType) ? "application/octet-stream" : r.File.ContentType;
            // Everything is downloaded as attachment, so uploaded HTML/SVG is never rendered in our origin.
            return Results.File(r.Content, contentType, r.File.OriginalFileName, enableRangeProcessing: true);
        });

        app.MapGet("/assets/report.csv", async (AssetService assets, CancellationToken ct) =>
        {
            var csv = AssetService.ToCsv(await assets.InsuranceReportAsync(ct));
            return Results.File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "insurance-report.csv");
        });
        return app;
    }
}
