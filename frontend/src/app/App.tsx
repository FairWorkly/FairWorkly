import { useRoutes } from 'react-router-dom'
import { publicRoutes } from '@/app/routes/public.routes'
import { toolRoutes } from '@/app/routes/tools.routes'

export function App() {
  const routes = useRoutes([...publicRoutes, ...toolRoutes])

  return routes
}
