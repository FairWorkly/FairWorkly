import React from 'react'
import type { RouteObject } from 'react-router-dom'
import { FairWorklyHome } from "../../modules/home/pages/FairWorklyHome";
import { LoginPage } from '../../modules/auth/pages/LoginPage'

export const publicRoutes: RouteObject[] = [
    {
        path: '/',
        element: <FairWorklyHome />,
    },
    {
        path: '/login',
        element: <LoginPage />,
    },
]
