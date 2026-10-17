using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smoehring.Home.Domain.Artworks;
using Smoehring.Home.Domain.Assets;
using Smoehring.Home.Domain.Common;
using Smoehring.Home.Domain.MediaLibrary;
using Smoehring.Home.Domain.Recipes;

namespace Smoehring.Home.Infrastructure.Persistence;

internal static class ConfigExtensions
{
    /// <summary>PostgreSQL optimistic concurrency via the xmin system column.</summary>
    public static void UseXmin<T>(this EntityTypeBuilder<T> b) where T : class =>
        b.Property<uint>("xmin").HasColumnType("xid").ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();
}

internal sealed class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> b)
    {
        b.UseXmin();
        b.Property(a => a.Name).HasMaxLength(300).IsRequired();
        b.Property(a => a.Uuid).HasDefaultValueSql("gen_random_uuid()");
        b.HasIndex(a => a.Uuid).IsUnique();
        b.HasIndex(a => a.Name);
        b.HasOne(a => a.Brand).WithMany().HasForeignKey(a => a.BrandId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.AssetType).WithMany().HasForeignKey(a => a.AssetTypeId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.AssetState).WithMany().HasForeignKey(a => a.AssetStateId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(a => a.Device).WithOne().HasForeignKey<Device>(d => d.AssetId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(a => a.Purchase).WithOne().HasForeignKey<Purchase>(p => p.AssetId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(a => a.Media).WithOne().HasForeignKey<Media>(m => m.AssetId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.Valuations).WithOne().HasForeignKey(v => v.AssetId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.StateChanges).WithOne().HasForeignKey(v => v.AssetId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.Files).WithOne().HasForeignKey(f => f.AssetId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class DeviceConfig : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> b)
    {
        b.HasIndex(d => d.SerialNumber);
        b.Property(d => d.SerialNumber).HasMaxLength(200);
        b.Property(d => d.ModelNumber).HasMaxLength(200);
    }
}

internal sealed class PurchaseConfig : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> b)
    {
        b.Property(p => p.Amount).HasPrecision(18, 2);
        b.HasOne(p => p.Currency).WithMany().HasForeignKey(p => p.CurrencyId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ValuationConfig : IEntityTypeConfiguration<AssetValuation>
{
    public void Configure(EntityTypeBuilder<AssetValuation> b)
    {
        b.Property(p => p.Amount).HasPrecision(18, 2);
        b.Property(p => p.Reason).HasConversion<string>().HasMaxLength(32);
        b.HasOne(p => p.Currency).WithMany().HasForeignKey(p => p.CurrencyId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class AssetFileConfig : IEntityTypeConfiguration<AssetFile>
{
    public void Configure(EntityTypeBuilder<AssetFile> b)
    {
        b.Property(f => f.Uuid).HasDefaultValueSql("gen_random_uuid()");
        b.HasIndex(f => f.Uuid).IsUnique();
        b.Property(f => f.Kind).HasConversion<string>().HasMaxLength(32);
        b.Property(f => f.OriginalFileName).HasMaxLength(500);
        b.Property(f => f.StorageKey).HasMaxLength(300);
        b.Property(f => f.Sha256).HasMaxLength(64);
    }
}

internal sealed class AssetTypeConfig : IEntityTypeConfiguration<AssetType>
{
    public void Configure(EntityTypeBuilder<AssetType> b) { b.HasIndex(x => x.Name).IsUnique(); b.Property(x => x.Name).HasMaxLength(100); }
}

internal sealed class BrandConfig : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> b) { b.HasIndex(x => x.Name).IsUnique(); b.Property(x => x.Name).HasMaxLength(100); }
}

internal sealed class AssetStateConfig : IEntityTypeConfiguration<AssetState>
{
    public void Configure(EntityTypeBuilder<AssetState> b)
    {
        b.Property(x => x.Name).HasMaxLength(50);
        b.HasData(
            new AssetState { Id = 1, Name = "Available", IsDefault = true, Ownership = true, Possession = true },
            new AssetState { Id = 2, Name = "Lost" },
            new AssetState { Id = 3, Name = "Stolen" },
            new AssetState { Id = 4, Name = "Broken" },
            new AssetState { Id = 5, Name = "Loaned", Ownership = true },
            new AssetState { Id = 6, Name = "Borrowed", Possession = true });
    }
}

internal sealed class CurrencyConfig : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> b)
    {
        b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.Code).HasMaxLength(3);
        b.HasData(
            new Currency { Id = 1, Code = "EUR", Symbol = "€", Name = "Euro" },
            new Currency { Id = 2, Code = "USD", Symbol = "$", Name = "US Dollar" },
            new Currency { Id = 3, Code = "AUD", Symbol = "$", Name = "Australian Dollar" },
            new Currency { Id = 4, Code = "GBP", Symbol = "£", Name = "Pound Sterling" },
            new Currency { Id = 5, Code = "RUB", Symbol = "₽", Name = "Russian Ruble" });
    }
}

internal sealed class LanguageConfig : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> b)
    {
        b.HasIndex(x => x.CultureName).IsUnique();
        b.Property(x => x.CultureName).HasMaxLength(8);
        b.HasData(
            new Language { Id = 1, CultureName = "de-DE", DisplayName = "German - Germany" },
            new Language { Id = 2, CultureName = "en-US", DisplayName = "English - United States" },
            new Language { Id = 3, CultureName = "en-GB", DisplayName = "English - United Kingdom" },
            new Language { Id = 4, CultureName = "en-AU", DisplayName = "English - Australia" },
            new Language { Id = 5, CultureName = "ru-RU", DisplayName = "Russian - Russia" });
    }
}

