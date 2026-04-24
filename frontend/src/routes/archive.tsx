import { createFileRoute, redirect } from "@tanstack/react-router";
import { Layout } from "@/components/layout/Layout";
import { Routes } from "@/constants";
import { Auth } from "@/libs/auth";
import { ArchivePage } from "@/pages/archive/ArchivePage";

export const Route = createFileRoute(Routes.ARCHIVE)({
  beforeLoad: () => {
    if (!Auth.isAuthenticated()) {
      throw redirect({ to: Routes.LOGIN });
    }
  },
  component: () => (
    <Layout>
      <ArchivePage />
    </Layout>
  ),
});
