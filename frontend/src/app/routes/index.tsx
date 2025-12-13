import React from 'react'
import { useRoutes, type RouteObject } from 'react-router-dom'
import { fairbotRoutes } from './fairbot.routes'
import { publicRoutes } from './public.routes'
import { toolRoutes } from './tools.routes'

const routes: RouteObject[] = [
    ...fairbotRoutes,
    ...publicRoutes,
    ...toolRoutes,
]

export const AppRoutes: React.FC = () => {
    const element = useRoutes(routes)
    return element
}
