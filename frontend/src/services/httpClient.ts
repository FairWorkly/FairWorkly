import axios from "axios";

/**
 * Unified HTTP client
 * - All API modules use this instance
 *
 * baseURL order:
 * - Vite: import.meta.env.VITE_API_BASE_URL
 * - fallback: /api
 */
const baseURL = import.meta.env.VITE_API_BASE_URL ?? "/api";
const httpClient = axios.create({
  baseURL,
  timeout: 15000,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

// Interceptors are configured in setupInterceptors.ts to enable DI with the store.

export default httpClient;
