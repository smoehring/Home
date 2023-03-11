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
    [Migration("20230311153422_AddedPrintIndicator")]
    partial class AddedPrintIndicator
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Asset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

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

                    b.Property<DateTimeOffset>("PurchaseTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetimeoffset")
                        .HasDefaultValueSql("(GetDate())");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Asset", b =>
                {
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

                    b.Navigation("AssetType");

                    b.Navigation("Brand");

                    b.Navigation("Device");

                    b.Navigation("Media");

                    b.Navigation("Purchase");
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

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Purchase", b =>
                {
                    b.HasOne("Smoehring.Home.Data.SqlDatabase.Models.Currency", "Currency")
                        .WithMany("Purchases")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");
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
                });

            modelBuilder.Entity("Smoehring.Home.Data.SqlDatabase.Models.Media", b =>
                {
                    b.Navigation("Asset")
                        .IsRequired();
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
