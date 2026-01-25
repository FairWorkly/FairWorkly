import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

export interface AuthUser {
  id: string;
  name?: string;
  email?: string;
  role?: string;
}

export type AuthStatus =
  | "idle"
  | "initializing"
  | "authenticating"
  | "authenticated"
  | "unauthenticated";

export interface AuthState {
  user: AuthUser | null;
  accessToken: string | null;
  status: AuthStatus;
}

const initialState: AuthState = {
  user: null,
  accessToken: null,
  status: "initializing",
};

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setAuthData(
      state,
      action: PayloadAction<{ user: AuthUser; accessToken: string }>
    ) {
      state.user = action.payload.user;
      state.accessToken = action.payload.accessToken;
      state.status = "authenticated";
    },
    setAccessToken(state, action: PayloadAction<string>) {
      state.accessToken = action.payload;
      if (action.payload) {
        state.status = "authenticated";
      }
    },
    logout(state) {
      state.user = null;
      state.accessToken = null;
      state.status = "unauthenticated";
    },
    setInitialized(state) {
      state.status =
        state.user && state.accessToken ? "authenticated" : "unauthenticated";
    },
  },
});

export const { setAuthData, setAccessToken, logout, setInitialized } =
  authSlice.actions;

export default authSlice.reducer;
