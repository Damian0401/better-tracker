import { Link, useRouterState } from "@tanstack/react-router"
import { cn } from "@/libs/utils/cn"

interface LayoutProps {
  children: React.ReactNode
}

export function Layout({ children }: LayoutProps) {
  const routerState = useRouterState()
  const currentPath = routerState.location.pathname

  const navItems = [
    { path: "/", label: "Home" },
    { path: "/notes", label: "Notes" },
  ]

  return (
    <div className="flex h-screen">
      {/* Left Sidebar */}
      <aside className="w-64 border-r bg-muted/40">
        <div className="flex h-full flex-col">
          <div className="border-b p-6">
            <h1 className="text-2xl font-bold">BetterTracker</h1>
          </div>
          <nav className="flex-1 space-y-1 p-4">
            {navItems.map((item) => (
              <Link
                key={item.path}
                to={item.path}
                className={cn(
                  "block rounded-lg px-3 py-2 text-sm font-medium transition-colors",
                  currentPath === item.path
                    ? "bg-primary text-primary-foreground"
                    : "hover:bg-muted"
                )}
              >
                {item.label}
              </Link>
            ))}
          </nav>
        </div>
      </aside>

      {/* Main Content */}
      <main className="flex-1 overflow-auto">
        {children}
      </main>
    </div>
  )
}
