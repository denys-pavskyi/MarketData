# MarketData Trading Application

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
 
### 3. Access the API

Once the containers are running, open your browser and navigate to:

http://localhost:8080/swagger