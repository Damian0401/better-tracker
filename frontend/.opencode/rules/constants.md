# Constants

This project uses predefined constants to avoid magic strings throughout the codebase.

## Route Constants

All route paths should be defined in `src/constants/routes.ts`:

```typescript
export const Routes = {
  HOME: "/",
  LOGIN: "/login",
  REGISTER: "/register",
  NOTES: "/notes",
} as const;
```

Usage:
```typescript
import { Routes } from "@/constants";

// In TanStack Router
export const Route = createFileRoute(Routes.NOTES)({
  component: NotesPage,
});

// In navigation
router.navigate({ to: Routes.LOGIN });

// In Links
<Link to={Routes.REGISTER}>Register</Link>
```

## Storage Keys

All localStorage/sessionStorage keys should be defined in `src/constants/storage.ts`:

```typescript
export const StorageKeys = {
  TOKEN: "auth_token",
  USER_ID: "auth_user_id",
  USER_NAME: "auth_user_name",
} as const;
```

Usage:
```typescript
import { StorageKeys } from "@/constants";

localStorage.getItem(StorageKeys.TOKEN);
localStorage.setItem(StorageKeys.USER_ID, userId);
```

## Adding New Constants

1. Create or update the appropriate file in `src/constants/`
2. Export constants with `as const` for type safety
3. Export a union type for the constants when needed
4. Import and use the constants throughout the codebase instead of hardcoded strings
