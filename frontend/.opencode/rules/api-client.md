# API Client

## openapi-fetch Usage

The API client is a typed wrapper around `fetch` using types generated from an OpenAPI spec.

## Client Setup

```typescript
import createFetchClient from "openapi-fetch";
import type { paths } from "./api.schema.g";
import { Env } from "./env";

export const Api = createFetchClient<paths>({
  baseUrl: Env.VITE_API_URL
})
```

## Usage

### GET Request

```typescript
import { Api } from "../../libs/api";

const response = await Api.GET('/api/v1/todos');
if (!response.data) return;
// response.data is fully typed
```

### POST Request

```typescript
const created = await Api.POST('/api/v1/todos', {
  body: { title: "New task" }
});
```

### Other Methods

```typescript
await Api.PUT('/api/v1/todos/1', { body: { title: "Updated" } });
await Api.PATCH('/api/v1/todos/1', { body: { completed: true } });
await Api.DELETE('/api/v1/todos/1');
```

## Response Handling

Always check for `response.data` before accessing:

```typescript
const response = await Api.GET('/api/v1/todos');
if (!response.data) {
  // handle error — response.error contains error details
  return;
}
```

## Type Safety

All request/response types are inferred from the OpenAPI spec:
- Path parameters are required and typed
- Request bodies are validated against schema
- Response data is fully typed
- Query parameters are typed with optional/required markers
