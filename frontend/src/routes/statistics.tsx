import { createFileRoute, redirect } from "@tanstack/react-router";
import { Layout } from "@/components/layout/Layout";
import { Routes } from "@/constants";
import { Auth } from "@/libs/auth";
import { StatisticsPage } from "@/pages/statistics/StatisticsPage";

export const Route = createFileRoute(Routes.STATISTICS)({
  beforeLoad: () => {
    if (!Auth.isAuthenticated()) {
      throw redirect({ to: Routes.LOGIN });
    }
  },
  component: () => (
    <Layout>
      <StatisticsPage />
    </Layout>
  ),
});
