import { useState } from "react";
import type { components } from "@/libs/api.schema.g";
import { getTagColorClass } from "@/libs/utils";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Separator } from "@/components/ui/separator";
import { Textarea } from "@/components/ui/textarea";
import { JobApplicationCommentsSection } from "./JobApplicationCommentsSection";

type DropdownsResponse = components["schemas"]["GetJobApplicationDropdownsResponse"];
type UpdateRequest = components["schemas"]["UpdateJobApplicationBody"];
type CommentDto = components["schemas"]["GetJobApplicationByIdCommentDto"];
type ListMyTagsItemDto = components["schemas"]["ListMyTagsItemDto"];

interface JobApplicationDetailsProps {
  selectedApplicationId: string | null;
  isCreating: boolean;
  isDetailsLoading: boolean;
  formData: UpdateRequest;
  dropdowns: DropdownsResponse;
  availableTags: ListMyTagsItemDto[];
  isSaving: boolean;
  isModified: boolean;
  comments: CommentDto[];
  isCommentSubmitting: boolean;
  onClose: () => void;
  onFormChange: (field: keyof UpdateRequest, value: UpdateRequest[keyof UpdateRequest]) => void;
  onAddTag: (tag: string) => boolean;
  onToggleTag: (tag: string) => void;
  onSave: () => void;
  onAddComment: (content: string) => Promise<boolean>;
  onDeleteComment: (id: string) => void;
}

export function JobApplicationDetails({
  selectedApplicationId,
  isCreating,
  isDetailsLoading,
  formData,
  dropdowns,
  availableTags,
  isSaving,
  isModified,
  comments,
  isCommentSubmitting,
  onClose,
  onFormChange,
  onAddTag,
  onToggleTag,
  onSave,
  onAddComment,
  onDeleteComment,
}: JobApplicationDetailsProps) {
  const [tagInput, setTagInput] = useState("");
  const [newComment, setNewComment] = useState("");
  const [isTagDropdownOpen, setIsTagDropdownOpen] = useState(false);
  const selectedTags = formData.tags ?? [];
  const normalizedTagInput = tagInput.trim().toLowerCase();

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

  return (
    <div className="flex-1 overflow-y-auto p-6">
      {selectedApplicationId || isCreating ? (
        <Card>
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
              <div className="space-y-2">
                <label className="text-sm font-medium">Title</label>
                <Input
                  value={formData.title}
                  onChange={(event) => onFormChange("title", event.target.value)}
                  placeholder="Internal title"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Company</label>
                <Input
                  value={formData.companyName}
                  onChange={(event) => onFormChange("companyName", event.target.value)}
                  placeholder="Company name"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Job Title</label>
                <Input
                  value={formData.jobTitle}
                  onChange={(event) => onFormChange("jobTitle", event.target.value)}
                  placeholder="Position title"
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Link</label>
                <Input
                  value={formData.link ?? ""}
                  onChange={(event) => onFormChange("link", event.target.value)}
                  placeholder="https://..."
                />
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Status</label>
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
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium">Work Type</label>
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
              </div>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Technologies</label>
              <Input
                value={formData.technologies ?? ""}
                onChange={(event) => onFormChange("technologies", event.target.value)}
                placeholder="React, TypeScript, .NET"
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Experience</label>
              <Input
                value={formData.experience ?? ""}
                onChange={(event) => onFormChange("experience", event.target.value)}
                placeholder="Mid / Senior / 3+ years"
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Description</label>
              <Textarea
                value={formData.description ?? ""}
                onChange={(event) => onFormChange("description", event.target.value)}
                rows={4}
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Requirements</label>
              <Textarea
                value={formData.requirements ?? ""}
                onChange={(event) => onFormChange("requirements", event.target.value)}
                rows={4}
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Benefits</label>
              <Textarea
                value={formData.benefits ?? ""}
                onChange={(event) => onFormChange("benefits", event.target.value)}
                rows={4}
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Tags</label>
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
            </div>

            <Separator />

            <div className="flex gap-2">
              <Button onClick={onSave} disabled={isSaving || (!isCreating && !isModified)}>
                {isSaving ? "Saving..." : "Save"}
              </Button>
              <Button variant="outline" onClick={onClose} disabled={isSaving}>
                Cancel
              </Button>
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
      ) : (
        <div className="flex h-full items-center justify-center text-muted-foreground">
          Select an application to view details or create a new one
        </div>
      )}
    </div>
  );
}
