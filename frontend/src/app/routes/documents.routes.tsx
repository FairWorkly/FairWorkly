
import React from "react";
import type { RouteObject } from "react-router-dom";
import { DocumentsHome } from "../../modules/documents/pages/DocumentsHome";

export const documentsRoutes: RouteObject[] = [
  {
    path: "/documents",
    element: <DocumentsHome />,
  },
];
