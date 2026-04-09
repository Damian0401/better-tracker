import { StorageKeys } from "@/constants";

export type AuthUser = {
  userId: string;
  userName: string;
};

export const Auth = {
  getToken: (): string | null => {
    return localStorage.getItem(StorageKeys.TOKEN);
  },

  setToken: (token: string): void => {
    localStorage.setItem(StorageKeys.TOKEN, token);
  },

  removeToken: (): void => {
    localStorage.removeItem(StorageKeys.TOKEN);
    localStorage.removeItem(StorageKeys.USER_ID);
    localStorage.removeItem(StorageKeys.USER_NAME);
  },

  isAuthenticated: (): boolean => {
    return !!localStorage.getItem(StorageKeys.TOKEN);
  },

  setUser: (user: AuthUser): void => {
    localStorage.setItem(StorageKeys.USER_ID, user.userId);
    localStorage.setItem(StorageKeys.USER_NAME, user.userName);
  },

  getUser: (): AuthUser | null => {
    const userId = localStorage.getItem(StorageKeys.USER_ID);
    const userName = localStorage.getItem(StorageKeys.USER_NAME);
    if (userId && userName) {
      return { userId, userName };
    }
    return null;
  },
};