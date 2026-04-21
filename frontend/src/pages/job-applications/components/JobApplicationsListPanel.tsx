import type { components } from "@/libs/api.schema.g";
import { formatDate, getOptionLabel, getTagColorClass } from "@/libs/utils";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import type { Filters } from "./types";

type DropdownsResponse = components["schemas"]["GetJobApplicationDropdownsResponse"];
type ListItem = components["schemas"]["ListJobApplicationsItemDto"];
type ListMyTagsItemDto = components["schemas"]["ListMyTagsItemDto"];

interface JobApplicationsListPanelProps {
  applications: ListItem[];
  selectedApplicationId: string | null;
  filters: Filters;
  dropdowns: DropdownsResponse;
  availableTags: ListMyTagsItemDto[];
  total: number;
  isListLoading: boolean;
  isLoadingMore: boolean;
  onCreate: () => void;
  onSearchChange: (value: string) => void;
  onStatusChange: (value: string) => void;
  onWorkTypeChange: (value: string) => void;
  onTagChange: (value: string) => void;
  onSelect: (id: string) => void;
  onLoadMore: () => void;
}

export function JobApplicationsListPanel({
  applications,
  selectedApplicationId,
  filters,
  dropdowns,
  availableTags,
  total,
  isListLoading,
  isLoadingMore,
  onCreate,
  onSearchChange,
  onStatusChange,
  onWorkTypeChange,
  onTagChange,
  onSelect,
  onLoadMore,
}: JobApplicationsListPanelProps) {
  return (
    <div className="flex h-full flex-col">
      <div className="space-y-3 border-b p-4">
        <Button onClick={onCreate} className="w-full">
          Create Application
        </Button>

        <Input
          placeholder="Search applications"
          value={filters.search}
          onChange={(event) => onSearchChange(event.target.value)}
        />

        <div className="flex flex-wrap items-center gap-2">
          <select
            className="h-9 min-w-40 flex-1 basis-56 rounded-md border bg-background px-3 text-sm"
            value={filters.status}
            onChange={(event) => onStatusChange(event.target.value)}
          >
            <option value="">All statuses</option>
            {dropdowns.jobApplicationStatuses.map((option) => (
              <option key={option.value.toString()} value={option.value.toString()}>
                {option.name}
              </option>
            ))}
          </select>

          <select
            className="h-9 min-w-40 flex-1 basis-56 rounded-md border bg-background px-3 text-sm"
            value={filters.workType}
            onChange={(event) => onWorkTypeChange(event.target.value)}
          >
            <option value="">All work types</option>
            {dropdowns.workTypes.map((option) => (
              <option key={option.value.toString()} value={option.value.toString()}>
                {option.name}
              </option>
            ))}
          </select>

          <select
            className="h-9 min-w-40 flex-1 basis-56 rounded-md border bg-background px-3 text-sm"
            value={filters.tag}
            onChange={(event) => onTagChange(event.target.value)}
          >
            <option value="">All tags</option>
            {availableTags.map((tag) => (
              <option key={tag.id} value={tag.name}>
                #{tag.name}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="min-h-0 flex-1 overflow-y-auto">
        {isListLoading && applications.length === 0 ? (
          <div className="p-4 text-center text-muted-foreground">Loading applications...</div>
        ) : null}

        {applications.map((application) => (
          <div
            key={application.id}
            onClick={() => onSelect(application.id)}
            className={`cursor-pointer border-b p-4 transition-colors hover:bg-muted ${
              selectedApplicationId === application.id ? "bg-muted" : ""
            }`}
          >
            <h3 className="truncate font-medium">{application.title}</h3>
            <p className="mt-1 truncate text-sm text-muted-foreground">{application.companyName}</p>
            <p className="mt-1 text-xs text-muted-foreground">
              {getOptionLabel(dropdowns.jobApplicationStatuses, application.currentStatus)}
            </p>
            {application.tags.length > 0 ? (
              <div className="mt-2 flex flex-wrap gap-1">
                {application.tags.map((tag) => (
                  <span
                    key={`${application.id}-${tag}`}
                    className={`inline-flex items-center rounded-full border px-2 py-0.5 text-[11px] font-medium ${getTagColorClass(tag)}`}
                  >
                    #{tag}
                  </span>
                ))}
              </div>
            ) : null}
            <p className="mt-1 text-xs text-muted-foreground">{formatDate(application.updatedAt)}</p>
          </div>
        ))}

        {applications.length < total ? (
          <div className="p-4">
            <Button onClick={onLoadMore} variant="outline" className="w-full" disabled={isLoadingMore}>
              {isLoadingMore ? "Loading..." : "Load More"}
            </Button>
          </div>
        ) : null}

        {!isListLoading && applications.length === 0 ? (
          <div className="p-4 text-center text-muted-foreground">No job applications found.</div>
        ) : null}
      </div>
    </div>
  );
}
