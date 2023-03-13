using Microsoft.EntityFrameworkCore;
using Smoehring.Home.Data.SqlDatabase.Models;

namespace Smoehring.Home.Data.SqlDatabase
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Media> Mediae { get; set; }
        public DbSet<MediaGroup> MediaGroups { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<AssetState> AssetStates { get; set; }
        public DbSet<MediaName> MediaNames { get; set; }

        #region Overrides of DbContext

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>(builder =>
            {
                builder.HasOne<Device>(asset => asset.Device).WithOne(device => device.Asset)
                    .OnDelete(DeleteBehavior.Restrict).HasForeignKey<Asset>(asset => asset.DeviceId);
                builder.HasOne<Purchase>(asset => asset.Purchase).WithOne(purchase => purchase.Asset)
                    .OnDelete(DeleteBehavior.Restrict).HasForeignKey<Asset>(asset => asset.PurchaseId);
                builder.HasOne<Media>(asset => asset.Media).WithOne(media => media.Asset)
                    .OnDelete(DeleteBehavior.Restrict).HasForeignKey<Asset>(asset => asset.MediaId);
                builder.HasIndex(asset => asset.Name);
                builder.Property(asset => asset.Name).IsRequired();
                builder.HasAlternateKey(asset => asset.Uuid);
                builder.Property(asset => asset.Uuid).HasDefaultValueSql("(NewId())");
                builder.Property(asset => asset.Creation).HasDefaultValueSql("(GetDate())");
                builder.HasOne<AssetType>(asset => asset.AssetType).WithMany(type => type.Assets)
                    .OnDelete(DeleteBehavior.Restrict);
                builder.HasOne<Brand>(asset => asset.Brand).WithMany(brand => brand.Assets)
                    .OnDelete(DeleteBehavior.Restrict);
                builder.HasOne<AssetState>(asset => asset.AssetState).WithMany(state => state.Assets)
                    .OnDelete(DeleteBehavior.Restrict).HasForeignKey(asset => asset.AssetStateId);
                builder.Property(asset => asset.AssetStateId).HasDefaultValue(1);
                builder.HasIndex(asset => asset.Name);
            });

            modelBuilder.Entity<Media>(builder =>
            {
                builder.HasOne<MediaGroup>(media => media.Group).WithMany(group => group.Mediae).OnDelete(DeleteBehavior.Restrict);
                builder.HasMany<MediaName>(media => media.MediaNames).WithOne(name => name.Media)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MediaGroup>(builder =>
            {
                builder.HasIndex(group => group.Name).IsUnique();
            });

            modelBuilder.Entity<AssetType>(builder => builder.HasIndex(type => type.Name).IsUnique());

            modelBuilder.Entity<Language>(builder =>
            {
                builder.HasIndex(language => language.LanguageCultureName).IsUnique();
                builder.HasIndex(language => language.DisplayName).IsUnique();
                builder.Property(language => language.LanguageCultureName).HasMaxLength(8);
                builder.HasData(new Language[]
                {
                    new Language() { Id = 1, LanguageCultureName = "de-DE", DisplayName = "German - Germany" },
                    new Language() { Id = 2, LanguageCultureName = "en-US", DisplayName = "English - United States" },
                    new Language() { Id = 3, LanguageCultureName = "en-GB", DisplayName = "English - United Kingdom" },
                    new Language() { Id = 4, LanguageCultureName = "en-AU", DisplayName = "English - Australia" },
                    new Language() { Id = 5, LanguageCultureName = "ru-RU", DisplayName = "Russian - Russia" }
                });
            });

            modelBuilder.Entity<Currency>(builder =>
            {
                builder.HasIndex(currency => currency.Code).IsUnique();
                builder.HasOne<Language>(currency => currency.Language).WithMany(language => language.Currencies)
                    .HasForeignKey(currency => currency.LanguageId).OnDelete(DeleteBehavior.SetNull);
                builder.HasData(new Currency[]
                {
                    new Currency() { Id = 1, Code = "EUR", Symbol = "€", Name = "Euro", LanguageId = 1 },
                    new Currency() { Id = 2, Code = "USD", Symbol = "$", Name = "Dollars", LanguageId = 2 },
                    new Currency() { Id = 3, Code = "AUD", Symbol = "$", Name = "Dollars", LanguageId = 4 },
                    new Currency() { Id = 4, Code = "GBP", Symbol = "£", Name = "Pound", LanguageId = 3 },
                    new Currency() { Id = 5, Code = "RUB", Symbol = "₽", Name = "Rubles", LanguageId = 5 },
                });
            });

            modelBuilder.Entity<Purchase>(builder =>
            {
                builder.Property(purchase => purchase.Amount).HasPrecision(18, 2);
                builder.Property(purchase => purchase.PurchaseTime).HasDefaultValueSql("(GetDate())");
                builder.HasOne<Currency>(purchase => purchase.Currency).WithMany(currency => currency.Purchases)
                    .HasForeignKey(purchase => purchase.CurrencyId);
            });

            modelBuilder.Entity<AssetState>(builder =>
            {
                builder.HasData(new AssetState[]
                {
                    new AssetState()
                        { Id = 1, Name = "Available", IsDefault = true, Ownership = true, Possession = true },
                    new AssetState()
                        { Id = 2, Name = "Lost", IsDefault = false, Ownership = false, Possession = false },
                    new AssetState()
                        { Id = 3, Name = "Stolen", IsDefault = false, Ownership = false, Possession = false },
                    new AssetState()
                        { Id = 4, Name = "Broken", IsDefault = false, Ownership = false, Possession = false },
                    new AssetState()
                        { Id = 5, Name = "Loaned", IsDefault = false, Ownership = true, Possession = false },
                    new AssetState()
                        { Id = 6, Name = "Borrowed", IsDefault = false, Ownership = false, Possession = true },
                });
            });

            modelBuilder.Entity<MediaName>(builder =>
            {
                builder.HasOne<Language>(name => name.Language).WithMany(language => language.MediaNames)
                    .OnDelete(DeleteBehavior.Restrict).HasForeignKey(name => name.LanguageId);
                builder.HasIndex(name => name.Name);
            });
        }

        #endregion
    }
}