# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY NorthboundSessions.slnx .
COPY src/NorthboundSessions.Web/*.csproj src/NorthboundSessions.Web/
COPY src/NorthboundSessions.Data/*.csproj src/NorthboundSessions.Data/
COPY src/NorthboundSessions.Jobs/*.csproj src/NorthboundSessions.Jobs/
RUN dotnet restore NorthboundSessions.slnx

COPY . .
RUN dotnet publish src/NorthboundSessions.Web -c Release -o /app/publish

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Render injects PORT at container start (not build time), unlike Azure
# Container Apps which let us configure a fixed target port — so we
# resolve it via a shell at runtime instead.
ENV PORT=8080
EXPOSE 8080
ENTRYPOINT ["/bin/sh", "-c", "dotnet NorthboundSessions.Web.dll --urls http://+:${PORT}"]