FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY FairWorkly.sln .

COPY src/FairWorkly.API/FairWorkly.API.csproj src/FairWorkly.API/
COPY src/FairWorkly.Application/FairWorkly.Application.csproj src/FairWorkly.Application/
COPY src/FairWorkly.Domain/FairWorkly.Domain.csproj src/FairWorkly.Domain/
COPY src/FairWorkly.Infrastructure/FairWorkly.Infrastructure.csproj src/FairWorkly.Infrastructure/

RUN dotnet restore src/FairWorkly.API/FairWorkly.API.csproj

COPY src/ src/

RUN dotnet build src/FairWorkly.API/FairWorkly.API.csproj -c Debug

EXPOSE 5680

CMD ["dotnet", "run", "--project", "src/FairWorkly.API/FairWorkly.API.csproj", "--urls", "http://0.0.0.0:5680"]