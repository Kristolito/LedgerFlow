# LedgerFlow

Production-grade .NET 8 Clean Architecture foundation for LedgerFlow.

## Solution Structure

```
/src
  /LedgerFlow.Api
  /LedgerFlow.Application
  /LedgerFlow.Domain
  /LedgerFlow.Infrastructure
  /LedgerFlow.Worker
/docs
docker-compose.yml
LedgerFlow.sln
README.md
```

## Architecture

- `LedgerFlow.Api` -> references `LedgerFlow.Application`, `LedgerFlow.Infrastructure`
- `LedgerFlow.Application` -> references `LedgerFlow.Domain`
- `LedgerFlow.Infrastructure` -> references `LedgerFlow.Application`, `LedgerFlow.Domain`
- `LedgerFlow.Worker` -> references `LedgerFlow.Application`, `LedgerFlow.Infrastructure`

## Foundation Included

- ASP.NET Core Web API (.NET 8)
- Swagger/OpenAPI with JWT bearer security definition
- `GET /health` health checks endpoint
- Correlation ID middleware (`X-Correlation-Id`) with Serilog enrichment
- Serilog structured logging with console sink and request logging middleware
- EF Core + Npgsql wiring via `LedgerFlowDbContext`
- Docker Compose with PostgreSQL, Redis, and API

## Configuration

API reads:
- `ConnectionStrings:Postgres` (supports env var `ConnectionStrings__Postgres`)
- `Redis:Host` (supports env var `Redis__Host`)

## Build and Run

### Build

```powershell
dotnet build LedgerFlow.sln
```

### Run API locally

```powershell
dotnet run --project src/LedgerFlow.Api
```

### Run worker locally

```powershell
dotnet run --project src/LedgerFlow.Worker
```

### Run with Docker Compose

```powershell
docker compose up --build
```

## Verify

- Swagger: `http://localhost:8080/swagger` (Docker) or local Kestrel URL when running `dotnet run`
- Health: `GET http://localhost:8080/health`
- Correlation header: response includes `X-Correlation-Id`
