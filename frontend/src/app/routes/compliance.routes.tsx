
import React from "react";
import type { RouteObject } from "react-router-dom";
import { ComplianceHome } from "../../modules/compliance/pages/ComplianceHome";
import { ComplianceQA } from "../../modules/compliance/pages/ComplianceQA";
import { RosterCheck } from "../../modules/compliance/pages/RosterCheck";
import { RosterResults } from "../../modules/compliance/pages/RosterResults";

export const complianceRoutes: RouteObject[] = [
  {
    path: "/compliance",
    element: <ComplianceHome />,
  },
  {
    path: "/compliance/qa",
    element: <ComplianceQA />,
  },
  {
    path: "/compliance/roster-check",
    element: <RosterCheck />,
  },
  {
    path: "/compliance/roster-results",
    element: <RosterResults />,
  },
];

