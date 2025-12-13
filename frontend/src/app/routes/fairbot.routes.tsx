import React from 'react'
import type { RouteObject } from 'react-router-dom'
import { FairBotChat } from "../../modules/fairbot/pages/FairBotChat";

export const fairbotRoutes: RouteObject[] = [
  {
    path: '/fairbotchat',
    element: <FairBotChat />,
  },
]
