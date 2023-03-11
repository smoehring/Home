using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Models;

namespace Smoehring.Home.Ui.BlazorSrv.Pages.Assets
{
    public partial class AssetOverview : ComponentBase
    {
        private List<Asset>? _assets;
        [Inject] public IDbContextFactory<DatabaseContext> DbContextFactory { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        #region Overrides of ComponentBase

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _assets = await context.Assets.Include(asset => asset.Brand).Include(asset => asset.AssetType).ToListAsync();
        }

        #endregion
    }
}
