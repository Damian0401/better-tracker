# Frontend Agent Guidelines

This file contains essential information for AI coding agents working in this frontend codebase.

## Quick Reference

- **Tech Stack**: React 19 + TypeScript 5.9 + Vite + TanStack Router + Tailwind CSS
- **Package Manager**: npm (use `npm` commands, not pnpm/yarn)
- **Path Alias**: `@/*` maps to `./src/*`

## Build/Lint/Test Commands

```bash
# Development
npm run dev                    # Start dev server (Vite)

# Build
npm run build                  # Type-check with tsc and build for production
npm run preview                # Preview production build locally

# Linting
npm run lint                   # Run ESLint on all files

# API Client Generation
npm run generate               # Generate TypeScript types from OpenAPI spec
npm run generate:api           # Generate API types (requires VITE_API_URL env var)
```

**Note**: There is currently no test runner configured in this project.

## Code Style Overview

For complete code style guidelines, see `.opencode/rules/code-style.md`. Key points:

### Imports
- **Named exports only** — no default exports
- Group imports: external libraries first, then internal `../` / `./` imports
- Use `import type` for type-only imports (enforced by `verbatimModuleSyntax`)
- Double quotes for strings

```typescript
// Good
import { useState } from "react";
import type { FC } from "react";
import { Api } from "@/libs/api";
import { HomePage } from "../pages/home/HomePage";

// Bad
import React from "react";  // no default imports
import { useState } from 'react'  // single quotes
```

### Formatting
- 2-space indentation
- Semicolons required
- Arrow function components: `export const Component = () => (...)`
- Named function components also acceptable: `export function Component() { ... }`

### TypeScript
- `strict: true` — no `any`, proper null checks required
- `noUnusedLocals` and `noUnusedParameters` enforced
- `verbatimModuleSyntax` enforced — use `import type` for type-only imports
- `erasableSyntaxOnly` — no TypeScript-only runtime constructs (enums, namespaces)
- Use `type` aliases for schema-derived types:
  ```typescript
  type ToDo = components['schemas']['TodoDto']
  ```

### React Patterns
- **React Compiler enabled** — no manual `useMemo`/`useCallback` needed
- Functional components only (no class components)
- Inline styles via `style={{ }}` or CSS classes via `className`

### Error Handling
- Always check `response.data` before accessing (openapi-fetch pattern):
  ```typescript
  const response = await Api.GET('/api/v1/todos');
  if (!response.data) {
    // handle error
    return;
  }
  // safe to use response.data
  ```
- Use early returns for guard clauses
- Environment variables validated at startup via Zod (see `src/libs/env.ts`)

## Project Structure

See `.opencode/rules/project-structure.md` for details. Key folders:

```
src/
  routes/              # TanStack Router file-based routes
  pages/               # Page components (one folder per page)
  components/          # Reusable UI components
    ui/                # shadcn/ui components
  libs/                # Utilities, API client, env validation
    api.ts             # Typed fetch client
    api.schema.g.ts    # AUTO-GENERATED — do not edit
    env.ts             # Zod-validated env vars
  main.tsx             # Entry point
```

### Naming Conventions
- **Components**: PascalCase (`HomePage`, `Navbar`)
- **Component Files**: PascalCase (`HomePage.tsx`, `Navbar.tsx`)
- **Routes**: lowercase with hyphens (`about.tsx`, `user-profile.tsx`)
- **Utilities**: camelCase (`formatDate.ts`)
- **Variables/functions**: camelCase
- **Types/interfaces**: PascalCase
- **Constants**: UPPER_SNAKE_CASE for true constants

## Routing

See `.opencode/rules/routing.md` for details.

- Uses **TanStack Router** with file-based routing
- Routes live in `src/routes/` and map to URL paths
- Keep route files thin — import and delegate to page components in `src/pages/`

```typescript
// src/routes/index.tsx
import { createFileRoute } from '@tanstack/react-router'
import { HomePage } from '../pages/home/HomePage'

export const Route = createFileRoute('/')({
  component: HomePage,
})
```

## API Client

See `.opencode/rules/api-client.md` for details.

- Uses **openapi-fetch** for type-safe API calls
- All types auto-generated from OpenAPI spec
- Client instance: `Api` from `@/libs/api`

```typescript
import { Api } from "@/libs/api";

// GET request
const response = await Api.GET('/api/v1/todos');
if (!response.data) {
  // handle error
  return;
}
// response.data is fully typed

// POST request
await Api.POST('/api/v1/todos', {
  body: { title: "New task" }
});
```

### Regenerating API Types

Run after OpenAPI spec changes:
```bash
npm run generate
```

This fetches the OpenAPI spec from `VITE_API_URL` and regenerates `src/libs/api.schema.g.ts`.

## UI Components

See `.opencode/rules/shadcn-setup.md` for shadcn/ui setup details.

- Uses **shadcn/ui** components (Radix UI + Tailwind)
- Components live in `src/components/ui/`
- Add new components: `npx shadcn@latest add <component-name>`

```typescript
import { Button } from "@/components/ui/button"
```

## Environment Variables

- Validated at startup via Zod in `src/libs/env.ts`
- App fails fast if required variables are missing
- All env vars must be prefixed with `VITE_`

```typescript
// src/libs/env.ts
import { z } from "zod";

const envSchema = z.object({
  VITE_API_URL: z.string().url(),
});

export const Env = envSchema.parse(import.meta.env);
```

## Important Files to Never Edit

- `src/libs/api.schema.g.ts` — Auto-generated from OpenAPI spec
- Any file ending in `.g.ts` or `.generated.ts`

## Common Pitfalls

1. **Don't use default exports** — all exports must be named
2. **Don't manually memoize** — React Compiler handles this automatically
3. **Don't edit generated files** — regenerate via `npm run generate`
4. **Always check `response.data`** — openapi-fetch returns `{ data?, error? }`
5. **Use `import type`** — for type-only imports (enforced by compiler)
6. **Don't use enums** — `erasableSyntaxOnly` forbids them; use union types instead

## Additional Documentation

For more details, see the `.opencode/rules/` directory:
- `tech-stack.md` — Full technology stack overview
- `code-style.md` — Complete code style guidelines
- `project-structure.md` — Detailed folder structure and responsibilities
- `routing.md` — TanStack Router usage and patterns
- `api-client.md` — API client usage and error handling
- `openapi-gen.md` — OpenAPI type generation workflow
- `shadcn-setup.md` — shadcn/ui setup and component management

**Always consult these files before implementing new features or making architectural decisions.**