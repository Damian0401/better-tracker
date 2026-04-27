export const Routes = {
  HOME: "/",
  LOGIN: "/login",
  REGISTER: "/register",
  NOTES: "/notes",
  JOB_APPLICATIONS: "/job-applications",
  ARCHIVE: "/archive",
  STATISTICS: "/statistics",
} as const;

export type RoutePath = (typeof Routes)[keyof typeof Routes];
