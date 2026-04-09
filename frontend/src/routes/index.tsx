import { createFileRoute, redirect } from "@tanstack/react-router";
import { HomePage } from "@/pages/home/HomePage";
import { Layout } from "@/components/layout/Layout";
import { Auth } from "@/libs/auth";
import { Routes } from "@/constants";

export const Route = createFileRoute(Routes.HOME)({
  beforeLoad: () => {
    if (!Auth.isAuthenticated()) {
      throw redirect({ to: Routes.LOGIN });
    }
  },
  component: () => (
    <Layout>
      <HomePage />
    </Layout>
  ),
});
