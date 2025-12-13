import React from 'react'
import type { RouteObject } from 'react-router-dom'
import { ConsolidatePage } from "../../modules/consolidate/pages/ConsolidatePage";

export const toolRoutes: RouteObject[] = [
    {
        path: '/consolidate',
        element: <ConsolidatePage />,
    },
]
