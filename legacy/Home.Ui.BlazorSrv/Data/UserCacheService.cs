using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Data.SqlDatabase;
using Smoehring.Home.Data.SqlDatabase.Models;

namespace Smoehring.Home.Ui.BlazorSrv.Data
{
    public class UserCacheService
    {
        private readonly ILogger<UserCacheService> _logger;
        private readonly IDbContextFactory<DatabaseContext> _dbContextFactory;
        private List<Language>? _languages;
        private List<Currency>? _currencies;
        private List<AssetState>? _assetStates;


        public List<int> SelectedAssetIds { get; set; } = new List<int>();

        public IReadOnlyList<Language> Languages
        {
            get
            {
                if (_languages is not null) return _languages;
                using var context = _dbContextFactory.CreateDbContext();
                _languages = context.Languages.ToList();
                return _languages;
            }
        }

        public IReadOnlyList<Currency> Currencies
        {
            get
            {
                if (_currencies is not null) return _currencies;
                using var context = _dbContextFactory.CreateDbContext();
                _currencies = context.Currencies.ToList();
                return _currencies;
            }
        }

        public IReadOnlyList<AssetState> AssetStates
        {
            get
            {
                if (_assetStates is not null) return _assetStates;
                using var context = _dbContextFactory.CreateDbContext();
                _assetStates = context.AssetStates.ToList();
                return _assetStates;
            }
        }

        public UserCacheService(ILogger<UserCacheService> logger, IDbContextFactory<DatabaseContext> dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }

    }
}
