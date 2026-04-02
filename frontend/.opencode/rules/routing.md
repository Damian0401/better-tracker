# Routing

## TanStack Router File-Based Routing

Routes are defined in `src/routes/` using TanStack Router's file-based routing system. Each file maps to a URL path.

## Route File Pattern

Routes are thin wrappers that import and delegate to page components:

```typescript
import { createFileRoute } from '@tanstack/react-router'
import { HomePage } from '../pages/home/HomePage'

export const Route = createFileRoute('/')({
  component: HomePage,
})
```

## Route Naming

- Root route: `__root.tsx`
- Index route: `index.tsx`
- Named routes: lowercase, hyphens if needed (`about.tsx`, `user-profile.tsx`)

## Page Components

Page components live in `src/pages/`, one folder per page:

```
src/pages/
  home/HomePage.tsx
  about/AboutPage.tsx
  user-profile/UserProfilePage.tsx
```

## Route Configuration

Routes can include additional options:

```typescript
export const Route = createFileRoute('/users')({
  component: UsersPage,
  loader: async () => {
    const response = await Api.GET('/api/v1/users');
    return response.data;
  },
})
```

## Best Practices

- Keep route files thin — only define route config and import page component
- Page components contain all page-specific logic and layout
- Use route loaders for data fetching when appropriate
- Nested routes use folder structure (e.g. `users/$userId.tsx`)
