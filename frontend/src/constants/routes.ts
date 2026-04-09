export const Routes = {
  HOME: "/",
  LOGIN: "/login",
  REGISTER: "/register",
  NOTES: "/notes",
} as const;

export type RoutePath = (typeof Routes)[keyof typeof Routes];
