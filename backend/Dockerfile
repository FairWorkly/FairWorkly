FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY backend/ ./backend/

WORKDIR /src/backend/src/FairWorkly.API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5680

COPY --from=build /app/publish .

EXPOSE 5680

ENTRYPOINT ["dotnet", "FairWorkly.API.dll"]
