using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Const;
using Smoehring.Home.Data.SqlDatabase.Models;
using Smoehring.Home.Ui.BlazorSrv.Const;
using Smoehring.Home.Ui.BlazorSrv.Data;
using System.Threading;

namespace Smoehring.Home.Ui.BlazorSrv.Pages.Assets
{
    public partial class AssetDetail : ComponentBase
    {
        [Inject] public IDbContextFactory<DatabaseContext> DbContextFactory { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public UserCacheService UserCache { get; set; }
        [Inject] public BlobServiceClient BlobServiceClient { get; set; }
        [Inject] public IHostEnvironment HostEnvironment { get; set; }
        [Parameter] public Guid? Uuid { get; set; }

        private Asset _currentAsset;
        private AssetDetailMode _mode;
        private DatabaseContext _context;
        private EditContext _editContext;
        private ValidationMessageStore _validationMessageStore;
        private IReadOnlyList<Brand>? _brandSuggestions;
        private IReadOnlyList<AssetType>? _assetTypeSuggestions;
        private IReadOnlyList<ArtworkCharacter>? _characterNameSuggestions;
        private IReadOnlyList<ArtworkArtist>? _artistNameSuggestions;
        private IReadOnlyList<MediaGroup>? _mediaGroups;
        private IReadOnlyList<IBrowserFile>? _tempBrowserFiles;
        private string _currentBrand = string.Empty;
        private string _currentAssetType = string.Empty;
        private string _currentMediaGroup = string.Empty;
        private MediaName _tempMediaName = new MediaName() { LanguageId = 1 };
        private string _tempArtistName = string.Empty;
        private string _tempCharacterName = string.Empty;

        private string _fileStorageBaseUrl =>
            $"{BlobServiceClient.Uri.AbsoluteUri}{_storageContainerName}-{HostEnvironment.EnvironmentName.ToLower()}";
        private bool _isWorking;

        private static string DefaultFileDragClass =
            "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";

        private static string FileDragClass = DefaultFileDragClass;
        private static string _storageContainerName = "assetfile";

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
                currentAsset = _context
                    .Assets
                    .Include(asset => asset.AssetType)
                    .Include(asset => asset.Brand)
                    .Include(asset => asset.Media)
                    .ThenInclude(group => group.Group)
                    .Include(asset => asset.Media)
                    .ThenInclude(asset => asset.MediaNames)
                    .Include(asset => asset.Device)
                    .Include(asset => asset.Purchase)
                    .Include(asset => asset.AssetState)
                    .Include(asset => asset.Artwork)
                    .ThenInclude(artwork => artwork.Characters)
                    .Include(asset => asset.Artwork)
                    .ThenInclude(artwork => artwork.Artists)
                    .ThenInclude(artist => artist.Names)
                    .Include(asset => asset.Files)
                    .AsSplitQuery()
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
            _artistNameSuggestions = _context.Artists.Include(artist => artist.Names).ToList();
            _characterNameSuggestions = _context.Characters.ToList();

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
            await ClearFiles();
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
            var currency = _context.Currencies.First(currency => currency.Id == 1);
            _currentAsset.Purchase = new Purchase() { PurchaseTime = DateTimeOffset.Now.Date, Currency = currency, CurrencyId = currency.Id};

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

        private void AddArtworkInformation_OnCLick()
        {
            _currentAsset.Artwork = new Artwork()
            {
                Stage = ArtworkStages.Backlog, Artists = new List<ArtworkArtist>(),
                Characters = new List<ArtworkCharacter>()
            };
        }

        private void RemoveArtworkInformation_OnCLick()
        {
            if (_currentAsset.Artwork is not null && _currentAsset.Artwork.Id > 0)
            {
                _context.Artworks.Remove(_currentAsset.Artwork);
            }

            _currentAsset.Artwork = null;
        }

        private void AddArtistName_OnClick()
        {
            var currentArtist = _artistNameSuggestions?.FirstOrDefault(artist => artist.Names.Any(name => name.Name.Equals(_tempArtistName)));
            if (currentArtist is null)
            {
                currentArtist = new ArtworkArtist() { Names = new List<ArtistName>() };
                currentArtist.Names.Add(new ArtistName() { Name = _tempArtistName });
            }
            _currentAsset.Artwork.Artists.Add(currentArtist);
        }

        private void RemoveArtistName_OnClick(ArtworkArtist artist)
        {
            _currentAsset.Artwork.Artists.Remove(artist);
        }

        private void AddCharacterName_OnClick()
        {
            var currentCharacter = _characterNameSuggestions?.FirstOrDefault(character => character.Name.Equals(_tempCharacterName));
            if (currentCharacter is null)
            {
                currentCharacter = new ArtworkCharacter() { Name = _tempCharacterName };
                _context.Characters.Add(currentCharacter);
            }
            _currentAsset.Artwork.Characters.Add(currentCharacter);
        }

        private void RemoveCharacter_OnClick(int characterId)
        {
            _currentAsset.Artwork.Characters.Remove(_currentAsset.Artwork.Characters.First(character => character.Id == characterId));
        }

        private void AddFileInformation_OnCLick()
        {
            _currentAsset.Files = new List<AssetFile>();
        }

        private void OnInputFilesChanged(InputFileChangeEventArgs e)
        {
            ClearDragClass();
            var files = e.GetMultipleFiles();
            _tempBrowserFiles = files;
        }

        private void SetDragClass()
        {
            FileDragClass = $"{DefaultFileDragClass} mud-border-primary"; 
        }

        private void ClearDragClass()
        {
            FileDragClass = DefaultFileDragClass;
        }

        private async Task UploadFiles()
        {
            if (_currentAsset.Id <= 0)
            {
                return;
            }
            _isWorking = true;
            await InvokeAsync(StateHasChanged);

            var containerName = $"{_storageContainerName}-{HostEnvironment.EnvironmentName.ToLower()}";
            var blobClient = BlobServiceClient.GetBlobContainerClient(containerName);
            await blobClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            foreach (var file in _tempBrowserFiles)
            {
                var assetFile = new AssetFile()
                {
                    OriginalFileName = file.Name,
                    Size = file.Size,
                    ContentType = file.ContentType,
                    StorageFileName = $"{_currentAsset.Uuid:N}x{Guid.NewGuid():N}{Path.GetExtension(file.Name)}"
                };
                var respone = await blobClient.UploadBlobAsync(assetFile.StorageFileName, file.OpenReadStream(1024*1024*1024));
                if (respone.GetRawResponse().IsError) continue;
                _currentAsset.Files.Add(assetFile);
                var currentBlob = blobClient.GetBlobClient(assetFile.StorageFileName);
                var properties = await currentBlob.GetPropertiesAsync();
                var newHeaders = new BlobHttpHeaders
                {
                    ContentType = assetFile.ContentType,
                    CacheControl = "public, max-age=31536000",
                    ContentDisposition = properties.Value.ContentDisposition,
                    ContentEncoding = properties.Value.ContentEncoding,
                    ContentLanguage = properties.Value.ContentLanguage,
                    ContentHash = properties.Value.ContentHash
                };
                await currentBlob.SetHttpHeadersAsync(newHeaders);
            }

            await _context.SaveChangesAsync();
            await ClearFiles();
            _isWorking = false;
        }

        private Task ClearFiles()
        {
            _tempBrowserFiles = new List<IBrowserFile>();
            ClearDragClass();
            return Task.CompletedTask;
        }

        private async Task<IEnumerable<string>> SearchBrandNames(string searchString, CancellationToken cancellationToken)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return await context.Brands.Select(brand => brand.Name).ToListAsync(cancellationToken);
            }
            return await context.Brands.Where(brand => brand.Name.Contains(searchString)).Select(brand => brand.Name).ToListAsync(cancellationToken);
        }

