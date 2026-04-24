import type { ComponentProps } from "react";

export function MoonIcon(props: ComponentProps<"svg">) {
  return (
    <svg
      viewBox="0 0 24 24"
      fill="none"
      stroke="currentColor"
      strokeWidth="2"
      strokeLinecap="round"
      strokeLinejoin="round"
      {...props}
    >
      <path d="M12 3a7 7 0 1 0 9 9 9 9 0 1 1-9-9Z" />
    </svg>
  );
}
