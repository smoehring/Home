using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Models;
using Smoehring.Home.Ui.BlazorSrv.Data;

namespace Smoehring.Home.Ui.BlazorSrv.Pages.Assets
{
    public partial class AssetOverview : ComponentBase
    {
        private List<Asset>? _assets;
        private EditContext? _editContext;
        [Inject] public IDbContextFactory<DatabaseContext> DbContextFactory { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public UserCacheService UserCache { get; set; }

        #region Overrides of ComponentBase

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _assets = await context.Assets.Include(asset => asset.Brand).Include(asset => asset.AssetType).ToListAsync();
            foreach (var asset in _assets.Where(asset => UserCache.SelectedAssetIds.Contains(asset.Id)))
            {
                asset.IsSelected = true;
            }
            _editContext = new EditContext(_assets);
            _editContext.OnFieldChanged += (sender, args) =>
            {
                UserCache.SelectedAssetIds = _assets.Where(asset => asset.IsSelected).Select(asset => asset.Id).ToList();
            };
        }

        #endregion
    }
}
