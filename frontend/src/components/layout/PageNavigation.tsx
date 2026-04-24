import { Link, useRouterState } from "@tanstack/react-router";
import { Routes } from "@/constants";
import { cn } from "@/libs/utils/cn";

const navItems = [
  { path: Routes.HOME, label: "Home" },
  { path: Routes.JOB_APPLICATIONS, label: "Applications" },
  { path: Routes.NOTES, label: "Notes" },
];

export function PageNavigation() {
  const routerState = useRouterState();
  const currentPath = routerState.location.pathname;

  return (
    <>
      <aside className="hidden w-64 border-r bg-muted/40 md:block">
        <div className="flex h-full flex-col">
          <div className="border-b p-6">
            <Link to={Routes.HOME} className="text-2xl font-bold">
              BetterTracker
            </Link>
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

      <div className="border-b bg-muted/40 md:hidden">
        <div className="flex items-center gap-3 px-4 py-3">
          <Link to={Routes.HOME} className="shrink-0 text-lg font-bold">
            BetterTracker
          </Link>
          <nav className="flex min-w-0 gap-2 overflow-x-auto">
          {navItems.map((item) => (
            <Link
              key={item.path}
              to={item.path}
              className={cn(
                "inline-flex whitespace-nowrap rounded-md px-3 py-2 text-sm font-medium transition-colors",
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
      </div>
    </>
  );
}
