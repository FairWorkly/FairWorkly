
import React from "react";
import type { RouteObject } from "react-router-dom";
import { FairWorklyHome } from "../../modules/home/pages/FairWorklyHome";

export const homeRoutes: RouteObject[] = [
  {
    path: "/",
    element: <FairWorklyHome />,
  },
];
