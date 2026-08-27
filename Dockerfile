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

# Azure Container Apps lets you configure the target port directly in the
# ingress settings (unlike Render, which injects PORT at runtime) — so we
# can just fix it here.
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "NorthboundSessions.Web.dll"]
