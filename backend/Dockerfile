FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY backend/ ./backend/

WORKDIR /src/backend/src/FairWorkly.API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# ── Migrate stage — runs EF Core migrations, then exits ───────────────────────
FROM build AS migrate
RUN dotnet tool install --global dotnet-ef --version 8.*
ENV PATH="$PATH:/root/.dotnet/tools"
WORKDIR /src/backend/src/FairWorkly.Infrastructure
ENTRYPOINT ["dotnet", "ef", "database", "update", \
            "--project", "/src/backend/src/FairWorkly.Infrastructure", \
            "--startup-project", "/src/backend/src/FairWorkly.API", \
            "--no-build"]

# ── Runtime image ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5680

COPY --from=build /app/publish .

EXPOSE 5680

ENTRYPOINT ["dotnet", "FairWorkly.API.dll"]
