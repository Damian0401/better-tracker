# Project Structure

```
src/
  routes/              # TanStack Router file-based routes (__root.tsx, index.tsx, about.tsx)
  pages/               # Page components (one folder per page, e.g. home/HomePage.tsx)
  components/          # Reusable UI components (organized by feature, e.g. layout/Navbar.tsx)
  libs/                # Utilities, API client, env validation
    api.ts             # Typed fetch client (openapi-fetch)
    api.schema.g.ts    # AUTO-GENERATED — do not edit
    env.ts             # Zod-validated env vars
    utils/             # Pure utility functions
  main.tsx             # Entry point, router setup
  styles.css           # Global styles
```

## Folder Responsibilities

### `src/routes/`
Thin route definitions using TanStack Router file-based routing. Each file maps to a URL path. Imports and delegates to page components.

### `src/pages/`
Page-level components. One folder per page (e.g. `home/HomePage.tsx`). Contains page-specific logic and layout composition.

### `src/components/`
Reusable UI components organized by feature/domain (e.g. `layout/Navbar.tsx`). Shared across pages.

### `src/libs/`
Utilities and infrastructure:
- `api.ts` — Typed API client instance
- `api.schema.g.ts` — Auto-generated TypeScript types from OpenAPI spec
- `env.ts` — Zod-validated environment variables
- `utils/` — Pure utility functions

### `src/main.tsx`
Application entry point. Sets up TanStack Router and renders root component.

## Naming Conventions

- **Components**: PascalCase (`HomePage`, `Navbar`)
- **Files**: PascalCase for components (`HomePage.tsx`), camelCase for utilities
- **Routes**: lowercase with hyphens if needed (`about.tsx`, `__root.tsx`)
- **Variables/functions**: camelCase
- **Constants/types**: PascalCase or UPPER_SNAKE_CASE for true constants
