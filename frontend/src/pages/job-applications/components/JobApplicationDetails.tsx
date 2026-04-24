import { useState } from "react";
import * as DialogPrimitive from "@radix-ui/react-dialog";
import type { components } from "@/libs/api.schema.g";
import { getTagColorClass } from "@/libs/utils";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Separator } from "@/components/ui/separator";
import { Textarea } from "@/components/ui/textarea";
import { ClockIcon } from "@/components/icons/ClockIcon";
import { FormField } from "@/components/FormField";
import { JobApplicationCommentsSection } from "./JobApplicationCommentsSection";
import { JobApplicationSalaryFields } from "./JobApplicationSalaryFields";

type DropdownsResponse = components["schemas"]["GetJobApplicationDropdownsResponse"];
type UpdateRequest = components["schemas"]["UpdateJobApplicationBody"];
type CommentDto = components["schemas"]["GetJobApplicationByIdCommentDto"];
type StatusHistoryDto = components["schemas"]["GetJobApplicationByIdStatusHistoryDto"];
type ListMyTagsItemDto = components["schemas"]["ListMyTagsItemDto"];
type UpdateSalaryDto = components["schemas"]["UpdateJobApplicationSalaryDto"];

interface JobApplicationDetailsProps {
  isCreating: boolean;
  isDetailsLoading: boolean;
  formData: UpdateRequest;
  dropdowns: DropdownsResponse;
  availableTags: ListMyTagsItemDto[];
  isSaving: boolean;
  isModified: boolean;
  comments: CommentDto[];
  statusHistory: StatusHistoryDto[];
  isCommentSubmitting: boolean;
  onClose: () => void;
  onFormChange: (field: keyof UpdateRequest, value: UpdateRequest[keyof UpdateRequest]) => void;
  onAddTag: (tag: string) => boolean;
  onToggleTag: (tag: string) => void;
  onSave: () => void;
  onDelete: () => void;
  onAddComment: (content: string) => Promise<boolean>;
  onDeleteComment: (id: string) => void;
}

