import { createFileRoute, redirect } from "@tanstack/react-router";
import { NotesPage } from "@/pages/notes/NotesPage";
import { Layout } from "@/components/layout/Layout";
import { Auth } from "@/libs/auth";
import { Routes } from "@/constants";

export const Route = createFileRoute(Routes.NOTES)({
  beforeLoad: () => {
    if (!Auth.isAuthenticated()) {
      throw redirect({ to: Routes.LOGIN });
    }
  },
  component: () => (
    <Layout>
      <NotesPage />
    </Layout>
  ),
});
