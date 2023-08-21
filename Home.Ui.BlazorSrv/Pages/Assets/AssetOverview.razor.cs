using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Models;
using Smoehring.Home.Ui.BlazorSrv.Data;

namespace Smoehring.Home.Ui.BlazorSrv.Pages.Assets
{
    public partial class AssetOverview : ComponentBase
    {
        private List<Asset>? _assets;
        private EditContext? _editContext;
        private string _searchString = string.Empty;
        private MudTable<Asset> _table;
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

        private async Task<TableData<Asset>> ServerReload(TableState tstate)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            var baseQuery = context.Assets.Include(asset => asset.Brand).Include(asset => asset.AssetType).AsQueryable();

            // Filter by search string
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                baseQuery = baseQuery.Where(asset => asset.Name.Contains(_searchString)
                || asset.AssetType.Name.Contains(_searchString)
                || asset.Brand.Name.Contains(_searchString)
                || asset.Media.MediaNames.Any(name => name.Name.Contains(_searchString)));
            }

            var totalItems = await baseQuery.CountAsync();

            baseQuery = tstate.SortLabel switch
            {
                nameof(Asset.Name) => baseQuery.OrderByDirection(tstate.SortDirection, asset => asset.Name),
                nameof(Asset.AssetType) => baseQuery.OrderByDirection(tstate.SortDirection, asset => asset.AssetType.Name),
                nameof(Asset.Brand) => baseQuery.OrderByDirection(tstate.SortDirection, asset => asset.Brand.Name),
                _ => baseQuery.OrderByDescending(asset => asset.Id)
            };

            var assets = await baseQuery.Skip(tstate.Page * tstate.PageSize).Take(tstate.PageSize).ToListAsync();

            return new TableData<Asset>() { Items = assets, TotalItems = totalItems };
        }

        private async Task OnSearch(string searchword)
        {
            _searchString = searchword;
            await _table.ReloadServerData();
        }
    }
}
