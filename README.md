# FairWorkly

> **Status:** MVP Development

FairWorkly is a B2B SaaS platform that helps Australian SMEs make compliant, auditable, and explainable HR decisions.

## Tech Stack

| Layer          | Technology                                                     |
| -------------- | -------------------------------------------------------------- |
| Frontend       | React 18, TypeScript, Vite, MUI, TanStack Query, Redux Toolkit |
| Backend        | .NET 8, EF Core 8, MediatR, PostgreSQL                         |
| AI Agent       | Python 3.11, FastAPI, LangChain, FAISS                         |
| Infrastructure | Docker, GitHub Actions                                         |

## Prerequisites

- Node.js 18+
- .NET 8 SDK
- Python 3.11+
- PostgreSQL 15+
- Docker (optional)

## What We Are Building

FairWorkly focuses on **agent-driven HR compliance**, with the following core agents in the MVP:

- **Roster Agent**  
  Checks whether rosters and working arrangements comply with relevant Award rules.

- **Payroll Agent**  
  Validates whether _already-calculated_ pay outcomes comply with Award rules.

  > FairWorkly does **not** calculate payroll. Payroll calculation is handled by external systems.

- **FairBot**  
  Provides guided interaction and explanations across agents, helping users understand compliance outcomes.

## Project Structure

```
fairworkly/
├── frontend/          # React SPA
├── backend/           # .NET Web API
├── agent-service/     # Python AI agents
└── docs/              # Architecture & guides
```

## Getting Started

### Option 1: Docker (Recommended)

```bash
# 1. Copy environment files
cp frontend/.env.example frontend/.env
cp agent-service/.env.example agent-service/.env

# 2. Start all services
docker compose up --build
```

### Option 2: Manual Setup

**Frontend:**

```bash
cd frontend && npm install && npm run dev
```

**Backend:**

```bash
cd backend && dotnet restore && dotnet run --project src/FairWorkly.API
```

**Agent Service:**

```bash
cd agent-service && poetry install && poetry run uvicorn main:app --reload
```

### Ports

| Service       | URL                   |
| ------------- | --------------------- |
| Frontend      | http://localhost:5173 |
| Backend API   | http://localhost:5680 |
| Agent Service | http://localhost:8000 |

## 📚 Documentation

Start here to understand how FairWorkly works and how to contribute safely.

1. 📘 [Project Overview – How FairWorkly Works](docs/00-project-overview.md)
2. 🧠 [Frontend Architecture Cheat Sheet](docs/01-frontend-architecture.md)
3. 🏗️ [Backend Architecture & Rules](docs/02-backend-architecture.md)
4. 🔄 [Development Workflow & PR Rules](docs/03-dev-workflow-and-pr.md)
