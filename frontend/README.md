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

## Project structure

- `src/main.tsx` - Vite entry; mounts BrowserRouter, React Query, Redux, and MUI Theme providers.
- `src/app/App.tsx` - route assembly via `useRoutes`.
- `src/app/routes/*.routes.tsx` - route arrays; `src/app/routes/index.tsx` includes `MainLayout` if you choose to use `AppRoutes`.
- `src/app/providers/` - Redux and MUI theme providers.
- `src/modules/*` - feature modules with `pages/`, `features/`, `hooks/`, `types/`, `ui/`.
- `src/shared/components/` - reusable UI (`ui/`, `feedback/`, `layout/`, `guards/`, `storybook/`).
- `src/shared/constants/`, `src/shared/types/`, `src/shared/utils/`, `src/shared/hooks/` - shared contracts and helpers (see `src/shared/README.md`).
- `src/services/` - axios client and API modules.
- `src/store/`, `src/slices/` - Redux store and slices (auth/ui/notifications scaffolds).
- `src/assets/`, `public/` - static assets.
- `src/index.css`, `src/styles/` - global styling entries.

## Routing and navigation

- Route configs live in `src/app/routes/*.routes.tsx`.
- Navigation labels and routes are centralized in `src/shared/constants/navigation.constants.ts` and consumed by `Topbar`/`Sidebar`.
- `ProtectedRoute` is stubbed to unauthenticated, so tool routes redirect to `/login` until auth is wired in `src/shared/components/guards/ProtectedRoute.tsx`.
- Keep `NAV_ROUTES` aligned with route configs to avoid dead links while modules are scaffolded.
- `FairBotChat` currently renders the shared `Sidebar` directly for a three-column layout; reconcile with `MainLayout` if you later wrap routes.
- FairBot layout widths are set in `src/modules/fairbot/constants/fairbot.constants.ts` (`GRID_TEMPLATE_COLUMNS`, `SIDEBAR_COLUMN_WIDTH`).
- Import alias: `@/` maps to `src/` (see `vite.config.ts` and `tsconfig.app.json`).

## State, data, and API

- React Query is wired in `src/main.tsx` and available through `useApiQuery`/`useApiMutation` in `src/shared/hooks/`.
- Redux store is configured in `src/store/` with slices under `src/slices/`.
- API modules live in `src/services/` and use the shared axios client in `src/services/httpClient.ts`.

## Styling and UI

- MUI v7 + Emotion are used for layout and components.
- Theme is defined in `src/shared/styles/theme.ts` and applied via `ThemeProvider`.
- Keep global styles minimal; prefer component-scoped styling with MUI/Emotion.

## Environment variables

Vite exposes env vars prefixed with `VITE_`. Examples:

```env
VITE_API_BASE_URL=https://api.example.com
VITE_AUTH_AUDIENCE=...
```

Create a `.env` or `.env.local` under `FairWorkly/frontend/` as needed; do not commit secrets.

## Quality and testing

- ESLint is configured via `eslint.config.js` (`npm run lint`).
- Storybook is configured under `.storybook/`. Vitest integration is set in `vite.config.ts` for storybook tests.
- Jest is listed in dev dependencies, but no project-level test scripts/configs exist yet.

## Documentation links

- Product/context: `../docs/PRD.md`, `../docs/TDD.md`, `../docs/erd.md`
- Repo conventions: `../../AGENTS.md`

## Contributing workflow

1) Branch from `main`.
2) Add or adjust routes in `src/app/routes/*.routes.tsx` and update navigation constants if needed.
3) Keep feature work inside `src/modules/*` and shared elements in `src/shared/`.
4) Run `npm run lint` before opening a PR; add tests or Storybook stories when introducing new UI behavior.
