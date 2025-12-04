// src/app/providers/ReduxProvider.tsx
import React from "react";
import { Provider } from "react-redux";
import { configureStore, createSlice } from "@reduxjs/toolkit";

interface UiState {
  sidebarOpen: boolean;
}

const initialUiState: UiState = {
  sidebarOpen: true,
};

// Temporary uiSlice，can move to src/slices/uiSlice.ts in the future
const uiSlice = createSlice({
  name: "ui",
  initialState: initialUiState,
  reducers: {
    toggleSidebar(state) {
      state.sidebarOpen = !state.sidebarOpen;
    },
  },
});

// can move to src/store/index.ts in the future and use "import { store } from "../../store";" instead
const tempStore = configureStore({ 
  reducer: {
    ui: uiSlice.reducer,
  },
});

export type RootState = ReturnType<typeof tempStore.getState>;
export type AppDispatch = typeof tempStore.dispatch;

interface ReduxProviderProps {
  children: React.ReactNode;
}

export const ReduxProvider: React.FC<ReduxProviderProps> = ({ children }) => {
  return <Provider store={tempStore}>{children}</Provider>;
};