export function JobApplicationDetails({
  isCreating,
  isDetailsLoading,
  formData,
  dropdowns,
  availableTags,
  isSaving,
  isModified,
  comments,
  statusHistory,
  isCommentSubmitting,
  onClose,
  onFormChange,
  onAddTag,
  onToggleTag,
  onSave,
  onDelete,
  onAddComment,
  onDeleteComment,
}: JobApplicationDetailsProps) {
  const [tagInput, setTagInput] = useState("");
  const [newComment, setNewComment] = useState("");
  const [isTagDropdownOpen, setIsTagDropdownOpen] = useState(false);
  const [isStatusHistoryOpen, setIsStatusHistoryOpen] = useState(false);
  const selectedTags = formData.tags ?? [];
  const normalizedTagInput = tagInput.trim().toLowerCase();
  const sortedStatusHistory = [...statusHistory].sort(
    (left, right) => Date.parse(right.changedAt) - Date.parse(left.changedAt),
  );

  const getStatusName = (statusValue: number | string | null | undefined) => {
    if (statusValue === null || statusValue === undefined) {
      return "Unknown";
    }

    return (
      dropdowns.jobApplicationStatuses.find((option) => option.value.toString() === statusValue.toString())?.name ??
      statusValue.toString()
    );
  };

  const filteredAvailableTags = availableTags.filter((tag) => {
    if (!normalizedTagInput) {
      return true;
    }

    return tag.name.toLowerCase().includes(normalizedTagInput);
  });

  const handleAddTag = () => {
    const wasAdded = onAddTag(tagInput);
    if (wasAdded) {
      setTagInput("");
    }
  };

  const handleAddComment = async () => {
    const wasAdded = await onAddComment(newComment);
    if (wasAdded) {
      setNewComment("");
    }
  };

  const updateSalary = (
    salaryTypeValue: number | string,
    field: keyof UpdateSalaryDto,
    value: UpdateSalaryDto[keyof UpdateSalaryDto],
  ) => {
    const currentSalaries = formData.salaries ?? [];
    const nextSalaries = currentSalaries.map((salary) => {
      if (salary.salaryType.toString() !== salaryTypeValue.toString()) {
        return salary;
      }

      return {
        ...salary,
        [field]: value,
      };
    });

    onFormChange("salaries", nextSalaries);
  };

  const handleCurrencyChange = (salaryTypeValue: number | string, currency: string) => {
    const currentSalaries = formData.salaries ?? [];
    const areOtherCurrenciesEmpty = currentSalaries
      .filter((salary) => salary.salaryType.toString() !== salaryTypeValue.toString())
      .every((salary) => !(salary.currency ?? "").trim());

    if (currency && areOtherCurrenciesEmpty) {
      onFormChange(
        "salaries",
        currentSalaries.map((salary) => ({
          ...salary,
          currency,
        })),
      );
      return;
    }

    updateSalary(salaryTypeValue, "currency", currency);
  };

  return (
    <div className="h-full overflow-y-auto p-6">
      <Card className="border-0 shadow-none">
        <CardHeader className="relative">
          <CardTitle>{isCreating ? "Create Job Application" : "Edit Job Application"}</CardTitle>
          <Button variant="ghost" size="icon" className="absolute right-4 top-4" onClick={onClose}>
            <span className="text-xl">x</span>
          </Button>
        </CardHeader>

        <CardContent className="space-y-4">
          {isDetailsLoading && !isCreating ? (
            <div className="text-sm text-muted-foreground">Loading details...</div>
          ) : null}

            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <FormField label="Job Title" className="md:col-span-2">
                <Input
                  value={formData.jobTitle}
                  onChange={(event) => onFormChange("jobTitle", event.target.value)}
                  placeholder="Position title"
                />
              </FormField>
              <FormField label="Company">
                <Input
                  value={formData.companyName}
                  onChange={(event) => onFormChange("companyName", event.target.value)}
                  placeholder="Company name"
                />
              </FormField>
              <FormField label="Link">
                <Input
                  value={formData.link ?? ""}
                  onChange={(event) => onFormChange("link", event.target.value)}
                  placeholder="https://..."
                />
              </FormField>
              <FormField
                label={
                  <>
                    <label className="text-sm font-medium">Status</label>
                    {!isCreating ? (
                      <Button
                        type="button"
                        variant="ghost"
                        size="icon"
                        className="h-4 w-4 p-0"
                        onClick={() => setIsStatusHistoryOpen((prev) => !prev)}
                        title={isStatusHistoryOpen ? "Hide status history" : "Show status history"}
                      >
                        <ClockIcon aria-hidden="true" className="h-4 w-4" />
                        <span className="sr-only">Toggle status history</span>
                      </Button>
                    ) : null}
                  </>
                }
                labelRowClassName="gap-1"
              >
                <select
                  className="h-9 w-full rounded-md border bg-background px-3 text-sm"
                  value={formData.currentStatus.toString()}
                  onChange={(event) => onFormChange("currentStatus", event.target.value)}
                >
                  {dropdowns.jobApplicationStatuses.map((option) => (
                    <option key={option.value.toString()} value={option.value.toString()}>
                      {option.name}
                    </option>
                  ))}
                </select>
              </FormField>
              <FormField label="Work Type">
                <select
                  className="h-9 w-full rounded-md border bg-background px-3 text-sm"
                  value={formData.workType.toString()}
                  onChange={(event) => onFormChange("workType", event.target.value)}
                >
                  {dropdowns.workTypes.map((option) => (
                    <option key={option.value.toString()} value={option.value.toString()}>
                      {option.name}
                    </option>
                  ))}
                </select>
              </FormField>
            </div>

            <FormField label="Technologies">
              <Input
                value={formData.technologies ?? ""}
                onChange={(event) => onFormChange("technologies", event.target.value)}
                placeholder="React, TypeScript, .NET"
              />
            </FormField>

            <FormField label="Experience">
              <Input
                value={formData.experience ?? ""}
                onChange={(event) => onFormChange("experience", event.target.value)}
                placeholder="Mid / Senior / 3+ years"
              />
            </FormField>

            <FormField label="Description">
              <Textarea
                value={formData.description ?? ""}
                onChange={(event) => onFormChange("description", event.target.value)}
                rows={4}
              />
            </FormField>

            <FormField label="Requirements">
              <Textarea
                value={formData.requirements ?? ""}
                onChange={(event) => onFormChange("requirements", event.target.value)}
                rows={4}
              />
            </FormField>

            <FormField label="Benefits">
              <Textarea
                value={formData.benefits ?? ""}
                onChange={(event) => onFormChange("benefits", event.target.value)}
                rows={4}
              />
            </FormField>

            <FormField label="Salaries (monthly)">
              <div className="space-y-3">
                {(formData.salaries ?? []).map((salary) => {
                  const salaryType = dropdowns.salaryTypes.find(
                    (option) => option.value.toString() === salary.salaryType.toString(),
                  );
                  const salaryLabel = salaryType?.name ?? salary.salaryType.toString();

                  return (
                    <JobApplicationSalaryFields
                      key={salary.salaryType.toString()}
                      salary={salary}
                      salaryLabel={salaryLabel}
                      onAmountChange={updateSalary}
                      onCurrencyChange={handleCurrencyChange}
                    />
                  );
                })}
              </div>
            </FormField>

            <FormField label="Tags">
              <div className="relative flex gap-2">
                <Input
                  value={tagInput}
                  onChange={(event) => setTagInput(event.target.value)}
                  placeholder="Type to add or select tag"
                  onFocus={() => setIsTagDropdownOpen(true)}
                  onBlur={() => {
                    window.setTimeout(() => {
                      setIsTagDropdownOpen(false);
                    }, 100);
                  }}
                  onKeyDown={(event) => {
                    if (event.key === "Enter") {
                      event.preventDefault();
                      handleAddTag();
                    }
                  }}
                />
                <Button type="button" variant="outline" onClick={handleAddTag}>
                  Add
                </Button>

                {isTagDropdownOpen ? (
                  <div
                    className="absolute left-0 right-20 top-11 z-10 max-h-48 overflow-y-auto rounded-md border p-1 text-popover-foreground shadow-lg"
                    style={{ backgroundColor: "hsl(var(--popover))", opacity: 1 }}
                  >
                    {filteredAvailableTags.length === 0 ? (
                      <div className="px-2 py-1 text-xs text-muted-foreground">No matching tags</div>
                    ) : (
                      filteredAvailableTags.map((tag) => {
                        const isSelected = selectedTags.includes(tag.name);

                        return (
                          <button
                            key={tag.id}
                            type="button"
                            onMouseDown={(event) => {
                              event.preventDefault();
                              onToggleTag(tag.name);
                            }}
                            className={`flex w-full items-center justify-between rounded px-2 py-1 text-left text-sm transition-colors hover:bg-muted ${
                              isSelected ? "bg-muted" : ""
                            }`}
                          >
                            <span>#{tag.name}</span>
                            {isSelected ? <span className="text-xs text-muted-foreground">Selected</span> : null}
                          </button>
                        );
                      })
                    )}
                  </div>
                ) : null}
              </div>

              <div className="flex flex-wrap gap-2">
                {selectedTags.length === 0 ? (
                  <div className="text-xs text-muted-foreground">No tags selected</div>
                ) : (
                  selectedTags.map((tag) => (
                    <button
                      key={tag}
                      type="button"
                      onClick={() => onToggleTag(tag)}
                      className={`inline-flex h-8 items-center rounded-full border px-3 text-xs font-medium transition-opacity hover:opacity-80 ${getTagColorClass(tag)}`}
                    >
                      #{tag}
                    </button>
                  ))
                )}
              </div>
            </FormField>

            <Separator />

            <div className="flex gap-2">
              <Button onClick={onSave} disabled={isSaving || (!isCreating && !isModified)}>
                {isSaving ? "Saving..." : "Save"}
              </Button>
              {isCreating ? (
                <Button variant="outline" onClick={onClose} disabled={isSaving}>
                  Cancel
                </Button>
              ) : (
                <Button variant="destructive" onClick={onDelete} disabled={isSaving}>
                  Delete
                </Button>
              )}
            </div>

          {!isCreating ? (
            <>
              <Separator />
              <JobApplicationCommentsSection
                comments={comments}
                newComment={newComment}
                isCommentSubmitting={isCommentSubmitting}
                onNewCommentChange={setNewComment}
                onAddComment={() => void handleAddComment()}
                onDeleteComment={onDeleteComment}
              />
            </>
          ) : null}
        </CardContent>
      </Card>

      <DialogPrimitive.Root open={isStatusHistoryOpen} onOpenChange={setIsStatusHistoryOpen}>
        <DialogPrimitive.Portal>
          <DialogPrimitive.Overlay className="fixed inset-0 z-50 bg-transparent" />
          <DialogPrimitive.Content
            style={{
              backgroundColor: "hsl(var(--background))",
              borderColor: "hsl(var(--border))",
            }}
            className="fixed left-[50%] top-[50%] z-50 grid w-full max-w-lg translate-x-[-50%] translate-y-[-50%] gap-4 rounded-lg border p-6 shadow-lg"
          >
            <DialogPrimitive.Title className="text-lg font-semibold">Status History</DialogPrimitive.Title>

            <div className="max-h-80 space-y-2 overflow-y-auto">
              {sortedStatusHistory.length === 0 ? (
                <div className="rounded-md border p-3 text-sm text-muted-foreground">No status history yet</div>
              ) : (
                sortedStatusHistory.map((historyItem, index) => {
                  const previousStatusLabel = getStatusName(historyItem.previousStatus);
                  const nextStatusLabel = getStatusName(historyItem.newStatus);

                  return (
                    <div key={`${historyItem.changedAt}-${index}`} className="rounded-md border bg-muted/40 p-3">
                      <div className="text-xs text-muted-foreground">{new Date(historyItem.changedAt).toLocaleString()}</div>
                      <div className="text-sm">
                        {historyItem.previousStatus === null
                          ? `Set to ${nextStatusLabel}`
                          : `${previousStatusLabel} -> ${nextStatusLabel}`}
                      </div>
                    </div>
                  );
                })
              )}
            </div>

            <div className="flex justify-end">
              <DialogPrimitive.Close asChild>
                <Button variant="outline">Close</Button>
              </DialogPrimitive.Close>
            </div>
          </DialogPrimitive.Content>
        </DialogPrimitive.Portal>
      </DialogPrimitive.Root>
    </div>
  );
}
