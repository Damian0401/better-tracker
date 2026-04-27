import * as React from "react";
import * as RechartsPrimitive from "recharts";
import { cn } from "@/libs/utils";

export type ChartConfig = {
  [key: string]: {
    label?: React.ReactNode;
    color?: string;
  };
};

interface ChartContextValue {
  config: ChartConfig;
}

const ChartContext = React.createContext<ChartContextValue | null>(null);

const useChart = () => {
  const context = React.useContext(ChartContext);

  if (!context) {
    throw new Error("useChart must be used within a <ChartContainer />");
  }

  return context;
};

interface ChartContainerProps extends React.HTMLAttributes<HTMLDivElement> {
  config: ChartConfig;
  children: React.ComponentProps<typeof RechartsPrimitive.ResponsiveContainer>["children"];
}

export const ChartContainer = React.forwardRef<HTMLDivElement, ChartContainerProps>(
  ({ id, className, children, config, ...props }, ref) => {
    const chartId = React.useId();
    const finalId = `chart-${id ?? chartId.replace(/:/g, "")}`;

    const cssVariables = Object.entries(config).reduce<Record<string, string>>((acc, [key, value]) => {
      if (value.color) {
        acc[`--color-${key}`] = value.color;
      }

      return acc;
    }, {});

    return (
      <ChartContext.Provider value={{ config }}>
        <div
          data-slot="chart"
          data-chart={finalId}
          ref={ref}
          className={cn(
            "flex aspect-video justify-center text-xs [&_.recharts-cartesian-axis-tick_text]:fill-muted-foreground [&_.recharts-cartesian-grid_line]:stroke-border/50 [&_.recharts-tooltip-cursor]:stroke-border [&_.recharts-layer]:outline-none",
            className,
          )}
          style={cssVariables as React.CSSProperties}
          {...props}
        >
          <RechartsPrimitive.ResponsiveContainer>
            {children}
          </RechartsPrimitive.ResponsiveContainer>
        </div>
      </ChartContext.Provider>
    );
  },
);

ChartContainer.displayName = "ChartContainer";

// eslint-disable-next-line react-refresh/only-export-components
export const ChartTooltip = RechartsPrimitive.Tooltip;

interface ChartTooltipPayloadItem {
  dataKey?: string | number;
  color?: string;
  value?: string | number;
}

interface ChartTooltipContentProps {
  active?: boolean;
  payload?: ChartTooltipPayloadItem[];
  label?: React.ReactNode;
  hideLabel?: boolean;
}

export const ChartTooltipContent = React.forwardRef<
  HTMLDivElement,
  ChartTooltipContentProps
>(({ active, payload, label, hideLabel = false }, ref) => {
  const { config } = useChart();

  if (!active || !payload?.length) {
    return null;
  }

  return (
    <div
      ref={ref}
      className="grid min-w-36 items-start gap-1.5 rounded-md border bg-background px-3 py-2 text-xs shadow-lg"
    >
      {!hideLabel ? <div className="font-medium text-foreground">{label}</div> : null}
      <div className="grid gap-1">
        {payload.map((item) => {
          const key = String(item.dataKey ?? "value");
          const itemConfig = config[key];

          return (
            <div key={key} className="flex items-center justify-between gap-3">
              <div className="flex items-center gap-2 text-muted-foreground">
                <span
                  className="size-2 rounded-[2px]"
                  style={{ backgroundColor: item.color ?? "currentColor" }}
                />
                <span>{itemConfig?.label ?? key}</span>
              </div>
              <span className="font-mono font-medium text-foreground">{item.value}</span>
            </div>
          );
        })}
      </div>
    </div>
  );
});

ChartTooltipContent.displayName = "ChartTooltipContent";
