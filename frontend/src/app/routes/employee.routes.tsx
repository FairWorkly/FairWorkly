
import React from "react";
import type { RouteObject } from "react-router-dom";
import { EmployeeHome } from "../../modules/employee/pages/EmployeeHome";

export const employeeRoutes: RouteObject[] = [
  {
    path: "/employee",
    element: <EmployeeHome />,
  },
  {
    path: "/employee/help",
    element: <EmployeeHome />, // TEMP: Reusing this component - plan to create dedicated EmployeeHelpPage
  },
];
