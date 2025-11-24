# FairWorkly Frontend

React 18 + TypeScript + Vite SPA for FairWorkly, an AI-powered HR compliance copilot for Australian SMEs. This frontend will surface the four core agents outlined in the PRD (Compliance, Document & Contract, Payroll & STP Check, Employee Self-Service).

> Repo status: scaffolded. Routing, state, and data layers are present as placeholders and need to be wired up to the API once backend contracts are ready.

## Quick start

```bash
cd frontend
npm install
npm run dev       # start Vite dev server (http://localhost:5173)
npm run build     # type-check + production build to dist/
npm run preview   # serve the production build locally
npm run lint      # ESLint across the workspace
```

Node 18+ is recommended. Dependencies are local (no global installs required).

## Project structure

- `src/main.tsx` — Vite entry; wraps the app.
- `src/App.tsx` — root component; add providers (router, Redux Toolkit, React Query) here.
- `src/index.css`, `src/App.css` — global styles; Emotion + MUI are available for component styling.
- `src/components/`, `src/pages/` — shared and page-level React components.
- `src/routes/` — central place for React Router route definitions (empty scaffold).
- `src/services/` — API client modules; align with backend/OpenAPI once available.
- `src/store/` — Redux Toolkit slices/store setup (empty scaffold).
- `src/constants/`, `src/types/`, `src/utils/` — shared definitions and helpers.
- `public/` — static assets copied verbatim to the build.
- `dist/` — production output after `npm run build` (checked in for reference; regenerate before release).

## Styling and UI

- MUI v7 with Emotion (`@mui/material`, `@emotion/react`, `@emotion/styled`) is available for layout and theming.
- Keep lightweight globals in `index.css`/`App.css`; prefer component-scoped styling via Emotion/MUI system props.

## State and data

- `@reduxjs/toolkit` and `react-redux` are installed for app state; initialize the store in `src/store/` and wrap `App` with `<Provider>`.
- `@tanstack/react-query` is available for server state/fetching; add a `QueryClientProvider` in `App` once API endpoints are defined.
- `react-router-dom` is installed; define routes under `src/routes/` and mount `<Router>` in `App`.

## Environment variables

Vite exposes env vars prefixed with `VITE_`. Common examples you may introduce:

```
VITE_API_BASE_URL=https://api.example.com
VITE_AUTH_AUDIENCE=...
```

Create a `.env` or `.env.local` in `frontend/` as needed; do not commit secrets.

## Quality and testing

- ESLint is configured via `eslint.config.js`. Run `npm run lint` before pushing.
- Jest is listed as a dev dependency; a project-level config and tests are not yet present. Add tests under `src/__tests__/` or alongside components when implementing features.

## Documentation links

- Product/context: `../docs/PRD.md`, `../docs/TDD.md`, `../docs/erd.md`
- Repo conventions: `AGENTS.md` in this directory

## Contributing workflow

1) Branch from `main`.
2) Implement feature slices with routing/state/data wiring in `src/routes/` and `src/store/`.
3) Add or update tests (and Jest config) as functionality appears.
4) Run `npm run lint` and, when available, the test suite before raising a PR.

Future TODOs:

- Wire providers (Router, Redux, React Query) in `App.tsx`.
- Implement initial navigation shell and layouts for the four agents.
- Add API client modules once OpenAPI specs/contracts are defined.
