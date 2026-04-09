import { z } from "zod";

const envSchema = z.object({
  VITE_API_URL: z.url(),
});

export const Env = envSchema.parse(import.meta.env);
