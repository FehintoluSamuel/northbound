# Architecture

## Overview

Northbound Sessions replaces a daily, manually-run live trading class with a
hybrid model: self-paced weekday lessons with automated slides and handouts,
auto-graded quizzes, and two live sessions per week. Built solo, part-time,
on a $0 infrastructure budget.

## Stack

| Layer | Choice | Why |
|---|---|---|
| UI framework | Blazor Server (.NET 10 LTS) | Real-time updates (attendance, quiz results) via built-in SignalR, no separate real-time layer needed. .NET 10 is the current LTS (supported through Nov 2028); .NET 8 reaches end of support Nov 2026 |
| Data access | Entity Framework Core | Standard, well-documented |
| Database | **PostgreSQL** (see note) | App uses the Npgsql EF Core provider and the checked-in migrations are Postgres migrations |
| Auth | ASP.NET Core Identity | Built-in roles (Student / Instructor) |
| Slide generation | Open XML SDK | Generates real .pptx files, no Office install needed |
| Handout generation | QuestPDF (Community license) | Clean PDF generation in C# |
| Hosting | Azure Container Apps | Free monthly compute grant, native Docker support |

> **Unresolved: database target.** The design decisions below record an
> intent to use Azure SQL Database, but the implementation is PostgreSQL
> (`UseNpgsql` in `Program.cs`, `Npgsql.EntityFrameworkCore.PostgreSQL`,
> and `InitialPostgresMigration`). These are mutually exclusive and the
> mismatch is not cosmetic — production hosting has to pick one:
>
> - **Keep PostgreSQL** — no code change. For Azure, that means Azure
>   Database for PostgreSQL (Flexible Server) instead of Azure SQL, and
>   `docker-compose.yml` / `appsettings.json` / README already match.
> - **Switch to Azure SQL** — swap the provider package to
>   `Microsoft.EntityFrameworkCore.SqlServer`, change
>   `UseNpgsql(...)` to `UseSqlServer(...)`, and regenerate the
>   initial migration. Existing local data would be lost.
| Automation | Azure Container Apps Job (cron trigger) | Same Docker image as the web app, runs on a schedule natively — no external cron workaround needed |
| Email | Gmail SMTP (app password) | Free at low volume |
| Live sessions | Google Meet (personal account) | Free, reliable |
| CI | GitHub Actions | Build + test on every push/PR (not deployment — see below) |

## Why Blazor Server specifically

Live attendance check-ins and real-time quiz results need push-style UI
updates. Blazor Server runs over SignalR natively, so this comes without
building a parallel real-time layer. The usual concern with Blazor Server —
many persistent connections straining the server — isn't a real issue at a
class size under 20.

## Deployment

Azure Container Apps builds directly from the repo's Dockerfile. CI
(ci.yml) is a quality gate that runs before merge; it does not perform the
deployment itself. Deployment is configured separately in Azure.

## Content management

Lesson, topic, and live-session content is created through the `/admin`
panel and stored in database tables, so adding content is a data change
rather than a code change. See the data model in NorthboundSessions.Data for
the current table list.

The marketing landing page is **not** managed this way: it is the static
`wwwroot/index.html` served at `/` by `Program.cs`, and its copy, FAQ,
testimonials, and instructor bio are hardcoded in that file. The `/admin`
panel has no editor for it. Moving it into the database (and adding the
corresponding admin screens) is outstanding work.

## Decisions log

- 2026, Hosting: Originally planned for Render.com (no card required at
  the time). Once Azure access became available, switched to Azure Container
  Apps + Azure SQL Database (free offer) for native cron support via
  Container Apps Jobs and no cold-start sleep behavior.
- 2026, Database (unresolved): The hosting decision above assumed Azure SQL,
  but the app was implemented against PostgreSQL. Local compose, the default
  connection string, and the README were corrected to PostgreSQL to match the
  code; the production target still needs a decision.
- 2026, Branding: "Northbound Sessions" chosen after "TradeLoop" was
  found to conflict with an existing trademark.
