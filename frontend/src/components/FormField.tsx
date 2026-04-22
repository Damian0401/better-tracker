import type { ReactNode } from "react";
import { cn } from "@/libs/utils";

interface FormFieldProps {
  label: ReactNode;
  children: ReactNode;
  className?: string;
  labelClassName?: string;
  labelRowClassName?: string;
}

export function FormField({ label, children, className, labelClassName, labelRowClassName }: FormFieldProps) {
  return (
    <div className={cn("space-y-2", className)}>
      <div className={cn("flex h-3 items-center", labelRowClassName)}>
        {typeof label === "string" ? <label className={cn("text-sm font-medium", labelClassName)}>{label}</label> : label}
      </div>
      {children}
    </div>
  );
}
