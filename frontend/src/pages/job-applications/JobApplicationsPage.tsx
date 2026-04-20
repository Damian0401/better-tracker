import { useEffect, useState } from "react";
import { Api } from "@/libs/api";
import type { components } from "@/libs/api.schema.g";
import { normalizeTextInput } from "@/libs/utils";
import { JobApplicationDetails } from "./components/JobApplicationDetails";
import { JobApplicationsListPanel } from "./components/JobApplicationsListPanel";
import type { Filters } from "./components/types";
import { toast } from "sonner";

type DropdownsResponse = components["schemas"]["GetJobApplicationDropdownsResponse"];
type ListItem = components["schemas"]["ListJobApplicationsItemDto"];
type DetailsDto = components["schemas"]["GetJobApplicationByIdDto"];
type CommentDto = components["schemas"]["GetJobApplicationByIdCommentDto"];
type CreateRequest = components["schemas"]["CreateJobApplicationRequest"];
type UpdateRequest = components["schemas"]["UpdateJobApplicationBody"];
type ListMyTagsItemDto = components["schemas"]["ListMyTagsItemDto"];

const PAGE_SIZE = 10;
const INITIAL_FILTERS: Filters = {
  search: "",
  status: "",
  workType: "",
  tag: "",
};

const emptyFormData: UpdateRequest = {
  title: "",
  jobTitle: "",
  description: "",
  companyName: "",
  requirements: "",
  benefits: "",
  link: "",
  technologies: "",
  experience: "",
  workType: 0,
  currentStatus: 0,
  salaries: [],
  tags: [],
};

const mapDetailsToUpdateRequest = (details: DetailsDto): UpdateRequest => ({
  title: details.title,
  jobTitle: details.jobTitle,
  description: details.description ?? "",
  companyName: details.companyName,
  requirements: details.requirements ?? "",
  benefits: details.benefits ?? "",
  link: details.link ?? "",
  technologies: details.technologies ?? "",
  experience: details.experience ?? "",
  workType: details.workType,
  currentStatus: details.currentStatus,
  salaries: details.salaries,
  tags: details.tags,
});

