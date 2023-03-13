using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Models;
using Smoehring.Home.Ui.BlazorSrv.Const;
using Smoehring.Home.Ui.BlazorSrv.Data;

namespace Smoehring.Home.Ui.BlazorSrv.Pages.Assets
{
    public partial class AssetDetail : ComponentBase
    {
        [Inject] public IDbContextFactory<DatabaseContext> DbContextFactory { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public UserCacheService UserCache { get; set; }
        [Parameter] public Guid? Uuid { get; set; }

        private Asset _currentAsset;
        private AssetDetailMode _mode;
        private DatabaseContext _context;
        private EditContext _editContext;
        private ValidationMessageStore _validationMessageStore;
        private IReadOnlyList<Brand>? _brandSuggestions;
        private IReadOnlyList<AssetType>? _assetTypeSuggestions;
        private IReadOnlyList<MediaGroup>? _mediaGroups;
        private string _currentBrand = string.Empty;
        private string _currentAssetType = string.Empty;
        private string _currentMediaGroup = string.Empty;
        private MediaName _tempMediaName = new MediaName() { LanguageId = 1 };
        private bool _isWorking;

        #region Overrides of ComponentBase

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            _context = DbContextFactory.CreateDbContext();
            
        }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            Asset? currentAsset;

            if (Uuid is null)
            {
                currentAsset = CreateNewAsset();
                _mode = AssetDetailMode.Create;
            }
            else
            {
                currentAsset = _context.Assets
                    .Include(asset => asset.AssetType)
                    .Include(asset => asset.Brand)
                    .Include(asset => asset.Media)
                    .ThenInclude(group => group.Group)
                    .Include(asset => asset.Media)
                    .ThenInclude(asset => asset.MediaNames)
                    .Include(asset => asset.Device)
                    .Include(asset => asset.Purchase)
                    .Include(asset => asset.AssetState)
                    .FirstOrDefault(asset => asset.Uuid.Equals(Uuid));

                _mode = AssetDetailMode.Edit;
            }

            if (currentAsset is null)
            {
                currentAsset = CreateNewAsset();
                _mode = AssetDetailMode.Create;
            }

            _currentAsset = currentAsset;
            if (_currentAsset.Brand is not null)
            {
                _currentBrand = _currentAsset.Brand.Name;
            }

            if (_currentAsset.AssetType is not null)
            {
                _currentAssetType = _currentAsset.AssetType.Name;
            }

            if (_currentAsset.Media?.Group is not null)
            {
                _currentMediaGroup = _currentAsset.Media.Group.Name;
            }

            _brandSuggestions = _context.Brands.ToList();
            _assetTypeSuggestions = _context.AssetTypes.ToList();

            _editContext = new EditContext(_currentAsset);
            _validationMessageStore = new ValidationMessageStore(_editContext);
        }

        #endregion

        protected Asset CreateNewAsset()
        {
            return new Asset()
            {
                Creation = DateTimeOffset.Now,
                AssetStateId = UserCache.AssetStates.First(state => state.IsDefault).Id
            };
        }

        private async Task EditForm_OnSubmit(EditContext obj)
        {
            _isWorking = true;
            await InvokeAsync(StateHasChanged);

            if (!string.IsNullOrWhiteSpace(_currentBrand))
            {
                var currentBrand =
                    await _context.Brands.FirstOrDefaultAsync(brand => brand.Name.Equals(_currentBrand)) ??
                    new Brand() { Name = _currentBrand };

                _currentAsset.Brand = currentBrand;
            }
            else
            {
                _currentAsset.Brand = null;
            }

            if (!string.IsNullOrWhiteSpace(_currentAssetType))
            {
                var currentAssetType =
                    await _context.AssetTypes.FirstOrDefaultAsync(type => type.Name.Equals(_currentAssetType)) ??
                    new AssetType() { Name = _currentAssetType };

                _currentAsset.AssetType = currentAssetType;
            }
            else
            {
                _currentAsset.AssetType = null;
            }

            if (_currentAsset.Media is not null && !string.IsNullOrWhiteSpace(_currentMediaGroup))
            {
                var currentMediaGroup =
                    await _context.MediaGroups.FirstOrDefaultAsync(group => group.Name.Equals(_currentMediaGroup)) ??
                    new MediaGroup() { Name = _currentMediaGroup };

                _currentAsset.Media.Group = currentMediaGroup;
            }

            if (_mode == AssetDetailMode.Create)
            {
                _context.Assets.Add(_currentAsset);
            }

            await _context.SaveChangesAsync();
            _isWorking = false;
            _mode = AssetDetailMode.Edit;
            NavigationManager.NavigateTo($"/Asset/{_currentAsset.Uuid:N}");
        }
        private void AddDeviceInformation_OnCLick()
        {
            _currentAsset.Device = new Device();
        }

        private void RemoveDeviceInformation_OnClick()
        {
            if (_currentAsset.Device is not null && _currentAsset.Device.Id > 0)
            {
                _context.Devices.Remove(_currentAsset.Device);
            }
            _currentAsset.Device = null;
        }

        private void AddPurchaseInformation_OnCLick()
        {
            _currentAsset.Purchase = new Purchase() { PurchaseTime = DateTimeOffset.Now.Date, CurrencyId = UserCache.Currencies.First().Id};

        }

        private void RemovePurchaseInformation_OnCLick()
        {
            if (_currentAsset.Purchase is not null && _currentAsset.Purchase.Id > 0)
            {
                _context.Purchases.Remove(_currentAsset.Purchase);
            }
            _currentAsset.Purchase = null;
        }

        
        private void AddMediaInformation_OnCLick()
        {
            _currentAsset.Media = new Media();

        }

        private void RemoveMediaInformation_OnCLick()
        {
            if (_currentAsset.Media is not null && _currentAsset.Media.Id > 0)
            {
                _context.Mediae.Remove(_currentAsset.Media);
            }
            _currentAsset.Media = null;
            _currentMediaGroup = string.Empty;
        }

        private void CreateNew()
        {
            NavigationManager.NavigateTo("/Asset/New");
        }

        private void RemoveMediaName_OnClick(MediaName mediaName)
        {
            _currentAsset.Media?.MediaNames?.Remove(mediaName);
            _context.MediaNames.Remove(mediaName);
        }

        private void AddMediaName_OnClick()
        {
            if(string.IsNullOrWhiteSpace(_tempMediaName.Name)) return;
            _currentAsset.Media.MediaNames ??= new List<MediaName>();
            _currentAsset.Media.MediaNames.Add(_tempMediaName);
            _tempMediaName = new MediaName() { LanguageId = 1 };
        }
    }
}
