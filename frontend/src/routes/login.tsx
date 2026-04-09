import { createFileRoute } from "@tanstack/react-router";
import { LoginPage } from "@/pages/login/LoginPage";
import { Routes } from "@/constants";

export const Route = createFileRoute(Routes.LOGIN)({
  component: LoginPage,
});