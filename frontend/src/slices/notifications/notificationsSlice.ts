import { createSlice, nanoid } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";

export type NotificationVariant = "success" | "info" | "warning" | "error";

export interface NotificationItem {
  id: string;
  message: string;
  variant: NotificationVariant;
  durationMs?: number;
}

export interface NotificationsState {
  queue: NotificationItem[];
}

const initialState: NotificationsState = {
  queue: [],
};

const notificationsSlice = createSlice({
  name: "notifications",
  initialState,
  reducers: {
    enqueueNotification: {
      reducer(state, action: PayloadAction<NotificationItem>) {
        state.queue.push(action.payload);
      },
      prepare(input: Omit<NotificationItem, "id">) {
        return {
          payload: {
            id: nanoid(),
            ...input,
          },
        };
      },
    },
    dequeueNotification(state) {
      state.queue.shift();
    },
    clearNotifications(state) {
      state.queue = [];
    },
  },
});

export const {
  enqueueNotification,
  dequeueNotification,
  clearNotifications,
} = notificationsSlice.actions;

export default notificationsSlice.reducer;