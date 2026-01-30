FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY backend/ ./backend/

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet restore -r linux-x64

RUN dotnet publish -c Release -o /app/publish --no-restore

RUN /root/.dotnet/tools/dotnet-ef migrations bundle \
    --project . \
    --runtime linux-x64 \
    --self-contained \
    -o /app/publish/migrate

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5680

COPY --from=build /app/publish .
COPY --from=build /src/backend/src/FairWorkly.API/appsettings.example.json ./appsettings.json

EXPOSE 5680

ENTRYPOINT ["dotnet", "FairWorkly.API.dll"]
