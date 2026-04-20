import { createFileRoute, redirect } from "@tanstack/react-router";
import { Layout } from "@/components/layout/Layout";
import { JobApplicationsPage } from "@/pages/job-applications/JobApplicationsPage";
import { Routes } from "@/constants";
import { Auth } from "@/libs/auth";

export const Route = createFileRoute(Routes.JOB_APPLICATIONS)({
  beforeLoad: () => {
    if (!Auth.isAuthenticated()) {
      throw redirect({ to: Routes.LOGIN });
    }
  },
  component: () => (
    <Layout>
      <JobApplicationsPage />
    </Layout>
  ),
});