        private async Task<IEnumerable<string>> SearchAssetTypeNames(string searchString, CancellationToken cancellationToken)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return await context.AssetTypes.Select(assetType => assetType.Name).ToListAsync(cancellationToken);
            }
            return await context.AssetTypes.Where(assetType => assetType.Name.Contains(searchString)).Select(assetType => assetType.Name).ToListAsync(cancellationToken);
        }

        private async Task<IEnumerable<string>> SearchMediaGroupNames(string searchString, CancellationToken cancellationToken)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return await context.MediaGroups.Select(group => group.Name).ToListAsync(cancellationToken);
            }
            return await context.MediaGroups.Where(group => group.Name.Contains(searchString)).Select(group => group.Name).ToListAsync(cancellationToken);
        }

        private async Task<IEnumerable<string>> SearchArtistNames(string searchString, CancellationToken cancellationToken)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return await context.ArtistNames.Select(artist => artist.Name).ToListAsync(cancellationToken);
            }
            return await context.ArtistNames.Where(artist => artist.Name.Contains(searchString)).Select(artist => artist.Name).ToListAsync(cancellationToken); 

        }

        private async Task<IEnumerable<string>> SearchCharacterNames(string searchString, CancellationToken cancellationToken)
        {
            await using var context = await DbContextFactory.CreateDbContextAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return await context.Characters.Select(character => character.Name).ToListAsync(cancellationToken);
            }
            return await context.Characters.Where(character => character.Name.Contains(searchString)).Select(character => character.Name).ToListAsync(cancellationToken);
        }
    }
}
