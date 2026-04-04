import { createFileRoute } from '@tanstack/react-router'
import { HomePage } from '@/pages/home/HomePage'
import { Layout } from '@/components/layout/Layout'

export const Route = createFileRoute('/')({
  component: () => (
    <Layout>
      <HomePage />
    </Layout>
  ),
})
