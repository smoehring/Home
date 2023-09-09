using Microsoft.AspNetCore.Components;

namespace Smoehring.Home.Ui.BlazorSrv
{
    public partial class MainLayout : LayoutComponentBase
    {
        private bool _drawerOpen = true;

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }
    }
}
