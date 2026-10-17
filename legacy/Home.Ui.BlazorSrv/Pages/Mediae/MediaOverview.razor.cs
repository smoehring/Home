using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Models;

namespace Smoehring.Home.Ui.BlazorSrv.Pages.Mediae
{
    public partial class MediaOverview : ComponentBase
    {
        private MudTable<Home.Data.SqlDatabase.Models.Media> _table;
        private string? _searchString;

        [Inject] public IDbContextFactory<DatabaseContext> DbContextFactory { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }

        private async Task<TableData<Home.Data.SqlDatabase.Models.Media>> ServerReload(TableState tableState)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            var baseQuery = context.Mediae
                .Include(media => media.MediaNames)
                .Include(media => media.Asset)
                .ThenInclude(asset => asset.Brand)
                .Include(media => media.Asset)
                .ThenInclude(asset => asset.AssetType)
                .AsSplitQuery()
                .AsQueryable();

            // Filter by search string
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                baseQuery = baseQuery.Where(media =>
                    media.Asset.Name.Contains(_searchString) || (media.MediaNames != null &&
                                                                 media.MediaNames.Select(name => name.Name)
                                                                     .Contains(_searchString)));
            }

            var totalItems = await baseQuery.CountAsync();

            // Sort
            if (!string.IsNullOrWhiteSpace(tableState.SortLabel))
            {
                baseQuery = tableState.SortLabel switch
                {
                    nameof(Asset) => baseQuery.OrderByDirection(tableState.SortDirection,asset => asset.Asset.Name),
                    nameof(AssetType) => baseQuery.OrderByDirection(tableState.SortDirection,asset => asset.Asset.AssetType.Name),
                    nameof(Brand) => baseQuery.OrderByDirection(tableState.SortDirection,asset => asset.Asset.Brand.Name),
                    nameof(MediaGroup) => baseQuery.OrderByDirection(tableState.SortDirection, asset => asset.Group.Name),
                    _ => baseQuery.OrderBy(media => media.Asset.AssetType.Name).ThenBy(media => media.Group.Name)
                        .ThenBy(media => media.GroupOrder).ThenBy(media => media.Asset.Name)
                };
            }
            else
            {
                baseQuery = baseQuery.OrderBy(media => media.Asset.AssetType.Name).ThenBy(media => media.Group.Name)
                    .ThenBy(media => media.GroupOrder).ThenBy(media => media.Asset.Name);
            }

            var medias = await baseQuery.Skip(tableState.Page * tableState.PageSize)
                .Take(tableState.PageSize)
                .ToListAsync();

            return new TableData<Home.Data.SqlDatabase.Models.Media>() { Items = medias, TotalItems = totalItems };
        }

        private void Table_OnRowClick(TableRowClickEventArgs<Home.Data.SqlDatabase.Models.Media> arg)
        {
            NavigationManager.NavigateTo($"/Asset/{arg.Item.Asset.Uuid:N}");
        }

        private async Task Table_OnSearch(string searchString)
        {
            _searchString = searchString;
            await _table.ReloadServerData();
        }
    }
}
