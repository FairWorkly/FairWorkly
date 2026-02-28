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
# 1. Copy non-secret environment file
cp frontend/.env.example frontend/.env

# 2. Export secrets in your local shell (do NOT commit into .env)
export OPENAI_API_KEY="your-openai-key"
export AGENT_SERVICE_KEY="your-shared-service-key"
export AiSettings__ServiceKey="$AGENT_SERVICE_KEY"

# 3. Start all services
docker compose up --build
```

### Option 2: Manual Setup

**Quick start (recommended):**

```bash
# 1. Copy non-secret environment files
cp frontend/.env.example frontend/.env
cp backend/src/FairWorkly.API/appsettings.Development.example.json backend/src/FairWorkly.API/appsettings.Development.json

# 2. Export secrets in your local shell
export OPENAI_API_KEY="your-openai-key"
export AGENT_SERVICE_KEY="your-shared-service-key"
export AiSettings__ServiceKey="$AGENT_SERVICE_KEY"

# 3. Validate local config consistency
make doctor

# 4. Start frontend + backend + agent-service
make dev-up
```

Stop all services started by `make dev-up`:

```bash
make dev-down
```

**Start each service manually:**

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
cd agent-service && poetry install && poetry run uvicorn master_agent.main:app --reload
```

### Ports

| Service       | URL                   |
| ------------- | --------------------- |
| Frontend      | http://localhost:5173 |
| Backend API   | http://localhost:5680 |
| Agent Service | http://localhost:8000 |

### Local Secret Injection (Recommended)

Use shell startup files for local-only secrets:

```bash
# ~/.zshrc or ~/.zprofile
export OPENAI_API_KEY="your-openai-key"
export AGENT_SERVICE_KEY="your-shared-service-key"
export AiSettings__ServiceKey="$AGENT_SERVICE_KEY"
```

Then reload and verify:

```bash
source ~/.zshrc
make doctor
```

Do not place secrets in:
- `frontend/.env`
- `agent-service/.env`
- any tracked `appsettings*.json`

## 📚 Documentation

Start here to understand how FairWorkly works and how to contribute safely.

1. 📘 [Project Overview – How FairWorkly Works](docs/00-project-overview.md)
2. 🧠 [Frontend Architecture Cheat Sheet](docs/01-frontend-architecture.md)
3. 🏗️ [Backend Architecture & Rules](docs/02-backend-architecture.md)
4. 🔄 [Development Workflow & PR Rules](docs/03-dev-workflow-and-pr.md)
