# Repository Guidelines

## Project Structure & Module Organization
- `src/` holds all TypeScript sources. `main.tsx` bootstraps Vite/React, `App.tsx` wires global providers (React Router, Redux Toolkit, React Query), and `assets/` stores static modules shared across screens.
- Styles live beside components (`App.css`, `index.css`). Co-locate feature-specific hooks/components under folders such as `src/dashboard/` or `src/features/auth/` to keep imports predictable.
- `public/` contains static files copied verbatim into the build, while `vite.config.ts`, `tsconfig*.json`, and `eslint.config.js` define tooling in the repo root.

## Build, Test, and Development Commands
- `npm install` installs dependencies; run once when onboarding or when `package-lock.json` changes.
- `npm run dev` starts the Vite dev server with HMR at `http://localhost:5173`.
- `npm run build` runs `tsc -b` for type-checking and `vite build` to emit production assets in `dist/`.
- `npm run preview` serves the production build locally; use it before opening a PR.
- `npm run lint` executes ESLint across the workspace; add `--fix` to auto-resolve stylistic issues.

## Coding Style & Naming Conventions
- Follow the repository Prettier config (`.prettierrc`) and keep indentation at two spaces, single quotes for strings, and semicolons disabled.
- Prefer functional React components in `.tsx` files, PascalCase for components (`UserMenu.tsx`), camelCase for hooks/utilities (`useAuthGate.ts`), and SCREAMING_SNAKE_CASE only for constants exported from `config/` modules.
- Centralize Redux slices and RTK Query services under `src/store/` (create if absent) and name slices `somethingSlice.ts`. Mirror feature folders and avoid deeply nested relative imports—use Vite path aliases when structure grows.

## Testing Guidelines
- Jest is available for unit tests; create specs under `src/__tests__/` or next to the component using the `*.test.tsx` suffix.
- Write tests for reducers, hooks, and UI behavior with React Testing Library (install if needed). Mock React Query fetches via MSW or `vi.fn` and assert loading/error states.
- Maintain meaningful coverage for shared utilities and asynchronous flows; run `npx jest --coverage` before merging larger refactors.

## Commit & Pull Request Guidelines
- Follow the existing Conventional Commits-inspired style (`chore: add .DS_Store to gitignore`, `feat: user auth flow`). Keep summaries under 60 characters and describe scope when useful.
- Each PR should describe motivation, high-level changes, testing evidence (commands + screenshots for UI), and link to tracked issues. Include screenshots or Loom demos for visual updates and note any new environment variables or migrations required.
