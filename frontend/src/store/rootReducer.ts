import { combineReducers } from "@reduxjs/toolkit";

import { authReducer } from "../slices/auth";
import { uiReducer } from "../slices/ui";
import { notificationsReducer } from "../slices/notifications";

export const rootReducer = combineReducers({
  auth: authReducer,
  ui: uiReducer,
  notifications: notificationsReducer,
});

export type RootState = ReturnType<typeof rootReducer>;