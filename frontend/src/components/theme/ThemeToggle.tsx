import { useTheme } from "next-themes";

import { Button } from "@/components/ui/button";
import { ThemeToggleIcon } from "@/components/theme/ThemeToggleIcon";

export function ThemeToggle() {
  const { theme, setTheme } = useTheme();

  const nextTheme = theme === "dark" ? "light" : "dark";
  const isDark = theme === "dark";

  return (
    <Button
      variant="outline"
      size="icon"
      onClick={() => setTheme(nextTheme)}
      aria-label={`Switch to ${nextTheme} theme`}
    >
      <ThemeToggleIcon isDark={isDark} />
      <span className="sr-only">Switch to {nextTheme} theme</span>
    </Button>
  );
}
