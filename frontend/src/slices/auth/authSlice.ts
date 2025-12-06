import { createSlice } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

export interface AuthUser {
  id?: string;
  name?: string;
  role?: string; // e.g. "owner" | "manager" | "employee"
}

export interface AuthState {
  isAuthenticated: boolean;
  user?: AuthUser;
  token?: string; // placeholder; you can remove if you don't store token in Redux
}

const initialState: AuthState = {
  isAuthenticated: false,
  user: undefined,
  token: undefined,
};

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    setAuthenticated(state, action: PayloadAction<boolean>) {
      state.isAuthenticated = action.payload;
      if (!action.payload) {
        state.user = undefined;
        state.token = undefined;
      }
    },
    setUser(state, action: PayloadAction<AuthUser | undefined>) {
      state.user = action.payload;
    },
    setToken(state, action: PayloadAction<string | undefined>) {
      state.token = action.payload;
    },
    logout(state) {
      state.isAuthenticated = false;
      state.user = undefined;
      state.token = undefined;
    },
  },
});

export const { setAuthenticated, setUser, setToken, logout } =
  authSlice.actions;

export default authSlice.reducer;