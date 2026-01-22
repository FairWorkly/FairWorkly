# Shared Module

## Purpose

The `src/shared` module contains cross-feature building blocks: reusable components, hooks, constants, types, styles, and utilities used by multiple feature modules.

## Structure

- `components/` - shared UI elements and layout primitives.
  - `layout/` (Topbar, Sidebar, MainLayout)
  - `ui/`, `feedback/`, `guards/` (ProtectedRoute, RoleBasedRoute, PermissionGate, ModuleGate), `storybook/`
- `constants/` - centralized strings, routes, layout tokens, and config (ex: `navigation.constants.ts`).
- `hooks/` - reusable hooks that are not feature-specific (ex: `usePermissions`, `useApiQuery`).
- `styles/` - theme and shared styling helpers.
- `types/` - shared TypeScript contracts (ex: `permissions.types.ts`, `api.types.ts`).
- `utils/` - pure helper functions.

## Usage Guidelines

- Put feature-specific logic inside `src/modules/*`; only move code into `shared` if it is used across multiple modules.
- Keep shared components small and focused; prefer composition over large, multi-purpose components.
- Centralize strings and numbers in `constants/` to avoid magic literals.
- Export interfaces for shared data shapes in `types/` and reuse them across modules.
- When adding a new shared component, place it in the most specific subfolder and keep naming consistent with existing files.

## Navigation Notes

- Navigation routes and labels live in `constants/navigation.constants.ts` and are consumed by `components/layout/Topbar` and `components/layout/Sidebar`.
- Navigation items use `requiredModule` to gate visibility based on user permissions.
- If you add or rename routes, update the constants first, then adjust any route configs.

## Permission System

- `PermissionProvider` (in `app/providers/`) fetches and caches user permissions via React Query.
- `usePermissions` hook provides `hasPermission()`, `canAccessModule()`, and permission state.
- UI gating components: `PermissionGate` (for specific permissions), `ModuleGate` (for module access).
- Types: `Role`, `Permission`, `ModuleId` defined in `types/permissions.types.ts`.
- Dev mode: Switch roles via `window.switchRole('admin')` or `window.switchRole('manager')`.
