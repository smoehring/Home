﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Smoehring.Home.Data.SqlDatabase;

#nullable disable

namespace Smoehring.Home.Data.SqlDatabase.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20230909122235_RenamingCharactersTableToSingular")]
    partial class RenamingCharactersTableToSingular
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ArtworkArtworkArtist", b =>
                {
                    b.Property<int>("ArtistsId")
                        .HasColumnType("int");

                    b.Property<int>("ArtworksId")
                        .HasColumnType("int");

                    b.HasKey("ArtistsId", "ArtworksId");

                    b.HasIndex("ArtworksId");

                    b.ToTable("ArtworkArtworkArtist");
                });

            modelBuilder.Entity("ArtworkArtworkCharacter", b =>
                {
                    b.Property<int>("ArtworksId")
                        .HasColumnType("int");

                    b.Property<int>("CharactersId")
                        .HasColumnType("int");

                    b.HasKey("ArtworksId", "CharactersId");

                    b.HasIndex("CharactersId");

                    b.ToTable("ArtworkArtworkCharacter");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.ArtistName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArtworkArtistId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ArtworkArtistId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ArtistNames");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.ArtistProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArtworkArtistId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ArtworkArtistId");

                    b.ToTable("ArtistProfile");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Artwork", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsAdult")
                        .HasColumnType("bit");

                    b.Property<int>("Stage")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Artworks");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.ArtworkArtist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Artists");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.ArtworkCharacter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsOwned")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Asset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ArtworkId")
                        .HasColumnType("int");

                    b.Property<int>("AssetStateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int?>("AssetTypeId")
                        .HasColumnType("int");

                    b.Property<int?>("BrandId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("Creation")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("(GetDate())");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int?>("MediaId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset?>("PrintDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("PurchaseId")
                        .HasColumnType("int");

                    b.Property<Guid>("Uuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("(NewId())");

                    b.HasKey("Id");

                    b.HasAlternateKey("Uuid");

                    b.HasIndex("ArtworkId")
                        .IsUnique()
                        .HasFilter("[ArtworkId] IS NOT NULL");

                    b.HasIndex("AssetStateId");

                    b.HasIndex("AssetTypeId");

                    b.HasIndex("BrandId");

                    b.HasIndex("DeviceId")
                        .IsUnique()
                        .HasFilter("[DeviceId] IS NOT NULL");

                    b.HasIndex("MediaId")
                        .IsUnique()
                        .HasFilter("[MediaId] IS NOT NULL");

                    b.HasIndex("Name");

                    b.HasIndex("PurchaseId")
                        .IsUnique()
                        .HasFilter("[PurchaseId] IS NOT NULL");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.AssetFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AssetId")
                        .HasColumnType("int");

                    b.Property<string>("ContentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OriginalFileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("Size")
                        .HasColumnType("bigint");

                    b.Property<string>("StorageFileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AssetId");

                    b.ToTable("AssetFiles");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.AssetState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Ownership")
                        .HasColumnType("bit");

                    b.Property<bool>("Possession")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("AssetStates");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsDefault = true,
                            Name = "Available",
                            Ownership = true,
                            Possession = true
                        },
                        new
                        {
                            Id = 2,
                            IsDefault = false,
                            Name = "Lost",
                            Ownership = false,
                            Possession = false
                        },
                        new
                        {
                            Id = 3,
                            IsDefault = false,
                            Name = "Stolen",
                            Ownership = false,
                            Possession = false
                        },
                        new
                        {
                            Id = 4,
                            IsDefault = false,
                            Name = "Broken",
                            Ownership = false,
                            Possession = false
                        },
                        new
                        {
                            Id = 5,
                            IsDefault = false,
                            Name = "Loaned",
                            Ownership = true,
                            Possession = false
                        },
                        new
                        {
                            Id = 6,
                            IsDefault = false,
                            Name = "Borrowed",
                            Ownership = false,
                            Possession = true
                        });
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.AssetType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("AssetTypes");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Brand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("LanguageId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("LanguageId");

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Code = "EUR",
                            LanguageId = 1,
                            Name = "Euro",
                            Symbol = "€"
                        },
                        new
                        {
                            Id = 2,
                            Code = "USD",
                            LanguageId = 2,
                            Name = "Dollars",
                            Symbol = "$"
                        },
                        new
                        {
                            Id = 3,
                            Code = "AUD",
                            LanguageId = 4,
                            Name = "Dollars",
                            Symbol = "$"
                        },
                        new
                        {
                            Id = 4,
                            Code = "GBP",
                            LanguageId = 3,
                            Name = "Pound",
                            Symbol = "£"
                        },
                        new
                        {
                            Id = 5,
                            Code = "RUB",
                            LanguageId = 5,
                            Name = "Rubles",
                            Symbol = "₽"
                        });
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ModelNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LanguageCultureName")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("nvarchar(8)");

                    b.HasKey("Id");

                    b.HasIndex("DisplayName")
                        .IsUnique();

                    b.HasIndex("LanguageCultureName")
                        .IsUnique();

                    b.ToTable("Languages");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayName = "German - Germany",
                            LanguageCultureName = "de-DE"
                        },
                        new
                        {
                            Id = 2,
                            DisplayName = "English - United States",
                            LanguageCultureName = "en-US"
                        },
                        new
                        {
                            Id = 3,
                            DisplayName = "English - United Kingdom",
                            LanguageCultureName = "en-GB"
                        },
                        new
                        {
                            Id = 4,
                            DisplayName = "English - Australia",
                            LanguageCultureName = "en-AU"
                        },
                        new
                        {
                            Id = 5,
                            DisplayName = "Russian - Russia",
                            LanguageCultureName = "ru-RU"
                        });
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Media", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<int?>("GroupOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("Mediae");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.MediaGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("MediaGroups");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.MediaName", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("LanguageId")
                        .HasColumnType("int");

                    b.Property<int>("MediaId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.HasIndex("MediaId");

                    b.HasIndex("Name");

                    b.ToTable("MediaNames");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Purchase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("CurrencyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PurchaseTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(GetDate())");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("ArtworkArtworkArtist", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.ArtworkArtist", null)
                        .WithMany()
                        .HasForeignKey("ArtistsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Artwork", null)
                        .WithMany()
                        .HasForeignKey("ArtworksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ArtworkArtworkCharacter", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Artwork", null)
                        .WithMany()
                        .HasForeignKey("ArtworksId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.ArtworkCharacter", null)
                        .WithMany()
                        .HasForeignKey("CharactersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.ArtistName", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.ArtworkArtist", null)
                        .WithMany("Names")
                        .HasForeignKey("ArtworkArtistId");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.ArtistProfile", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.ArtworkArtist", null)
                        .WithMany("Profiles")
                        .HasForeignKey("ArtworkArtistId");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Asset", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Artwork", "Artwork")
                        .WithOne("Asset")
                        .HasForeignKey("Smoehring.Home.Data.SqlDatabase.Models.Asset", "ArtworkId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.AssetState", "AssetState")
                        .WithMany("Assets")
                        .HasForeignKey("AssetStateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.AssetType", "AssetType")
                        .WithMany("Assets")
                        .HasForeignKey("AssetTypeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Brand", "Brand")
                        .WithMany("Assets")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Device", "Device")
                        .WithOne("Asset")
                        .HasForeignKey("Smoehring.Home.Data.SqlDatabase.Models.Asset", "DeviceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Media", "Media")
                        .WithOne("Asset")
                        .HasForeignKey("Smoehring.Home.Data.SqlDatabase.Models.Asset", "MediaId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Purchase", "Purchase")
                        .WithOne("Asset")
                        .HasForeignKey("Smoehring.Home.Data.SqlDatabase.Models.Asset", "PurchaseId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Artwork");

                    b.Navigation("AssetState");

                    b.Navigation("AssetType");

                    b.Navigation("Brand");

                    b.Navigation("Device");

                    b.Navigation("Media");

                    b.Navigation("Purchase");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.AssetFile", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Asset", "Asset")
                        .WithMany("Files")
                        .HasForeignKey("AssetId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Asset");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Currency", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Language", "Language")
                        .WithMany("Currencies")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Language");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Media", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.MediaGroup", "Group")
                        .WithMany("Mediae")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Group");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.MediaName", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Language", "Language")
                        .WithMany("MediaNames")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Media", "Media")
                        .WithMany("MediaNames")
                        .HasForeignKey("MediaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Language");

                    b.Navigation("Media");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Purchase", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Currency", "Currency")
                        .WithMany("Purchases")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Artwork", b =>
                {
                    b.Navigation("Asset");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.ArtworkArtist", b =>
                {
                    b.Navigation("Names");

                    b.Navigation("Profiles");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Asset", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.AssetState", b =>
                {
                    b.Navigation("Assets");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.AssetType", b =>
                {
                    b.Navigation("Assets");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Brand", b =>
                {
                    b.Navigation("Assets");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Currency", b =>
                {
                    b.Navigation("Purchases");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Device", b =>
                {
                    b.Navigation("Asset")
                        .IsRequired();
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Language", b =>
                {
                    b.Navigation("Currencies");

                    b.Navigation("MediaNames");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Media", b =>
                {
                    b.Navigation("Asset")
                        .IsRequired();

                    b.Navigation("MediaNames");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.MediaGroup", b =>
                {
                    b.Navigation("Mediae");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Purchase", b =>
                {
                    b.Navigation("Asset")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
