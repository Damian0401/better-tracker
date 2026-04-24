export const JOB_APPLICATION_LIST_STATES = {
  ALL: "all",
  ACTIVE: "active",
  ARCHIVED: "archived",
} as const;

export type JobApplicationListState =
  (typeof JOB_APPLICATION_LIST_STATES)[keyof typeof JOB_APPLICATION_LIST_STATES];

export type Filters = {
  search: string;
  statuses: string[];
  workTypes: string[];
  tags: string[];
};
