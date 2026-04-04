import { createFileRoute } from '@tanstack/react-router'
import { NotesPage } from '@/pages/notes/NotesPage'
import { Layout } from '@/components/layout/Layout'

export const Route = createFileRoute('/notes')({
  component: () => (
    <Layout>
      <NotesPage />
    </Layout>
  ),
})
