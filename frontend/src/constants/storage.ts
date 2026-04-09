export const StorageKeys = {
  TOKEN: "auth_token",
  USER_ID: "auth_user_id",
  USER_NAME: "auth_user_name",
} as const;

export type StorageKey = (typeof StorageKeys)[keyof typeof StorageKeys];
