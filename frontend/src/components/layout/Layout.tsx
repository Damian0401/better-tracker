import { Link, useRouter, useRouterState } from "@tanstack/react-router";
import { cn } from "@/libs/utils/cn";
import { Auth } from "@/libs/auth";
import { Button } from "@/components/ui/button";
import { Routes } from "@/constants";

interface LayoutProps {
  children: React.ReactNode;
}

export function Layout({ children }: LayoutProps) {
  const router = useRouter();
  const routerState = useRouterState();
  const currentPath = routerState.location.pathname;
  const isAuthenticated = Auth.isAuthenticated();
  const user = Auth.getUser();

  const navItems = [
    { path: Routes.HOME, label: "Home" },
    { path: Routes.JOB_APPLICATIONS, label: "Applications" },
    { path: Routes.NOTES, label: "Notes" },
  ];

  const handleLogout = () => {
    Auth.removeToken();
    router.navigate({ to: Routes.LOGIN });
  };

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
      <main className="flex flex-col flex-1 overflow-hidden">
        <div className="flex shrink-0 items-center justify-end gap-4 p-4">
          {isAuthenticated && user && (
            <span className="text-sm text-muted-foreground">{user.userName}</span>
          )}
          {isAuthenticated && (
            <Button variant="outline" onClick={handleLogout}>
              Logout
            </Button>
          )}
        </div>
        <div className="flex-1 overflow-hidden">
          {children}
        </div>
      </main>
    </div>
  );
}
