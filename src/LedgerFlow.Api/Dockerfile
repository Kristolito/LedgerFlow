FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LedgerFlow.sln", "./"]
COPY ["src/LedgerFlow.Api/LedgerFlow.Api.csproj", "src/LedgerFlow.Api/"]
COPY ["src/LedgerFlow.Application/LedgerFlow.Application.csproj", "src/LedgerFlow.Application/"]
COPY ["src/LedgerFlow.Domain/LedgerFlow.Domain.csproj", "src/LedgerFlow.Domain/"]
COPY ["src/LedgerFlow.Infrastructure/LedgerFlow.Infrastructure.csproj", "src/LedgerFlow.Infrastructure/"]

RUN dotnet restore "src/LedgerFlow.Api/LedgerFlow.Api.csproj"

COPY . .
RUN dotnet publish "src/LedgerFlow.Api/LedgerFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "LedgerFlow.Api.dll"]
