# FairWorkly Frontend

React 18 + TypeScript + Vite SPA for FairWorkly, an AI-powered HR compliance copilot for Australian SMEs.

> Repo status: scaffolded. Feature modules, routes, state, and API layers exist; backend wiring is still pending.

## Quick start (from repo root)

```bash
cd FairWorkly/frontend
npm install
npm run dev       # Vite dev server (http://localhost:5173)
npm run build     # tsc -b + Vite production build
npm run preview   # serve the production build locally
npm run lint      # ESLint
```

Node 18+ is recommended. Dependencies are local (no global installs required).

## Storybook

```bash
cd FairWorkly/frontend
npm run storybook        # UI dev server (http://localhost:6006)
npm run build-storybook  # static Storybook build
```

## Project structure (high level)

- `src/main.tsx` - app bootstrap (Router, Query, Redux, Theme).
- `src/app/` - app shell and routing.
- `src/modules/` - feature modules (business domains).
- `src/shared/` - reusable UI + utilities.
- `src/services/` - API client layer.
- `src/store/`, `src/slices/` - global state (used sparingly).
- `src/assets/`, `public/`, `src/styles/` - assets and styles.

## Architecture & rules

For detailed structure rules and frontend constraints, refer to:

- `../docs/01-frontend-architecture.md`
- `../docs/03-dev-workflow-and-pr.md`

## Environment variables

Vite exposes env vars prefixed with `VITE_`. Use the backend appsettings as your source of truth for local values.

Start from:

```bash
cp .env.example .env
```

Recommended local defaults (aligned with `backend/src/FairWorkly.API/appsettings.Development.example.json`):

```env
VITE_API_BASE_URL=http://localhost:5680
VITE_APP_ENV=development
VITE_DEBUG=false
```

Notes:
- `VITE_API_BASE_URL` should match the backend base URL you run (local `dotnet run` or Docker).
- Keep secrets out of git; use `.env.local` if you want local-only overrides.

## Quality and testing

- ESLint is configured via `eslint.config.js` (`npm run lint`).
- Storybook lives under `.storybook/`. Vitest integration is configured in `vite.config.ts`, but there is no test script yet.
- Jest is installed but not wired to any project-level test scripts/configs.

## Documentation links

- Frontend architecture: `../docs/01-frontend-architecture.md`
- Dev workflow & PR rules: `../docs/03-dev-workflow-and-pr.md`

## Contributing workflow

Follow the repo workflow rules in `../docs/03-dev-workflow-and-pr.md`. Quick reminders:

1) Branch from `main`.
2) Update routes in `src/app/routes/*.routes.tsx` and keep sidebar nav config in sync (`src/shared/components/layout/app/Sidebar/config/navigation.config.tsx`).
3) Keep feature work inside `src/modules/*` and shared elements in `src/shared/`.
4) Run `npm run lint` before opening a PR; add tests or Storybook stories when introducing new UI behavior.
