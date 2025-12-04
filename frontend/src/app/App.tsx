
import React from "react";
import { AppRoutes } from "./routes";
import { MainLayout } from "../shared/components/layout/MainLayout";

export const App: React.FC = () => {
  return (
    <MainLayout>
      <AppRoutes />
    </MainLayout>
  );
};
