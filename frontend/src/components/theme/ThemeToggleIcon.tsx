import { MoonIcon } from "@/components/icons/MoonIcon";
import { SunIcon } from "@/components/icons/SunIcon";

interface ThemeToggleIconProps {
  isDark: boolean;
}

export function ThemeToggleIcon({ isDark }: ThemeToggleIconProps) {
  if (isDark) {
    return <SunIcon aria-hidden="true" className="size-4" />;
  }

  return <MoonIcon aria-hidden="true" className="size-4" />;
}
