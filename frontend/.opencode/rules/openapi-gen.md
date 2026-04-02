# OpenAPI Client Generation

## Required Packages

| Package | Version | Purpose |
|---------|---------|---------|
| `openapi-fetch` | ^0.15.0 | Type-safe HTTP client |
| `openapi-typescript` | latest (via `pnpx`) | TypeScript type generation |
| `dotenv-cli` | latest (via `pnpx`) | Environment variable injection |

## File Structure

### Generated Schema (`src/libs/api.schema.g.ts`)

Auto-generated file containing TypeScript types derived from the OpenAPI specification. Do not edit manually.

### API Client (`src/libs/api.ts`)

```typescript
import createFetchClient from "openapi-fetch";
import type { paths } from "./api.schema.g";
import { Env } from "./env";

export const Api = createFetchClient<paths>({
  baseUrl: Env.VITE_API_URL
})
```

**Explanation:**

- Line 1: Imports the `openapi-fetch` client factory
- Line 2: Imports `paths` type from the generated schema (provides full API type safety)
- Line 3: Imports validated environment variables
- Line 5-6: Creates a typed API client with the base URL from environment

### Environment (`src/libs/env.ts`)

Must export `VITE_API_URL` validated via Zod. Example:

```typescript
import { z } from "zod";

const envSchema = z.object({
  VITE_API_URL: z.string().url(),
});

export const Env = envSchema.parse(import.meta.env);
```

## Generate Command

Add to `package.json` scripts:

```json
{
  "scripts": {
    "generate:api": "pnpx openapi-typescript@latest %VITE_API_URL%/openapi/v1.json -o ./src/libs/api.schema.g.ts",
    "generate": "pnpx dotenv-cli -v NODE_TLS_REJECT_UNAUTHORIZED=0 -- pnpm run generate:api"
  }
}
```

**Commands:**

- `generate:api` — Runs `openapi-typescript` against the OpenAPI JSON spec and outputs types to `api.schema.g.ts`
- `generate` — Loads `.env` variables via `dotenv-cli` then runs `generate:api`; disables TLS rejection for local/self-signed certs

## Usage

```typescript
import { Api } from "../../libs/api";

// GET request
const response = await Api.GET('/api/v1/todos');
if (!response.data) return;

// POST request
const created = await Api.POST('/api/v1/todos', {
  body: { title: "New task" }
});
```
