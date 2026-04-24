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
  isReadOnly?: boolean;
  onCreate: () => void;
  onSearchChange: (value: string) => void;
  onStatusToggle: (value: string, checked: boolean) => void;
  onWorkTypeToggle: (value: string, checked: boolean) => void;
  onTagToggle: (value: string, checked: boolean) => void;
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
  isReadOnly = false,
  onCreate,
  onSearchChange,
  onStatusToggle,
  onWorkTypeToggle,
  onTagToggle,
  onSelect,
  onLoadMore,
}: JobApplicationsListPanelProps) {
  return (
    <div className="flex h-full flex-col">
      <div className="space-y-3 border-b p-4">
        {!isReadOnly ? (
          <Button onClick={onCreate} className="w-full">
            Create Application
          </Button>
        ) : null}

        <Input
          placeholder="Search applications"
          value={filters.search}
          onChange={(event) => onSearchChange(event.target.value)}
        />

        <div className="space-y-2">
          <details className="rounded-md border p-2">
            <summary className="cursor-pointer text-sm font-medium">
              Statuses {filters.statuses.length > 0 ? `(${filters.statuses.length})` : ""}
            </summary>
            <div className="mt-2 space-y-1">
              {dropdowns.jobApplicationStatuses.map((option) => {
                const value = option.value.toString();
                const isChecked = filters.statuses.includes(value);

                return (
                  <label key={value} className="flex cursor-pointer items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={isChecked}
                      onChange={(event) => onStatusToggle(value, event.target.checked)}
                    />
                    <span>{option.name}</span>
                  </label>
                );
              })}
            </div>
          </details>

          <details className="rounded-md border p-2">
            <summary className="cursor-pointer text-sm font-medium">
              Work Types {filters.workTypes.length > 0 ? `(${filters.workTypes.length})` : ""}
            </summary>
            <div className="mt-2 space-y-1">
              {dropdowns.workTypes.map((option) => {
                const value = option.value.toString();
                const isChecked = filters.workTypes.includes(value);

                return (
                  <label key={value} className="flex cursor-pointer items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={isChecked}
                      onChange={(event) => onWorkTypeToggle(value, event.target.checked)}
                    />
                    <span>{option.name}</span>
                  </label>
                );
              })}
            </div>
          </details>

          <details className="rounded-md border p-2">
            <summary className="cursor-pointer text-sm font-medium">
              Tags {filters.tags.length > 0 ? `(${filters.tags.length})` : ""}
            </summary>
            <div className="mt-2 space-y-1">
              {availableTags.map((tag) => {
                const isChecked = filters.tags.includes(tag.name);

                return (
                  <label key={tag.id} className="flex cursor-pointer items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={isChecked}
                      onChange={(event) => onTagToggle(tag.name, event.target.checked)}
                    />
                    <span>#{tag.name}</span>
                  </label>
                );
              })}
            </div>
          </details>
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
            <h3 className="truncate font-medium">{application.jobTitle}</h3>
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
