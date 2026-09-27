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
docker-compose.yml            # Local PostgreSQL, matches the app's Npgsql provider
```

## Database

The app uses **PostgreSQL** via the Npgsql EF Core provider
(`UseNpgsql` in `Program.cs`), and the checked-in migrations in
`src/NorthboundSessions.Web/Migrations` are Postgres migrations. `docker compose`
brings up a matching PostgreSQL 17 instance on port `5432`.

## Running locally

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download) (current LTS) and
   [Docker Desktop](https://www.docker.com/products/docker-desktop/).
2. Start the local database: `docker compose up -d`
3. Set your local connection string (never commit real credentials):
   ```
   cd src/NorthboundSessions.Web
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=northbound_dev;Username=northbound;Password=LocalDevPassword1!"
   ```
4. Apply migrations: `dotnet ef database update`
5. Run the app: `dotnet run`

### Granting the Instructor role

Nothing is granted automatically on boot. To give an account the `Instructor`
role (needed for `/admin/*`), list its email(s):
```
dotnet user-secrets set "Northbound:InstructorEmails:0" "you@example.com"
```
The app creates the role if it's missing and assigns it on the next start,
logging what it did. Addresses that have no matching user are logged as a
warning and skipped.

## CI/CD

Every push to `main` and every pull request triggers `.github/workflows/ci.yml`,
which restores, builds, and tests the solution. This is a quality gate, not a
deploy step — deployment to Azure Container Apps happens separately (see
ARCHITECTURE.md).
