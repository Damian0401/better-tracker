# Tech Stack

## Core

- **React 19** with React Compiler (automatic memoization)
- **TypeScript 5.9** (strict mode, ES2022 target)
- **Vite 7** as bundler
- **TanStack Router** for file-based routing
- **openapi-fetch** for typed API calls
- **Zod v4** for runtime validation (env vars)
- **pnpm** as package manager

## Key Characteristics

- React Compiler removes need for manual `useMemo`/`useCallback`
- File-based routing via TanStack Router (`src/routes/`)
- Type-safe API client generated from OpenAPI spec
- Environment variables validated at startup via Zod
