# Better Tracker

A job application tracking tool for managing your job search process. Keep track of applications, notes, and progress all in one place.

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
  - [Backend](#backend)
  - [Frontend](#frontend)
  - [Infrastructure](#infrastructure)
  - [API Schema Generation](#api-schema-generation)
- [Installation](#installation)
  - [Prerequisites](#prerequisites)
  - [Environment Setup](#environment-setup)
  - [Environment Variables](#environment-variables)
  - [Running with Docker](#running-with-docker)
  - [Local Development Without Docker](#local-development-without-docker)

## Features

- Create, update, and delete job applications
- Archive applications to keep your active list clean
- Add comments to job applications for interview notes and follow-ups
- Tag system for organizing applications by categories
- Notes section for general job search thoughts
- Statistics page with charts showing application progress
- Dark and light theme support

## Tech Stack

### Backend
- .NET 10.0 / C# with ASP.NET Core Web API
- Clean Architecture with CQRS pattern (separate commands and queries)
- SQLite database with Entity Framework Core
- JWT bearer token authentication
- FluentValidation for request validation
- OpenAPI documentation via Scalar
- API versioning (v1)
- xUnit tests with FluentAssertions and NSubstitute

### Frontend
- React 19 with TypeScript (Single Page Application)
- Vite build tool
- TanStack Router for file-based routing
- Tailwind CSS with shadcn/ui components
- Type-safe API client using openapi-fetch
- Recharts for statistics visualization
- Zod for environment variable validation
- next-themes for dark/light mode

### Infrastructure
- Docker containers for both frontend and backend
- docker-compose for local development
- nginx for serving the frontend SPA
- Multi-stage Docker builds

### API Schema Generation
The backend exposes an OpenAPI schema that is used to auto-generate type-safe frontend API client code. After starting the backend, run:

```bash
cd frontend
npm run generate
```

This runs the `generate` script which fetches the OpenAPI spec from `VITE_API_URL` and generates TypeScript types to `src/libs/api.schema.g.ts`, keeping the frontend API client in sync with backend endpoint definitions.


## Installation

### Prerequisites
- Docker and docker-compose installed

### Environment Setup

Create a `.env` file in the project root:

```
AUTH_SECRET=your-secret-key-min-32-characters-long
AUTH_TOKEN_TTL_MINUTES=60
VITE_API_URL=http://localhost:5000
```

The `AUTH_SECRET` must be at least 32 characters long and is used to sign JWT tokens.

### Environment Variables

| Variable | Required | Description | Default |
|----------|----------|-------------|---------|
| `AUTH_SECRET` | Yes | JWT signing secret (min 32 characters) | - |
| `AUTH_TOKEN_TTL_MINUTES` | No | Token expiration time in minutes | `60` |
| `VITE_API_URL` | No | Backend API URL for frontend | `http://localhost:5000` |
| `ConnectionStrings__DefaultConnection` | No | SQLite connection string (Docker) | `Data Source=/app/data/better-tracker.db` |
| `ASPNETCORE_ENVIRONMENT` | No | Backend environment name | `Development` |

For local development without Docker, create `.env` in the frontend directory with `VITE_API_URL` pointing to your backend.

### Running with Docker

```bash
docker-compose up --build -d
```

Services:
- Backend API at `http://localhost:5000`
- Frontend at `http://localhost:3000`

### Local Development Without Docker

**Backend:**
```bash
cd backend
dotnet restore
dotnet run --project src/BetterTracker
```

The backend uses SQLite and will create the database file automatically using EF Core migrations.

**Frontend:**
```bash
cd frontend
npm install
npm run dev
```

The frontend expects the backend to be running at the URL specified in `VITE_API_URL`.
