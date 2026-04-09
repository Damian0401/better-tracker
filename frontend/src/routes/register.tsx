import { createFileRoute } from "@tanstack/react-router";
import { RegisterPage } from "@/pages/register/RegisterPage";
import { Routes } from "@/constants";

export const Route = createFileRoute(Routes.REGISTER)({
  component: RegisterPage,
});