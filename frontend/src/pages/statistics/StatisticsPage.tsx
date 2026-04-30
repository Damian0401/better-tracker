import { useEffect, useMemo, useState } from "react";
import { toast } from "sonner";
import { FormField } from "@/components/FormField";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Api } from "@/libs/api";
import type { components } from "@/libs/api.schema.g";
import { Bar, BarChart, CartesianGrid, Cell, XAxis, YAxis } from "recharts";
import { ChartContainer, ChartTooltip, ChartTooltipContent, type ChartConfig } from "@/components/ui/chart";

type DropdownsResponse = components["schemas"]["GetJobApplicationDropdownsResponse"];
type StatisticsResponse = components["schemas"]["GetJobApplicationStatisticsResponse"];
type StatisticsFilters = {
  dateFrom: string;
  dateTo: string;
  includeArchived: boolean;
};

type ChartDataItem = {
  name: string;
  count: number;
  fill: string;
  isTotal?: boolean;
};

const DEFAULT_FILTERS: StatisticsFilters = {
  dateFrom: "",
  dateTo: "",
  includeArchived: false,
};

const chartConfig = {
  count: {
    label: "Applications",
    color: "hsl(var(--primary))",
  },
} satisfies ChartConfig;

const REGULAR_BAR_COLORS = [
  "hsl(var(--primary))",
  "hsl(var(--accent-foreground))",
  "hsl(var(--muted-foreground))",
  "hsl(var(--ring))",
];

const TOTAL_BAR_COLOR = "hsl(var(--destructive))";

const toNumber = (value: number | string | undefined) => {
  if (typeof value === "number") {
    return value;
  }

  return Number(value ?? 0);
};

export function StatisticsPage() {
  const [dropdowns, setDropdowns] = useState<DropdownsResponse>({
    workTypes: [],
    salaryTypes: [],
    jobApplicationStatuses: [],
  });
  const [statistics, setStatistics] = useState<StatisticsResponse | null>(null);
  const [filters, setFilters] = useState<StatisticsFilters>(DEFAULT_FILTERS);
  const [isLoading, setIsLoading] = useState(false);

  const statusNameByValue = useMemo(
    () =>
      new Map(
        dropdowns.jobApplicationStatuses.map((status) => [String(status.value), status.name]),
      ),
    [dropdowns.jobApplicationStatuses],
  );

  const chartData = useMemo<ChartDataItem[]>(() => {
    if (!statistics) {
      return [];
    }

    const statusBars = statistics.statusCounts.map((statusCount, index) => {
      const statusKey = String(statusCount.status);
      const statusName = statusNameByValue.get(statusKey) ?? `Status ${statusKey}`;

      return {
        name: statusName,
        count: toNumber(statusCount.count),
        fill: REGULAR_BAR_COLORS[index % REGULAR_BAR_COLORS.length],
      };
    });

    return [
      ...statusBars,
      {
        name: "Total",
        count: toNumber(statistics.total),
        fill: TOTAL_BAR_COLOR,
        isTotal: true,
      },
    ];
  }, [statistics, statusNameByValue]);

  useEffect(() => {
    const loadStatistics = async () => {
      setIsLoading(true);

      const query: {
        DateFrom?: string;
        DateTo?: string;
        IncludeArchived?: boolean;
      } = {
        IncludeArchived: filters.includeArchived,
      };

      if (filters.dateFrom) {
        query.DateFrom = filters.dateFrom;
      }

      if (filters.dateTo) {
        query.DateTo = filters.dateTo;
      }

      if (filters.dateFrom && filters.dateTo && filters.dateFrom > filters.dateTo) {
        toast.error("Date from cannot be after date to");
        setIsLoading(false);
        return;
      }

      try {
        const [dropdownsResponse, statisticsResponse] = await Promise.all([
          Api.GET("/api/v1/job-applications/dropdowns"),
          Api.GET("/api/v1/job-applications/statistics", {
            params: {
              query,
            },
          }),
        ]);

        if (!dropdownsResponse.data) {
          toast.error("Failed to load status filters");
          return;
        }

        if (!statisticsResponse.data) {
          toast.error("Failed to load statistics");
          return;
        }

        setDropdowns(dropdownsResponse.data);
        setStatistics(statisticsResponse.data);
      } finally {
        setIsLoading(false);
      }
    };

    void loadStatistics();
  }, [filters]);

  return (
    <div className="h-full overflow-y-auto p-4 md:p-6">
      <div className="mx-auto flex w-full max-w-6xl flex-col gap-6">
        <Card>
          <CardHeader>
            <CardTitle>Job Application Statistics</CardTitle>
            <CardDescription>
              Review status distribution and total applications for selected filters.
            </CardDescription>
          </CardHeader>
          <CardContent className="grid gap-4 md:grid-cols-3">
            <FormField label="Date from">
              <Input
                type="date"
                value={filters.dateFrom}
                onChange={(event) =>
                  setFilters((prev) => ({
                    ...prev,
                    dateFrom: event.target.value,
                  }))
                }
              />
            </FormField>
            <FormField label="Date to">
              <Input
                type="date"
                value={filters.dateTo}
                onChange={(event) =>
                  setFilters((prev) => ({
                    ...prev,
                    dateTo: event.target.value,
                  }))
                }
              />
            </FormField>
            <div className="flex items-end gap-4">
              <div className="flex h-9 items-center gap-2 rounded-md border border-input px-3">
                <Checkbox
                  id="include-archived"
                  className="h-4 w-4 border-primary"
                  checked={filters.includeArchived}
                  onCheckedChange={(checked) =>
                    setFilters((prev) => ({
                      ...prev,
                      includeArchived: checked === true,
                    }))
                  }
                />
                <label
                  htmlFor="include-archived"
                  className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                >
                  Include archived
                </label>
              </div>
              <Button
                variant="default"
                onClick={() => setFilters(DEFAULT_FILTERS)}
              >
                Reset
              </Button>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Status distribution</CardTitle>
            <CardDescription>
              Total bar is always shown last and highlighted.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <div className="flex h-80 items-center justify-center text-sm text-muted-foreground">
                Loading statistics...
              </div>
            ) : chartData.length === 0 ? (
              <div className="flex h-80 items-center justify-center text-sm text-muted-foreground">
                No data for selected filters.
              </div>
            ) : (
              <ChartContainer config={chartConfig} className="h-80 w-full">
                <BarChart data={chartData} margin={{ top: 8, right: 16, left: 8, bottom: 8 }}>
                  <CartesianGrid vertical={false} />
                  <XAxis dataKey="name" tickLine={false} axisLine={false} interval={0} />
                  <YAxis allowDecimals={false} tickLine={false} axisLine={false} width={28} />
                  <ChartTooltip content={<ChartTooltipContent hideLabel />} cursor={false} />
                  <Bar dataKey="count" radius={[8, 8, 0, 0]}>
                    {chartData.map((item, index) => (
                      <Cell key={`${item.name}-${index}`} fill={item.fill} />
                    ))}
                  </Bar>
                </BarChart>
              </ChartContainer>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}