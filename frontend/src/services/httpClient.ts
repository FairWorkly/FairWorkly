import axios from "axios";

/**
 * 统一 HTTP 客户端
 * - 所有模块 API 都从这里发请求
 *
 * baseURL 优先读取环境变量
 * - Vite: import.meta.env.VITE_API_BASE_URL
 * - 默认 /api
 */
const baseURL = import.meta.env.VITE_API_BASE_URL ?? "/api";
const httpClient = axios.create({
  baseURL,
  timeout: 15000,
  headers: {
    "Content-Type": "application/json",
  },
});

// ------- Request Interceptor (Auth Placeholder) -------
httpClient.interceptors.request.use((config) => {
  // 简单占位
  // 后续做 auth 模块时再换成更正式的 token 管理
  const token = localStorage.getItem("authToken");

  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// ------- Response Interceptor (Optional lightweight handling) -------
httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    // 这里不做“强行格式化”，交给 baseApi 统一 normalize
    return Promise.reject(error);
  },
);

export default httpClient;