internal sealed class MediaConfig : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> b)
    {
        b.Property(m => m.Kind).HasConversion<string>().HasMaxLength(32);
        b.HasOne(m => m.Group).WithMany().HasForeignKey(m => m.GroupId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(m => m.Names).WithOne().HasForeignKey(n => n.MediaId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(m => m.ExternalIds).WithOne().HasForeignKey(n => n.MediaId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class MediaGroupConfig : IEntityTypeConfiguration<MediaGroup>
{
    public void Configure(EntityTypeBuilder<MediaGroup> b) { b.HasIndex(x => x.Name).IsUnique(); }
}

internal sealed class MediaNameConfig : IEntityTypeConfiguration<MediaName>
{
    public void Configure(EntityTypeBuilder<MediaName> b)
    {
        b.HasIndex(x => x.Name);
        b.HasOne(x => x.Language).WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class MediaExternalIdConfig : IEntityTypeConfiguration<MediaExternalId>
{
    public void Configure(EntityTypeBuilder<MediaExternalId> b) { b.HasIndex(x => new { x.System, x.Value }); }
}

internal sealed class ArtworkConfig : IEntityTypeConfiguration<Artwork>
{
    public void Configure(EntityTypeBuilder<Artwork> b)
    {
        b.UseXmin();
        b.Property(a => a.Uuid).HasDefaultValueSql("gen_random_uuid()");
        b.HasIndex(a => a.Uuid).IsUnique();
        b.Property(a => a.Title).HasMaxLength(300);
        b.Property(a => a.Stage).HasConversion<string>().HasMaxLength(32);
        b.HasOne(a => a.Asset).WithOne().HasForeignKey<Artwork>(a => a.AssetId).OnDelete(DeleteBehavior.Restrict);
        b.HasMany(a => a.Credits).WithOne().HasForeignKey(c => c.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.Characters).WithOne().HasForeignKey(c => c.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.Payments).WithOne().HasForeignKey(c => c.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.Versions).WithOne().HasForeignKey(c => c.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.StageHistory).WithOne().HasForeignKey(c => c.ArtworkId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.SetItems).WithOne(i => i.Artwork).HasForeignKey(c => c.ArtworkId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ArtistConfig : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> b)
    {
        b.Ignore(a => a.DisplayName);
        b.HasMany(a => a.Names).WithOne().HasForeignKey(n => n.ArtistId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(a => a.Profiles).WithOne().HasForeignKey(n => n.ArtistId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ArtistNameConfig : IEntityTypeConfiguration<ArtistName>
{
    public void Configure(EntityTypeBuilder<ArtistName> b)
    {
        b.Property(n => n.Name).HasMaxLength(200);
        b.HasIndex(n => n.Name);
        b.HasIndex(n => n.ArtistId).IsUnique().HasFilter("is_primary");
    }
}

internal sealed class ArtworkArtistConfig : IEntityTypeConfiguration<ArtworkArtist>
{
    public void Configure(EntityTypeBuilder<ArtworkArtist> b)
    {
        b.HasKey(x => new { x.ArtworkId, x.ArtistId, x.Role });
        b.Property(x => x.Role).HasConversion<string>().HasMaxLength(32);
        b.HasOne(x => x.Artist).WithMany().HasForeignKey(x => x.ArtistId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class CharacterConfig : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> b) { b.HasIndex(x => x.Name); b.Property(x => x.Name).HasMaxLength(200); }
}

internal sealed class ArtworkCharacterConfig : IEntityTypeConfiguration<ArtworkCharacter>
{
    public void Configure(EntityTypeBuilder<ArtworkCharacter> b)
    {
        b.HasKey(x => new { x.ArtworkId, x.CharacterId });
        b.HasOne(x => x.Character).WithMany().HasForeignKey(x => x.CharacterId).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class ArtworkPaymentConfig : IEntityTypeConfiguration<ArtworkPayment>
{
    public void Configure(EntityTypeBuilder<ArtworkPayment> b)
    {
        b.Property(p => p.Amount).HasPrecision(18, 2);
        b.Property(p => p.Kind).HasConversion<string>().HasMaxLength(32);
        b.Property(p => p.Method).HasConversion<string>().HasMaxLength(32);
        b.HasOne(p => p.Currency).WithMany().HasForeignKey(p => p.CurrencyId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<Artist>().WithMany().HasForeignKey(p => p.ArtistId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<AssetFile>().WithMany().HasForeignKey(p => p.InvoiceFileId).OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class ArtworkVersionConfig : IEntityTypeConfiguration<ArtworkVersion>
{
    public void Configure(EntityTypeBuilder<ArtworkVersion> b)
    {
        b.Property(v => v.Label).HasMaxLength(200);
        b.HasIndex(v => v.ArtworkId).IsUnique().HasFilter("is_primary");
        b.HasOne(v => v.ParentVersion).WithMany().HasForeignKey(v => v.ParentVersionId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<AssetFile>().WithMany().HasForeignKey(v => v.AssetFileId).OnDelete(DeleteBehavior.SetNull);
    }
}

internal sealed class ArtworkStageChangeConfig : IEntityTypeConfiguration<ArtworkStageChange>
{
    public void Configure(EntityTypeBuilder<ArtworkStageChange> b) { b.Property(v => v.Stage).HasConversion<string>().HasMaxLength(32); }
}

internal sealed class ArtworkSetConfig : IEntityTypeConfiguration<ArtworkSet>
{
    public void Configure(EntityTypeBuilder<ArtworkSet> b)
    {
        b.Ignore(s => s.CompletionPercent);
        b.Property(s => s.Name).HasMaxLength(200);
        b.HasMany(s => s.Items).WithOne(i => i.Set).HasForeignKey(i => i.SetId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ArtworkSetItemConfig : IEntityTypeConfiguration<ArtworkSetItem>
{
    public void Configure(EntityTypeBuilder<ArtworkSetItem> b) { b.HasKey(x => new { x.SetId, x.ArtworkId }); }
}

internal sealed class RecipeConfig : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> b)
    {
        b.UseXmin();
        b.Property(r => r.Uuid).HasDefaultValueSql("gen_random_uuid()");
        b.HasIndex(r => r.Uuid).IsUnique();
        b.Property(r => r.Title).HasMaxLength(300);
        b.HasMany(r => r.Iterations).WithOne().HasForeignKey(i => i.RecipeId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class RecipeIterationConfig : IEntityTypeConfiguration<RecipeIteration>
{
    public void Configure(EntityTypeBuilder<RecipeIteration> b)
    {
        b.HasIndex(i => new { i.RecipeId, i.Number }).IsUnique();
        b.HasOne<RecipeIteration>().WithMany().HasForeignKey(i => i.BasedOnIterationId).OnDelete(DeleteBehavior.SetNull);
        b.HasMany(i => i.Steps).WithOne().HasForeignKey(s => s.IterationId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(i => i.Ingredients).WithOne().HasForeignKey(s => s.IterationId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class RecipeStepConfig : IEntityTypeConfiguration<RecipeStep>
{
    public void Configure(EntityTypeBuilder<RecipeStep> b)
    {
        b.Property(s => s.Name).HasMaxLength(300);
        b.HasMany(s => s.Dependencies).WithOne().HasForeignKey(d => d.StepId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class RecipeStepDependencyConfig : IEntityTypeConfiguration<RecipeStepDependency>
{
    public void Configure(EntityTypeBuilder<RecipeStepDependency> b)
    {
        b.Property(d => d.Type).HasConversion<string>().HasMaxLength(32);
        b.HasOne(d => d.DependsOn).WithMany().HasForeignKey(d => d.DependsOnStepId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class RecipeIngredientConfig : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> b) { b.Property(i => i.Quantity).HasPrecision(12, 3); b.Property(i => i.Name).HasMaxLength(200); }
}
