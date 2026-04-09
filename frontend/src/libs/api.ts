import createFetchClient, { type Middleware } from "openapi-fetch";
import type { paths } from "./api.schema.g";
import { Env } from "./env";
import { Auth } from "./auth";
import { Routes } from "@/constants";

const createAuthMiddleware = (): Middleware => ({
  async onRequest({ request }) {
    const url = request.url;
    if (
      url.includes("/api/v1/auth/login") ||
      url.includes("/api/v1/auth/register")
    ) {
      return request;
    }

    const token = Auth.getToken();
    if (token) {
      request.headers.set("Authorization", `Bearer ${token}`);
    }

    return request;
  },
  async onResponse({ request, response }) {
    const url = request.url;
    if (
      url.includes("/api/v1/auth/login") ||
      url.includes("/api/v1/auth/register")
    ) {
      return response;
    }

    if (response.status === 401) {
      Auth.removeToken();
      window.location.href = Routes.LOGIN;
    }
    return response;
  },
});

const baseApi = createFetchClient<paths>({
  baseUrl: Env.VITE_API_URL,
});

baseApi.use(createAuthMiddleware());

export const Api = {
  GET: baseApi.GET.bind(baseApi),
  POST: baseApi.POST.bind(baseApi),
  PUT: baseApi.PUT.bind(baseApi),
  DELETE: baseApi.DELETE.bind(baseApi),
  PATCH: baseApi.PATCH.bind(baseApi),
};
