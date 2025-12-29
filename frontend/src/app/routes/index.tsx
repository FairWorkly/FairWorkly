import { createBrowserRouter } from 'react-router-dom'
import { publicRoutes } from './public.routes'
import { fairbotRoutes } from './fairbot.routes'
import { toolRoutes } from './tools.routes'
import { errorRoutes } from './error.routes'

export const router = createBrowserRouter([
  ...publicRoutes,
  ...fairbotRoutes,
  ...toolRoutes,
  ...errorRoutes,
])
