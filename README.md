# FairWorkly

FairWorkly is a B2B SaaS platform that helps Australian SMEs make compliant, auditable, and explainable HR decisions.

👉 **New here? Start with:**  
📘 [Project Overview – How FairWorkly Works](docs/00-project-overview.md)

## What We Are Building

FairWorkly focuses on **agent-driven HR compliance**, with the following core agents in the MVP:

- **Compliance Agent**  
  Checks whether rosters and working arrangements comply with relevant Award rules.

- **Payroll Agent**  
  Validates whether _already-calculated_ pay outcomes comply with Award rules.

  > FairWorkly does **not** calculate payroll. Payroll calculation is handled by external systems.

- **FairBot**  
  Provides guided interaction and explanations across agents, helping users understand compliance outcomes.

## Getting Started

This section is intentionally minimal. Detailed architecture and workflows are documented separately.

### Frontend

cd frontend
pnpm install
pnpm dev

### Backend

cd backend
dotnet restore
dotnet run

## 📚 Documentation

Start here to understand how FairWorkly works and how to contribute safely.

1. 📘 [Project Overview – How FairWorkly Works](docs/00-project-overview.md)
2. 🧠 [Frontend Architecture Cheat Sheet](docs/01-frontend-architecture.md)
3. 🏗️ [Backend Architecture & Rules](docs/02-backend-architecture.md)
4. 🔄 [Development Workflow & PR Rules](docs/03-dev-workflow-and-pr.md)
