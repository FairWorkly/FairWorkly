import { useRoutes } from 'react-router-dom'
import { publicRoutes } from '@/app/routes/public.routes'
import { toolRoutes } from '@/app/routes/tools.routes'
import { fairbotRoutes } from './routes/fairbot.routes'

export function App() {
  const routes = useRoutes([...fairbotRoutes, ...publicRoutes, ...toolRoutes])
  return routes
}
