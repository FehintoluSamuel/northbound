# Northbound Sessions

Daily market education platform. A structured lesson, auto-generated slides,
and a handout are released automatically each weekday; the class meets live
twice a week over Google Meet.

See [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) for the system design and
[`docs/REQUIREMENTS.md`](docs/REQUIREMENTS.md) for what this project must
and must not do.

## Project structure

```
NorthboundSessions.sln
src/
  NorthboundSessions.Web/     # Blazor Server app — UI + backend, Identity auth
  NorthboundSessions.Data/    # EF Core models, DbContext, migrations
  NorthboundSessions.Jobs/    # Console app: generates slides/handouts,
                               #   run on a schedule by an Azure Container Apps Job
docs/
  ARCHITECTURE.md
  REQUIREMENTS.md
.github/workflows/
  ci.yml                      # Build + test on every push/PR
Dockerfile
docker-compose.yml            # Local SQL Server, mirrors Azure SQL in production
```

## Running locally

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download) (current LTS) and
   [Docker Desktop](https://www.docker.com/products/docker-desktop/).
2. Start the local database: `docker compose up -d`
3. Set your local connection string (never commit real credentials):
   ```
   cd src/NorthboundSessions.Web
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=northbound_dev;User Id=sa;Password=LocalDevPassword1!;TrustServerCertificate=True"
   ```
4. Apply migrations: `dotnet ef database update`
5. Run the app: `dotnet run`

## CI/CD

Every push to `main` and every pull request triggers `.github/workflows/ci.yml`,
which restores, builds, and tests the solution. This is a quality gate, not a
deploy step — deployment to Azure Container Apps happens separately (see
ARCHITECTURE.md).
