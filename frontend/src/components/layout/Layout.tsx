import type { ReactNode } from "react";
import { useRouter } from "@tanstack/react-router";
import { Auth } from "@/libs/auth";
import { Button } from "@/components/ui/button";
import { ThemeToggle } from "@/components/theme/ThemeToggle";
import { UserIcon } from "@/components/icons/UserIcon";
import { PageNavigation } from "@/components/layout/PageNavigation";
import { Routes } from "@/constants";

interface LayoutProps {
  children: ReactNode;
}

export function Layout({ children }: LayoutProps) {
  const router = useRouter();
  const isAuthenticated = Auth.isAuthenticated();
  const user = Auth.getUser();

  const handleLogout = () => {
    Auth.removeToken();
    router.navigate({ to: Routes.LOGIN });
  };

  return (
    <div className="flex h-screen flex-col md:flex-row">
      <PageNavigation />

      <main className="flex flex-col flex-1 overflow-hidden">
        <div className="flex shrink-0 items-center justify-end gap-3 p-4">
          {isAuthenticated && user && (
            <div className="flex items-center gap-2 rounded-md border bg-card px-3 py-1.5 text-sm font-medium text-foreground shadow-sm">
              <UserIcon aria-hidden="true" className="h-4 w-4 text-muted-foreground" />
              {user.userName}
            </div>
          )}
          <ThemeToggle />
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
