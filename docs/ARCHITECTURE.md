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
| Database | Azure SQL Database — free offer | 100,000 vCore-seconds + 32GB/month, free for the life of the subscription |
| Auth | ASP.NET Core Identity | Built-in roles (Student / Instructor) |
| Slide generation | Open XML SDK | Generates real .pptx files, no Office install needed |
| Handout generation | QuestPDF (Community license) | Clean PDF generation in C# |
| Hosting | Azure Container Apps | Free monthly compute grant, native Docker support |
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

Landing page content (instructor bios, testimonials, FAQ, curriculum
modules) lives in database tables, editable through an /admin panel, not
hardcoded in Razor markup. This means adding a second instructor or updating
a bio is a data change, not a code change. See the data model in
NorthboundSessions.Data for the current table list.

## Decisions log

- 2026, Hosting: Originally planned for Render.com (no card required at
  the time). Once Azure access became available, switched to Azure Container
  Apps + Azure SQL Database (free offer) for native cron support via
  Container Apps Jobs and no cold-start sleep behavior.
- 2026, Branding: "Northbound Sessions" chosen after "TradeLoop" was
  found to conflict with an existing trademark.
