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
            });

            modelBuilder.Entity<Media>(builder =>
            {
                builder.HasMany<Media2MediaGroup>(media => media.Groups).WithOne(group => group.Media).HasForeignKey(group => group.MediaId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MediaGroup>(builder =>
            {
                builder.HasMany<Media2MediaGroup>(group => group.Mediae).WithOne(group => group.Group).HasForeignKey(group => group.GroupId).OnDelete(DeleteBehavior.Cascade);
                builder.HasIndex(group => group.Name).IsUnique();
            });

            modelBuilder.Entity<MediaType>(builder => builder.HasIndex(type => type.Name).IsUnique());

            modelBuilder.Entity<Media2MediaGroup>(builder =>
                builder.HasKey(group => new { group.MediaId, group.GroupId }));

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
            });
        }

        #endregion
    }
}