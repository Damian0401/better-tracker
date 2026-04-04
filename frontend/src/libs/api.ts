import createFetchClient from "openapi-fetch";
import type { paths } from "./api.schema.g";
import { Env } from "./env";

export const Api = createFetchClient<paths>({
  baseUrl: Env.VITE_API_URL
});
