# MarketData Trading Application Docker Installation

This project provides a trading application backend with PostgreSQL database, fully containerized with Docker.

---

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) installed on your machine
- [Docker Compose](https://docs.docker.com/compose/install/) installed (usually comes with Docker Desktop)

---

## How to run the solution

Follow these steps to run the entire solution using Docker:

### 1. Clone the repository

```bash
git clone https://github.com/denys-pavskyi/MarketData.git

cd MarketData  (main root folder)
```

### 2. Build and start the containers

```bash
docker-compose up --build
```

This command will:
 - Build the ASP.NET API Docker image,
 - Pull the PostgreSQL image,
 - Start both containers and create a Docker network between them,
 - Apply any pending Entity Framework migrations automatically.
 

# API Usage

The application exposes a simple REST API for retrieving and updating market data. Once the containers are running Swagger UI is available at:

```bash
http://localhost:8080/swagger
```

## 🔹 GET /api/assets

Returns a list of supported market assets stored in the local database.
If the database is empty, the service will fetch and cache asset data from an external provider.

Example request:

```http
GET http://localhost:8080/api/assets
```

Response:

```json
[
  {
    "id": "ec15a527-5a7c-47ac-9357-24a051efaa1e",
    "symbol": "EUR/USD",
    "provider": "oanda",
    "kind": "forex",
    "description": "Euro vs US Dollar"
  },
  ...
]
```

## 🔹 POST /api/assets/prices

Fetches the latest price and historical bar data for one or more instruments from an external provider.

Request body

```json
[[
  {
    "instrumentId": "ec15a527-5a7c-47ac-9357-24a051efaa1e",
    "provider": "oanda",
    "interval": 1,
    "periodicity": "minute",
    "barsCount": 10
  }
]
```

You can also send multiple requests in a single call:

```json
[
  {
    "instrumentId": "6aa6b522-4de6-454e-b5b9-c08a67089206",
    "provider": "dxfeed",
    "interval": 1,
    "periodicity": "minute",
    "barsCount": 10
  },
  {
    "instrumentId": "ec15a527-5a7c-47ac-9357-24a051efaa1e",
    "provider": "oanda",
    "interval": 1,
    "periodicity": "minute",
    "barsCount": 10
  }
]
```

Response:

```json
[
  {
    "instrumentId": "ec15a527-5a7c-47ac-9357-24a051efaa1e",
    "provider": "oanda",
    "price": 1.60289,
    "updateTime": "2025-07-07T00:02:32.2718688+00:00",
    "historicalBars": [
      {
        "time": "2025-07-06T23:53:00+00:00",
        "open": 1.60224,
        "high": 1.60234,
        "low": 1.60221,
        "close": 1.60226,
        "volume": 111
      },
      ...
    ]
  }
]
```

Notes:
 - instrumentId must match a valid instrument stored in the database (from /api/assets)
 - provider should match the one associated with the instrument
 - Supported values for periodicity: "minute", "hour", etc.
 - barsCount defines how many historical bars to return