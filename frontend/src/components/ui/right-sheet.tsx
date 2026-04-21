import * as DialogPrimitive from "@radix-ui/react-dialog";
import type { ReactNode } from "react";
import { cn } from "@/libs/utils/cn";

interface RightSheetProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  container: HTMLElement | null;
  children: ReactNode;
  widthClassName?: string;
}

export function RightSheet({
  open,
  onOpenChange,
  container,
  children,
  widthClassName,
}: RightSheetProps) {
  return (
    <DialogPrimitive.Root open={open} onOpenChange={onOpenChange}>
      <DialogPrimitive.Portal container={container ?? undefined}>
        <DialogPrimitive.Overlay className="absolute inset-0 z-40 bg-transparent" />
        <DialogPrimitive.Content
          style={{ backgroundColor: "hsl(var(--background))" }}
          className={cn(
            "absolute inset-y-0 right-0 z-50 h-full w-full max-w-2xl overflow-hidden border-l bg-background shadow-xl data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:slide-out-to-right data-[state=open]:slide-in-from-right",
            widthClassName
          )}
        >
          {children}
        </DialogPrimitive.Content>
      </DialogPrimitive.Portal>
    </DialogPrimitive.Root>
  );
}
