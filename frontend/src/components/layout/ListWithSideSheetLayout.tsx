import { useState } from "react";
import type { ReactNode } from "react";
import { RightSheet } from "@/components/ui/right-sheet";
import { cn } from "@/libs/utils/cn";

interface ListWithSideSheetLayoutProps {
  leftPanel: ReactNode;
  sheetOpen: boolean;
  onSheetOpenChange: (open: boolean) => void;
  sheetContent: ReactNode;
  leftPanelClassName?: string;
  sheetWidthClassName?: string;
}

export function ListWithSideSheetLayout({
  leftPanel,
  sheetOpen,
  onSheetOpenChange,
  sheetContent,
  leftPanelClassName,
  sheetWidthClassName,
}: ListWithSideSheetLayoutProps) {
  const [container, setContainer] = useState<HTMLElement | null>(null);

  return (
    <div ref={setContainer} className="relative flex h-full overflow-hidden">
      <section className={cn("flex min-w-0 flex-1 flex-col", leftPanelClassName)}>{leftPanel}</section>
      <RightSheet
        open={sheetOpen}
        onOpenChange={onSheetOpenChange}
        container={container}
        widthClassName={sheetWidthClassName}
      >
        {sheetContent}
      </RightSheet>
    </div>
  );
}
