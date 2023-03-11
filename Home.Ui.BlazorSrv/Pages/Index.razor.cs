using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Models;

namespace Smoehring.Home.Ui.BlazorSrv.Pages
{
    public partial class Index : ComponentBase
    {
        private Dictionary<string, int>? _values;
        [Inject] public IDbContextFactory<DatabaseContext> DbContextFactory { get; set; }

        #region Overrides of ComponentBase

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            await using var context = await DbContextFactory.CreateDbContextAsync();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _values = new Dictionary<string, int>
            {
                { nameof(Asset), await context.Assets.CountAsync() },
                { nameof(Media), await context.Mediae.CountAsync() },
                { nameof(Device), await context.Devices.CountAsync() },
                { nameof(Purchase), await context.Purchases.CountAsync() }
            };
        }

        #endregion
    }
}