export function JobApplicationsPage() {
  const [applications, setApplications] = useState<ListItem[]>([]);
  const [selectedApplicationId, setSelectedApplicationId] = useState<string | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [formData, setFormData] = useState<UpdateRequest>(emptyFormData);
  const [baselineFormData, setBaselineFormData] = useState<UpdateRequest>(emptyFormData);
  const [comments, setComments] = useState<CommentDto[]>([]);
  const [availableTags, setAvailableTags] = useState<ListMyTagsItemDto[]>([]);
  const [dropdowns, setDropdowns] = useState<DropdownsResponse>({
    workTypes: [],
    salaryTypes: [],
    jobApplicationStatuses: [],
  });
  const [filters, setFilters] = useState<Filters>(INITIAL_FILTERS);
  const [skip, setSkip] = useState(0);
  const [total, setTotal] = useState(0);
  const [isListLoading, setIsListLoading] = useState(false);
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const [isDetailsLoading, setIsDetailsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [isCommentSubmitting, setIsCommentSubmitting] = useState(false);

  const isModified = JSON.stringify(formData) !== JSON.stringify(baselineFormData);

  const fetchDropdowns = async () => {
    const response = await Api.GET("/api/v1/job-applications/dropdowns");
    if (!response.data) {
      toast.error("Failed to load filter options");
      return;
    }

    setDropdowns(response.data);
  };

  const fetchTags = async () => {
    const response = await Api.GET("/api/v1/tags/me");
    if (!response.data) {
      toast.error("Failed to load tags");
      return;
    }

    setAvailableTags(response.data.items);
  };

  const fetchApplications = async (skipParam: number, shouldAppend: boolean, activeFilters: Filters) => {
    const query: {
      Count: number;
      Skip: number;
      Status?: number | string;
      WorkType?: number | string;
      Tag?: string;
      Search?: string;
    } = {
      Count: PAGE_SIZE,
      Skip: skipParam,
    };

    if (activeFilters.status) {
      query.Status = activeFilters.status;
    }
    if (activeFilters.workType) {
      query.WorkType = activeFilters.workType;
    }
    if (activeFilters.tag) {
      query.Tag = activeFilters.tag;
    }
    if (activeFilters.search.trim()) {
      query.Search = activeFilters.search.trim();
    }

    const response = await Api.GET("/api/v1/job-applications", {
      params: {
        query,
      },
    });

    if (!response.data) {
      toast.error("Failed to load job applications");
      return;
    }

    setTotal(Number(response.data.total));
    setSkip(skipParam + response.data.items.length);
    setApplications((prev) => (shouldAppend ? [...prev, ...response.data.items] : response.data.items));
  };

  const fetchApplicationDetails = async (id: string) => {
    setIsDetailsLoading(true);

    try {
      const response = await Api.GET("/api/v1/job-applications/{id}", {
        params: { path: { id } },
      });

      if (!response.data) {
        toast.error("Failed to load job application details");
        return;
      }

      const details = response.data.jobApplication;
      const mapped = mapDetailsToUpdateRequest(details);
      setFormData(mapped);
      setBaselineFormData(mapped);
      setComments(details.comments);
    } finally {
      setIsDetailsLoading(false);
    }
  };

  const resetDetails = () => {
    setSelectedApplicationId(null);
    setIsCreating(false);
    setFormData(emptyFormData);
    setBaselineFormData(emptyFormData);
    setComments([]);
  };

  useEffect(() => {
    const loadInitialData = async () => {
      await fetchDropdowns();
      await fetchTags();
      setIsListLoading(true);
      await fetchApplications(0, false, INITIAL_FILTERS);
      setIsListLoading(false);
    };

    void loadInitialData();
  }, []);

  useEffect(() => {
    const loadFilteredList = async () => {
      setIsListLoading(true);
      await fetchApplications(0, false, filters);
      setIsListLoading(false);
    };

    void loadFilteredList();
  }, [filters]);

  const handleSelect = async (id: string) => {
    setIsCreating(false);
    setSelectedApplicationId(id);
    await fetchApplicationDetails(id);
  };

  const handleCreate = () => {
    resetDetails();
    setIsCreating(true);

    const defaultWorkType = dropdowns.workTypes[0]?.value ?? 0;
    const defaultStatus = dropdowns.jobApplicationStatuses[0]?.value ?? 0;
    const nextFormData: UpdateRequest = {
      ...emptyFormData,
      workType: defaultWorkType,
      currentStatus: defaultStatus,
    };

    setFormData(nextFormData);
    setBaselineFormData(nextFormData);
  };

  const handleCloseDetails = () => {
    resetDetails();
  };

  const handleLoadMore = async () => {
    setIsLoadingMore(true);
    await fetchApplications(skip, true, filters);
    setIsLoadingMore(false);
  };

  const handleFormChange = (field: keyof UpdateRequest, value: UpdateRequest[keyof UpdateRequest]) => {
    setFormData((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleToggleTag = (tag: string) => {
    const selectedTags = formData.tags ?? [];
    const nextTags = selectedTags.includes(tag)
      ? selectedTags.filter((item) => item !== tag)
      : [...selectedTags, tag];

    setFormData((prev) => ({
      ...prev,
      tags: nextTags,
    }));
  };

  const handleAddTag = (rawTag: string): boolean => {
    const normalizedTagName = rawTag.trim();
    if (!normalizedTagName) {
      return false;
    }

    setAvailableTags((prev) => {
      if (prev.some((item) => item.name === normalizedTagName)) {
        return prev;
      }

      return [
        {
          id: `local-${normalizedTagName}`,
          name: normalizedTagName,
        },
        ...prev,
      ];
    });

    const selectedTags = formData.tags ?? [];
    if (!selectedTags.includes(normalizedTagName)) {
      setFormData((prev) => ({
        ...prev,
        tags: [...(prev.tags ?? []), normalizedTagName],
      }));
    }

    return true;
  };

  const buildPayload = (): CreateRequest => ({
    ...formData,
    description: normalizeTextInput(formData.description ?? ""),
    requirements: normalizeTextInput(formData.requirements ?? ""),
    benefits: normalizeTextInput(formData.benefits ?? ""),
    link: normalizeTextInput(formData.link ?? ""),
    technologies: normalizeTextInput(formData.technologies ?? ""),
    experience: normalizeTextInput(formData.experience ?? ""),
    salaries: formData.salaries ?? [],
    tags: formData.tags ?? [],
  });

  const handleSave = async () => {
    if (!formData.title.trim() || !formData.jobTitle.trim() || !formData.companyName.trim()) {
      toast.error("Title, job title, and company name are required");
      return;
    }

    setIsSaving(true);

    try {
      const payload = buildPayload();

      if (isCreating) {
        const response = await Api.POST("/api/v1/job-applications", {
          body: payload,
        });

        if (!response.data) {
          toast.error("Failed to create job application");
          return;
        }

        toast.success("Job application created");
        setIsListLoading(true);
        await fetchApplications(0, false, filters);
        setIsListLoading(false);

        resetDetails();
        await fetchTags();
        return;
      }

      if (!selectedApplicationId) {
        return;
      }

      const response = await Api.PUT("/api/v1/job-applications/{id}", {
        params: { path: { id: selectedApplicationId } },
        body: payload,
      });

      if (response.error) {
        toast.error("Failed to update job application");
        return;
      }

      toast.success("Job application updated");
      setBaselineFormData(formData);

      setApplications((prev) =>
        prev.map((item) =>
          item.id === selectedApplicationId
            ? {
                ...item,
                title: formData.title,
                jobTitle: formData.jobTitle,
                companyName: formData.companyName,
                workType: formData.workType,
                currentStatus: formData.currentStatus,
                tags: formData.tags ?? [],
              }
            : item
        )
      );

      await fetchTags();
    } finally {
      setIsSaving(false);
    }
  };

  const handleCreateComment = async (rawComment: string): Promise<boolean> => {
    if (!selectedApplicationId || !rawComment.trim()) {
      return false;
    }

    setIsCommentSubmitting(true);

    try {
      const response = await Api.POST("/api/v1/comments", {
        body: {
          jobApplicationId: selectedApplicationId,
          content: rawComment.trim(),
        },
      });

      if (!response.data) {
        toast.error("Failed to add comment");
        return false;
      }

      toast.success("Comment added");
      await fetchApplicationDetails(selectedApplicationId);
      return true;
    } finally {
      setIsCommentSubmitting(false);
    }
  };

  const handleDeleteComment = async (commentId: string) => {
    if (!selectedApplicationId) {
      return;
    }

    const response = await Api.DELETE("/api/v1/comments/{id}", {
      params: { path: { id: commentId } },
    });

    if (response.error) {
      toast.error("Failed to delete comment");
      return;
    }

    toast.success("Comment deleted");
    await fetchApplicationDetails(selectedApplicationId);
  };

  return (
    <div className="flex h-full">
      <JobApplicationsListPanel
        applications={applications}
        selectedApplicationId={selectedApplicationId}
        filters={filters}
        dropdowns={dropdowns}
        availableTags={availableTags}
        total={total}
        isListLoading={isListLoading}
        isLoadingMore={isLoadingMore}
        onCreate={handleCreate}
        onSearchChange={(value) => setFilters((prev) => ({ ...prev, search: value }))}
        onStatusChange={(value) => setFilters((prev) => ({ ...prev, status: value }))}
        onWorkTypeChange={(value) => setFilters((prev) => ({ ...prev, workType: value }))}
        onTagChange={(value) => setFilters((prev) => ({ ...prev, tag: value }))}
        onSelect={(id) => void handleSelect(id)}
        onLoadMore={() => void handleLoadMore()}
      />
      <JobApplicationDetails
        key={selectedApplicationId ?? (isCreating ? "create" : "empty")}
        selectedApplicationId={selectedApplicationId}
        isCreating={isCreating}
        isDetailsLoading={isDetailsLoading}
        formData={formData}
        dropdowns={dropdowns}
        availableTags={availableTags}
        isSaving={isSaving}
        isModified={isModified}
        comments={comments}
        isCommentSubmitting={isCommentSubmitting}
        onClose={handleCloseDetails}
        onFormChange={handleFormChange}
        onAddTag={handleAddTag}
        onToggleTag={handleToggleTag}
        onSave={() => void handleSave()}
        onAddComment={handleCreateComment}
        onDeleteComment={(id) => void handleDeleteComment(id)}
      />
    </div>
  );
}
