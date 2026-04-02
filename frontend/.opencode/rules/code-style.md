# Code Style

## Imports

- Use named exports for components and functions (no default exports)
- Group imports: external libraries first, then internal `../` / `./` imports
- Use `import type` for type-only imports (enforced by `verbatimModuleSyntax`)
- Double quotes for strings

## Formatting

- 2-space indentation (enforced by ESLint)
- Semicolons at end of statements
- Arrow function components: `export const Component = () => (...)`
- Named function components also acceptable: `export function Component() { ... }`

## TypeScript

- `strict: true` — no `any`, proper null checks required
- `noUnusedLocals` and `noUnusedParameters` enforced — remove dead code
- `erasableSyntaxOnly` — no TypeScript-only runtime constructs
- Use `type` aliases for schema-derived types (e.g. `type ToDo = components['schemas']['Dto']`)
- Generated schema files (`*.g.ts`) are auto-generated — never edit manually

## React Patterns

- Use React Compiler — no manual `useMemo`/`useCallback` needed
- Functional components only (no class components)
- Inline styles via `style={{ }}` object or CSS classes via `className`
- Keep routes thin — delegate to page components in `src/pages/`

## Error Handling

- API responses: check `response.data` before accessing (openapi-fetch pattern)
- Env vars: validated at startup via Zod — app fails fast if missing
- Use early returns for guard clauses

## Environment Validation

Env vars are validated at startup via Zod in `libs/env.ts`. The app fails fast if required variables are missing.

```typescript
import { z } from "zod";

const envSchema = z.object({
  VITE_API_URL: z.string().url(),
});

export const Env = envSchema.parse(import.meta.env);
```

Usage:

```typescript
import { Env } from "./libs/env";

const url = Env.VITE_API_URL; // type-safe, guaranteed to be valid
```
