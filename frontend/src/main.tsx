
import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import { ReduxProvider } from "./app/providers/ReduxProvider";
import { ThemeProvider } from "./app/providers/ThemeProvider";
import { App } from "./app/App";

const queryClient = new QueryClient();

ReactDOM.createRoot(document.getElementById("root") as HTMLElement).render(
  <React.StrictMode>
    <BrowserRouter>
      <QueryClientProvider client={queryClient}>
        <ReduxProvider>
          <ThemeProvider>
            <App />
          </ThemeProvider>
        </ReduxProvider>
      </QueryClientProvider>
    </BrowserRouter>
  </React.StrictMode>,
);

