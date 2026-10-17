using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Smoehring.Home.Ui.BlazorSrv
{
    public partial class MainLayout : LayoutComponentBase
    {
        private bool _drawerOpen = true;
        private MudThemeProvider _mudThemeProvider;
        private bool _isDarkmode;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isDarkmode = await _mudThemeProvider.GetSystemPreference(); 
                StateHasChanged();
            }
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}
