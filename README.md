# Home

Personal asset library (media + insurance inventory with serial numbers, purchases, valuations), digital artwork manager and recipes.
Authentication uses Azure Entra ID; everything else is cloud-neutral (PostgreSQL, local/PVC file storage, runs on k3s).

See [docs/DesignDocumentation.md](docs/DesignDocumentation.md) for the design.

## Layout (clean architecture)

| Path | Role | Depends on |
|---|---|---|
| `src/Home.Domain` | Entities, business rules, Gantt scheduler | nothing |
| `src/Home.Application` | Use-case services, ports (`IApplicationDbContext`, `IFileStore`) | Domain |
| `src/Home.Infrastructure` | EF Core + PostgreSQL (Npgsql), migrations, local file store | Application |
| `src/Home.Web` | Blazor Server UI, Entra ID auth, health checks, file endpoint | Application, Infrastructure (composition root) |
| `tests/*` | Domain / Application (in-memory EF) / Infrastructure (+ PostgreSQL) tests | |
| `deploy/` | Dockerfile and Kubernetes manifests | |
| `legacy/` | The previous .NET 8 / SQL Server / Azure implementation (reference only) | |

## Run locally

```bash
docker compose up -d                      # PostgreSQL
dotnet run --project src/Home.Web         # Development: auto-migrates, no login if AzureAd:ClientId is empty
dotnet test                               # set HOME_TEST_POSTGRES="Host=localhost;Username=home;Password=home" to include PostgreSQL integration tests
```

Add a migration: `dotnet ef migrations add <Name> -p src/Home.Infrastructure -o Persistence/Migrations`.

## Configuration (environment variables)

`ConnectionStrings__Postgres`, `AzureAd__TenantId`, `AzureAd__ClientId`, `AzureAd__ClientSecret`, `Storage__LocalPath`, `DataProtection__KeysPath`, `Database__AutoMigrate`, `OTEL_EXPORTER_OTLP_ENDPOINT`.
Outside Development the app refuses to start without `AzureAd:ClientId`.
