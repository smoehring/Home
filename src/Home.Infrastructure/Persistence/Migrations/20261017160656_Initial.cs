using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Smoehring.Home.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "home");

            migrationBuilder.CreateTable(
                name: "artists",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artists", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "artwork_sets",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    planned_count = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artwork_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "asset_states",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    possession = table.Column<bool>(type: "boolean", nullable: false),
                    ownership = table.Column<bool>(type: "boolean", nullable: false),
                    is_default = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "asset_types",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    requires_serial_number = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    series = table.Column<string>(type: "text", nullable: true),
                    is_owned = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    symbol = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    culture_name = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "media_groups",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recipes",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    servings = table.Column<int>(type: "integer", nullable: false),
                    source_url = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "artist_name",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    artist_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artist_name", x => x.id);
                    table.ForeignKey(
                        name: "fk_artist_name_artists_artist_id",
                        column: x => x.artist_id,
                        principalSchema: "home",
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artist_profile",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    artist_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artist_profile", x => x.id);
                    table.ForeignKey(
                        name: "fk_artist_profile_artists_artist_id",
                        column: x => x.artist_id,
                        principalSchema: "home",
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assets",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    location = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    print_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    brand_id = table.Column<int>(type: "integer", nullable: true),
                    asset_type_id = table.Column<int>(type: "integer", nullable: true),
                    asset_state_id = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_assets_asset_states_asset_state_id",
                        column: x => x.asset_state_id,
                        principalSchema: "home",
                        principalTable: "asset_states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_assets_asset_types_asset_type_id",
                        column: x => x.asset_type_id,
                        principalSchema: "home",
                        principalTable: "asset_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_assets_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "home",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recipe_iterations",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    recipe_id = table.Column<int>(type: "integer", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    change_notes = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<int>(type: "integer", nullable: true),
                    cooked_on = table.Column<DateOnly>(type: "date", nullable: true),
                    based_on_iteration_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_iterations", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_iterations_recipe_iterations_based_on_iteration_id",
                        column: x => x.based_on_iteration_id,
                        principalSchema: "home",
                        principalTable: "recipe_iterations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_recipe_iterations_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "home",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artworks",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    asset_id = table.Column<int>(type: "integer", nullable: false),
                    is_adult = table.Column<bool>(type: "boolean", nullable: false),
                    stage = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artworks", x => x.id);
                    table.ForeignKey(
                        name: "fk_artworks_assets_asset_id",
                        column: x => x.asset_id,
                        principalSchema: "home",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asset_files",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_id = table.Column<int>(type: "integer", nullable: false),
                    uuid = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    original_file_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: true),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_files_assets_asset_id",
                        column: x => x.asset_id,
                        principalSchema: "home",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asset_state_change",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_id = table.Column<int>(type: "integer", nullable: false),
                    from_state_id = table.Column<int>(type: "integer", nullable: false),
                    to_state_id = table.Column<int>(type: "integer", nullable: false),
                    at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_state_change", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_state_change_assets_asset_id",
                        column: x => x.asset_id,
                        principalSchema: "home",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "asset_valuation",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency_id = table.Column<int>(type: "integer", nullable: false),
                    valued_on = table.Column<DateOnly>(type: "date", nullable: false),
                    reason = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asset_valuation", x => x.id);
                    table.ForeignKey(
                        name: "fk_asset_valuation_assets_asset_id",
                        column: x => x.asset_id,
                        principalSchema: "home",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asset_valuation_currencies_currency_id",
                        column: x => x.currency_id,
                        principalSchema: "home",
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "device",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_id = table.Column<int>(type: "integer", nullable: false),
                    serial_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    model_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_assets_asset_id",
                        column: x => x.asset_id,
                        principalSchema: "home",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mediae",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_id = table.Column<int>(type: "integer", nullable: false),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: true),
                    group_order = table.Column<int>(type: "integer", nullable: true),
                    release_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mediae", x => x.id);
                    table.ForeignKey(
                        name: "fk_mediae_assets_asset_id",
                        column: x => x.asset_id,
                        principalSchema: "home",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_mediae_media_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "home",
                        principalTable: "media_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "purchase",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asset_id = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency_id = table.Column<int>(type: "integer", nullable: false),
                    purchase_date = table.Column<DateOnly>(type: "date", nullable: false),
                    vendor = table.Column<string>(type: "text", nullable: true),
                    order_number = table.Column<string>(type: "text", nullable: true),
                    warranty_until = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_purchase", x => x.id);
                    table.ForeignKey(
                        name: "fk_purchase_assets_asset_id",
                        column: x => x.asset_id,
                        principalSchema: "home",
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_purchase_currencies_currency_id",
                        column: x => x.currency_id,
                        principalSchema: "home",
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recipe_ingredient",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    iteration_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(12,3)", precision: 12, scale: 3, nullable: true),
                    unit = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_ingredient", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_ingredient_recipe_iterations_iteration_id",
                        column: x => x.iteration_id,
                        principalSchema: "home",
                        principalTable: "recipe_iterations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_step",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    iteration_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    instructions = table.Column<string>(type: "text", nullable: true),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    start_offset = table.Column<TimeSpan>(type: "interval", nullable: true),
                    is_active_work = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_step", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_step_recipe_iterations_iteration_id",
                        column: x => x.iteration_id,
                        principalSchema: "home",
                        principalTable: "recipe_iterations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artwork_artist",
                schema: "home",
                columns: table => new
                {
                    artwork_id = table.Column<int>(type: "integer", nullable: false),
                    artist_id = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artwork_artist", x => new { x.artwork_id, x.artist_id, x.role });
                    table.ForeignKey(
                        name: "fk_artwork_artist_artists_artist_id",
                        column: x => x.artist_id,
                        principalSchema: "home",
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_artwork_artist_artworks_artwork_id",
                        column: x => x.artwork_id,
                        principalSchema: "home",
                        principalTable: "artworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artwork_character",
                schema: "home",
                columns: table => new
                {
                    artwork_id = table.Column<int>(type: "integer", nullable: false),
                    character_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artwork_character", x => new { x.artwork_id, x.character_id });
                    table.ForeignKey(
                        name: "fk_artwork_character_artworks_artwork_id",
                        column: x => x.artwork_id,
                        principalSchema: "home",
                        principalTable: "artworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artwork_character_characters_character_id",
                        column: x => x.character_id,
                        principalSchema: "home",
                        principalTable: "characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "artwork_set_item",
                schema: "home",
                columns: table => new
                {
                    set_id = table.Column<int>(type: "integer", nullable: false),
                    artwork_id = table.Column<int>(type: "integer", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    variant_of_position = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artwork_set_item", x => new { x.set_id, x.artwork_id });
                    table.ForeignKey(
                        name: "fk_artwork_set_item_artwork_sets_set_id",
                        column: x => x.set_id,
                        principalSchema: "home",
                        principalTable: "artwork_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artwork_set_item_artworks_artwork_id",
                        column: x => x.artwork_id,
                        principalSchema: "home",
                        principalTable: "artworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artwork_stage_change",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    artwork_id = table.Column<int>(type: "integer", nullable: false),
                    stage = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artwork_stage_change", x => x.id);
                    table.ForeignKey(
                        name: "fk_artwork_stage_change_artworks_artwork_id",
                        column: x => x.artwork_id,
                        principalSchema: "home",
                        principalTable: "artworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "artwork_payment",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    artwork_id = table.Column<int>(type: "integer", nullable: false),
                    artist_id = table.Column<int>(type: "integer", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency_id = table.Column<int>(type: "integer", nullable: false),
                    paid_on = table.Column<DateOnly>(type: "date", nullable: false),
                    kind = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    method = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    reference = table.Column<string>(type: "text", nullable: true),
                    invoice_file_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artwork_payment", x => x.id);
                    table.ForeignKey(
                        name: "fk_artwork_payment_artists_artist_id",
                        column: x => x.artist_id,
                        principalSchema: "home",
                        principalTable: "artists",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_artwork_payment_artworks_artwork_id",
                        column: x => x.artwork_id,
                        principalSchema: "home",
                        principalTable: "artworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artwork_payment_asset_files_invoice_file_id",
                        column: x => x.invoice_file_id,
                        principalSchema: "home",
                        principalTable: "asset_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_artwork_payment_currencies_currency_id",
                        column: x => x.currency_id,
                        principalSchema: "home",
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "artwork_version",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    artwork_id = table.Column<int>(type: "integer", nullable: false),
                    label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    parent_version_id = table.Column<int>(type: "integer", nullable: true),
                    asset_file_id = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_artwork_version", x => x.id);
                    table.ForeignKey(
                        name: "fk_artwork_version_artwork_version_parent_version_id",
                        column: x => x.parent_version_id,
                        principalSchema: "home",
                        principalTable: "artwork_version",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_artwork_version_artworks_artwork_id",
                        column: x => x.artwork_id,
                        principalSchema: "home",
                        principalTable: "artworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_artwork_version_asset_files_asset_file_id",
                        column: x => x.asset_file_id,
                        principalSchema: "home",
                        principalTable: "asset_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "media_external_id",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    media_id = table.Column<int>(type: "integer", nullable: false),
                    system = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_external_id", x => x.id);
                    table.ForeignKey(
                        name: "fk_media_external_id_mediae_media_id",
                        column: x => x.media_id,
                        principalSchema: "home",
                        principalTable: "mediae",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "media_name",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    media_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    language_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_media_name", x => x.id);
                    table.ForeignKey(
                        name: "fk_media_name_languages_language_id",
                        column: x => x.language_id,
                        principalSchema: "home",
                        principalTable: "languages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_media_name_mediae_media_id",
                        column: x => x.media_id,
                        principalSchema: "home",
                        principalTable: "mediae",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recipe_step_dependency",
                schema: "home",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    step_id = table.Column<int>(type: "integer", nullable: false),
                    depends_on_step_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recipe_step_dependency", x => x.id);
                    table.ForeignKey(
                        name: "fk_recipe_step_dependency_recipe_step_depends_on_step_id",
                        column: x => x.depends_on_step_id,
                        principalSchema: "home",
                        principalTable: "recipe_step",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_recipe_step_dependency_recipe_step_step_id",
                        column: x => x.step_id,
                        principalSchema: "home",
                        principalTable: "recipe_step",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "home",
                table: "asset_states",
                columns: new[] { "id", "is_default", "name", "ownership", "possession" },
                values: new object[,]
                {
                    { 1, true, "Available", true, true },
                    { 2, false, "Lost", false, false },
                    { 3, false, "Stolen", false, false },
                    { 4, false, "Broken", false, false },
                    { 5, false, "Loaned", true, false },
                    { 6, false, "Borrowed", false, true }
                });

            migrationBuilder.InsertData(
                schema: "home",
                table: "currencies",
                columns: new[] { "id", "code", "name", "symbol" },
                values: new object[,]
                {
                    { 1, "EUR", "Euro", "€" },
                    { 2, "USD", "US Dollar", "$" },
                    { 3, "AUD", "Australian Dollar", "$" },
                    { 4, "GBP", "Pound Sterling", "£" },
                    { 5, "RUB", "Russian Ruble", "₽" }
                });

            migrationBuilder.InsertData(
                schema: "home",
                table: "languages",
                columns: new[] { "id", "culture_name", "display_name" },
                values: new object[,]
                {
                    { 1, "de-DE", "German - Germany" },
                    { 2, "en-US", "English - United States" },
                    { 3, "en-GB", "English - United Kingdom" },
                    { 4, "en-AU", "English - Australia" },
                    { 5, "ru-RU", "Russian - Russia" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_artist_name_artist_id",
                schema: "home",
                table: "artist_name",
                column: "artist_id",
                unique: true,
                filter: "is_primary");

            migrationBuilder.CreateIndex(
                name: "ix_artist_name_name",
                schema: "home",
                table: "artist_name",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_artist_profile_artist_id",
                schema: "home",
                table: "artist_profile",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_artist_artist_id",
                schema: "home",
                table: "artwork_artist",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_character_character_id",
                schema: "home",
                table: "artwork_character",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_payment_artist_id",
                schema: "home",
                table: "artwork_payment",
                column: "artist_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_payment_artwork_id",
                schema: "home",
                table: "artwork_payment",
                column: "artwork_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_payment_currency_id",
                schema: "home",
                table: "artwork_payment",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_payment_invoice_file_id",
                schema: "home",
                table: "artwork_payment",
                column: "invoice_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_set_item_artwork_id",
                schema: "home",
                table: "artwork_set_item",
                column: "artwork_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_stage_change_artwork_id",
                schema: "home",
                table: "artwork_stage_change",
                column: "artwork_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_version_artwork_id",
                schema: "home",
                table: "artwork_version",
                column: "artwork_id",
                unique: true,
                filter: "is_primary");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_version_asset_file_id",
                schema: "home",
                table: "artwork_version",
                column: "asset_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_artwork_version_parent_version_id",
                schema: "home",
                table: "artwork_version",
                column: "parent_version_id");

            migrationBuilder.CreateIndex(
                name: "ix_artworks_asset_id",
                schema: "home",
                table: "artworks",
                column: "asset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_artworks_uuid",
                schema: "home",
                table: "artworks",
                column: "uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asset_files_asset_id",
                schema: "home",
                table: "asset_files",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_files_uuid",
                schema: "home",
                table: "asset_files",
                column: "uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asset_state_change_asset_id",
                schema: "home",
                table: "asset_state_change",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_types_name",
                schema: "home",
                table: "asset_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asset_valuation_asset_id",
                schema: "home",
                table: "asset_valuation",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "ix_asset_valuation_currency_id",
                schema: "home",
                table: "asset_valuation",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_assets_asset_state_id",
                schema: "home",
                table: "assets",
                column: "asset_state_id");

            migrationBuilder.CreateIndex(
                name: "ix_assets_asset_type_id",
                schema: "home",
                table: "assets",
                column: "asset_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_assets_brand_id",
                schema: "home",
                table: "assets",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "ix_assets_name",
                schema: "home",
                table: "assets",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_assets_uuid",
                schema: "home",
                table: "assets",
                column: "uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_brands_name",
                schema: "home",
                table: "brands",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_characters_name",
                schema: "home",
                table: "characters",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_currencies_code",
                schema: "home",
                table: "currencies",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_device_asset_id",
                schema: "home",
                table: "device",
                column: "asset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_device_serial_number",
                schema: "home",
                table: "device",
                column: "serial_number");

            migrationBuilder.CreateIndex(
                name: "ix_languages_culture_name",
                schema: "home",
                table: "languages",
                column: "culture_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_media_external_id_media_id",
                schema: "home",
                table: "media_external_id",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_external_id_system_value",
                schema: "home",
                table: "media_external_id",
                columns: new[] { "system", "value" });

            migrationBuilder.CreateIndex(
                name: "ix_media_groups_name",
                schema: "home",
                table: "media_groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_media_name_language_id",
                schema: "home",
                table: "media_name",
                column: "language_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_name_media_id",
                schema: "home",
                table: "media_name",
                column: "media_id");

            migrationBuilder.CreateIndex(
                name: "ix_media_name_name",
                schema: "home",
                table: "media_name",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "ix_mediae_asset_id",
                schema: "home",
                table: "mediae",
                column: "asset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_mediae_group_id",
                schema: "home",
                table: "mediae",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_purchase_asset_id",
                schema: "home",
                table: "purchase",
                column: "asset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_purchase_currency_id",
                schema: "home",
                table: "purchase",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_ingredient_iteration_id",
                schema: "home",
                table: "recipe_ingredient",
                column: "iteration_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_iterations_based_on_iteration_id",
                schema: "home",
                table: "recipe_iterations",
                column: "based_on_iteration_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_iterations_recipe_id_number",
                schema: "home",
                table: "recipe_iterations",
                columns: new[] { "recipe_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recipe_step_iteration_id",
                schema: "home",
                table: "recipe_step",
                column: "iteration_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_step_dependency_depends_on_step_id",
                schema: "home",
                table: "recipe_step_dependency",
                column: "depends_on_step_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipe_step_dependency_step_id",
                schema: "home",
                table: "recipe_step_dependency",
                column: "step_id");

            migrationBuilder.CreateIndex(
                name: "ix_recipes_uuid",
                schema: "home",
                table: "recipes",
                column: "uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "artist_name",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artist_profile",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artwork_artist",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artwork_character",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artwork_payment",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artwork_set_item",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artwork_stage_change",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artwork_version",
                schema: "home");

            migrationBuilder.DropTable(
                name: "asset_state_change",
                schema: "home");

            migrationBuilder.DropTable(
                name: "asset_valuation",
                schema: "home");

            migrationBuilder.DropTable(
                name: "device",
                schema: "home");

            migrationBuilder.DropTable(
                name: "media_external_id",
                schema: "home");

            migrationBuilder.DropTable(
                name: "media_name",
                schema: "home");

            migrationBuilder.DropTable(
                name: "purchase",
                schema: "home");

            migrationBuilder.DropTable(
                name: "recipe_ingredient",
                schema: "home");

            migrationBuilder.DropTable(
                name: "recipe_step_dependency",
                schema: "home");

            migrationBuilder.DropTable(
                name: "characters",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artists",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artwork_sets",
                schema: "home");

            migrationBuilder.DropTable(
                name: "artworks",
                schema: "home");

            migrationBuilder.DropTable(
                name: "asset_files",
                schema: "home");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "home");

            migrationBuilder.DropTable(
                name: "mediae",
                schema: "home");

            migrationBuilder.DropTable(
                name: "currencies",
                schema: "home");

            migrationBuilder.DropTable(
                name: "recipe_step",
                schema: "home");

            migrationBuilder.DropTable(
                name: "assets",
                schema: "home");

            migrationBuilder.DropTable(
                name: "media_groups",
                schema: "home");

            migrationBuilder.DropTable(
                name: "recipe_iterations",
                schema: "home");

            migrationBuilder.DropTable(
                name: "asset_states",
                schema: "home");

            migrationBuilder.DropTable(
                name: "asset_types",
                schema: "home");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "home");

            migrationBuilder.DropTable(
                name: "recipes",
                schema: "home");
        }
    }
}
