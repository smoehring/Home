using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Models;

namespace Smoehring.Home.Ui.BlazorSrv.Pages.Assets
{
    public partial class AssetPrint : ComponentBase
    {
        [Inject] public IDbContextFactory<DatabaseContext> DbContextFactory { get; set; }

        private readonly AssetPrintPageModel _pageModel = new AssetPrintPageModel();
        private string _output = string.Empty;
        private bool _isWorking;

        private async Task Generate(EditContext obj)
        {
            _isWorking = true;
            InvokeAsync(StateHasChanged);
            await using var context = await DbContextFactory.CreateDbContextAsync();

            var queryResult = await FetchData(context);

            var builder = new StringBuilder();

            builder.Append('"').Append("UUID").Append('"').Append(',');
            builder.Append('"').Append("Name").Append('"').Append(',');
            builder.Append('"').Append("AssetType").Append('"').Append(',');
            builder.Append('"').Append("Brand").Append('"').Append(',');
            if (_pageModel.PrintSource is PrintSource.All or PrintSource.Devices)
            {
                builder.Append('"').Append("SerialNumber").Append('"').Append(',');
                builder.Append('"').Append("ModelNumber").Append('"').Append(',');
            }

            if (_pageModel.PrintSource is PrintSource.All or PrintSource.Media)
            {
                builder.Append('"').Append("GroupName").Append('"').Append(',');
                builder.Append('"').Append("GroupOrder").Append('"').Append(',');
            }

            builder.Remove(builder.Length - 1, 1).Append(Environment.NewLine);

            foreach (var asset in queryResult)
            {
                builder.Append('"').Append(asset.Uuid.ToString("N")).Append('"').Append(',');
                builder.Append('"').Append(asset.Name).Append('"').Append(',');
                builder.Append('"').Append(asset.AssetType?.Name ?? string.Empty).Append('"').Append(',');
                builder.Append('"').Append(asset.Brand?.Name ?? string.Empty).Append('"').Append(',');

                if (_pageModel.PrintSource is PrintSource.All or PrintSource.Devices)
                {
                    builder.Append('"').Append(asset.Device?.SerialNumber ?? string.Empty).Append('"').Append(',');
                    builder.Append('"').Append(asset.Device?.ModelNumber ?? string.Empty).Append('"').Append(',');
                }

                if (_pageModel.PrintSource is PrintSource.All or PrintSource.Media)
                {
                    builder.Append('"').Append(asset.Media?.Group?.Name ?? string.Empty).Append('"').Append(',');
                    builder.Append('"').Append(asset.Media?.GroupOrder.ToString() ?? string.Empty).Append('"').Append(',');
                }

                builder.Remove(builder.Length - 1, 1).Append(Environment.NewLine);

                asset.PrintDate = DateTimeOffset.Now;
            }

            _output = builder.ToString();
            await context.SaveChangesAsync();
            _isWorking = false;
        }

        private async Task<IList<Asset>> FetchData(DatabaseContext context)
        {
            IQueryable<Asset> query;
            var basequery = context.Assets.Include(asset => asset.Brand).Include(asset => asset.AssetType);

            switch (_pageModel.PrintSource)
            {
                case PrintSource.All:
                    query = basequery.Include(asset => asset.Device)
                        .Include(asset => asset.Media)
                        .ThenInclude(media => media.Group);
                    break;
                case PrintSource.Devices:
                    query = basequery.Include(asset => asset.Device);
                    query = query.Where(asset => asset.Device != null);
                    break;
                case PrintSource.Media:
                    query = basequery.Include(asset => asset.Media).ThenInclude(media => media.Group);
                    query = query.Where(asset => asset.Media != null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_pageModel.PrintOnlyNew)
            {
                query = query.Where(asset => asset.PrintDate == null);
            }

            return await query.ToListAsync();
        }

        private class AssetPrintPageModel
        {
            public bool PrintOnlyNew { get; set; }

            public PrintSource PrintSource { get; set; }
        }

        private enum PrintSource
        {
            All,
            Devices,
            Media,
        }
    }
}
