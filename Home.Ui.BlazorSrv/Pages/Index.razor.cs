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
        private bool _isDatabaseWakingUp;
        private bool _isDatabaseAvailable;
        [Inject] public IDbContextFactory<DatabaseContext> DbContextFactory { get; set; }
        [Inject] public ILogger<Index> Logger { get; set; }

        #region Overrides of ComponentBase

        /// <inheritdoc />
        protected override async Task OnInitializedAsync()
        {
            const int dbTestTimeout = 5;
            try
            {
                await using var pingContext = await DbContextFactory.CreateDbContextAsync();
                pingContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(dbTestTimeout));
                pingContext.Database.SqlQuery<int>($"SELECT {1}");
            }
            catch (Exception e)
            {
                _isDatabaseWakingUp = true;
                Logger.LogWarning(e, "Database did not respond after {Timeout} seconds. Hibernating?", dbTestTimeout);
                await InvokeAsync(StateHasChanged);
            }
            
            await using var context = await DbContextFactory.CreateDbContextAsync();
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            _values = new Dictionary<string, int>
            {
                { nameof(Asset), await context.Assets.CountAsync() },
                //{ nameof(Media), await context.Mediae.CountAsync() },
                //{ nameof(Device), await context.Devices.CountAsync() },
                //{ nameof(Purchase), await context.Purchases.CountAsync() }
            };
            _isDatabaseAvailable = true;
        }

        #endregion
    }
}
