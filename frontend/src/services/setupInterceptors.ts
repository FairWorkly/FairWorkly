import axios from "axios";
import type { AxiosError, AxiosRequestConfig } from "axios";
import httpClient from "./httpClient";
import { logout, setAccessToken, setAuthData } from "../slices/auth";
import type { RootState } from "../store";

type StoreLike = {
  dispatch: (action: unknown) => void;
  getState: () => RootState;
};

type FailedRequest = {
  resolve: (token: string) => void;
  reject: (error: unknown) => void;
  config: AxiosRequestConfig & { _retry?: boolean };
};

const baseURL = import.meta.env.VITE_API_BASE_URL ?? "/api";
const refreshClient = axios.create({
  baseURL,
  timeout: 15000,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

let isRefreshing = false;
let failedQueue: FailedRequest[] = [];

function processQueue(error: unknown, token: string | null) {
  failedQueue.forEach(({ resolve, reject }) => {
    if (error) {
      reject(error);
      return;
    }
    if (!token) {
      reject(new Error("Missing access token"));
      return;
    }
    resolve(token);
  });
  failedQueue = [];
}

export function setupInterceptors(store: StoreLike) {
  httpClient.interceptors.request.use((config) => {
    const token = store.getState().auth.accessToken;
    if (token) {
      config.headers = config.headers ?? {};
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });

  httpClient.interceptors.response.use(
    (response) => response,
    async (error: AxiosError) => {
      const originalConfig = error.config as
        | (AxiosRequestConfig & { _retry?: boolean })
        | undefined;

      if (!originalConfig || !error.response) {
        return Promise.reject(error);
      }

      if (error.response.status !== 401) {
        return Promise.reject(error);
      }

      if (originalConfig._retry) {
        return Promise.reject(error);
      }
      originalConfig._retry = true;

      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({
            resolve: (token) => {
              originalConfig.headers = originalConfig.headers ?? {};
              originalConfig.headers.Authorization = `Bearer ${token}`;
              resolve(httpClient(originalConfig));
            },
            reject,
            config: originalConfig,
          });
        });
      }

      isRefreshing = true;

      try {
        const refreshResponse = await refreshClient.post("/auth/refresh");
        const accessToken =
          refreshResponse.data?.accessToken ?? refreshResponse.data?.token;
        const user = refreshResponse.data?.user ?? null;

        if (!accessToken) {
          throw new Error("Refresh succeeded without access token");
        }

        if (user) {
          store.dispatch(setAuthData({ user, accessToken }));
        } else {
          store.dispatch(setAccessToken(accessToken));
        }

        processQueue(null, accessToken);

        originalConfig.headers = originalConfig.headers ?? {};
        originalConfig.headers.Authorization = `Bearer ${accessToken}`;
        return httpClient(originalConfig);
      } catch (refreshError) {
        processQueue(refreshError, null);
        store.dispatch(logout());
        window.location.href = "/login";
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    },
  );
}
