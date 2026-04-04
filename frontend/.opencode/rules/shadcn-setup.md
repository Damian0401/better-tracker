# Shadcn UI Setup (Vite + React + TypeScript + pnpm)

## 1. Create Project (if needed)

If you don’t already have a Vite project, create one:

```bash
pnpm create vite@latest
```

Choose:
- **Framework:** React
- **Variant:** TypeScript

If you already have a project, skip this step.

---

## 2. Install Tailwind CSS

If Tailwind is not installed yet:

```bash
pnpm add tailwindcss @tailwindcss/vite
```

### Configure CSS

Replace everything in:

```
src/index.css
```

with:

```css
@import "tailwindcss";
```

---

## 3. Configure Path Alias (`@/*`)

### Edit `tsconfig.json`

Add `baseUrl` and `paths`:

```json
{
  "files": [],
  "references": [
    { "path": "./tsconfig.app.json" },
    { "path": "./tsconfig.node.json" }
  ],
  "compilerOptions": {
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"]
    }
  }
}
```

---

### Edit `tsconfig.app.json`

Add the same alias:

```json
{
  "compilerOptions": {
    "baseUrl": ".",
    "paths": {
      "@/*": ["./src/*"]
    }
  }
}
```

---

## 4. Update Vite Config

### Install types

```bash
pnpm add -D @types/node
```

### Update `vite.config.ts`

```ts
import path from "path"
import tailwindcss from "@tailwindcss/vite"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
})
```

---

## 5. Initialize shadcn/ui

Run:

```bash
pnpm dlx shadcn@latest init
```

Follow the CLI prompts.

---

## 6. Add Components

Example: add a Button component

```bash
pnpm dlx shadcn@latest add button
```

---

## 7. Use Components

Example usage in `src/App.tsx`:

```tsx
import { Button } from "@/components/ui/button"

function App() {
  return (
    <div className="flex min-h-svh flex-col items-center justify-center">
      <Button>Click me</Button>
    </div>
  )
}

export default App
```
