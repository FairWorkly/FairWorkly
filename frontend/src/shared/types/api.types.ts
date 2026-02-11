import type { AxiosError } from "axios";

/**
 * 全项目统一的 API 错误模型
 * - 让 UI / hooks 不需要理解 axios 的内部结构
 */
export interface ApiError {
  status?: number;
  message: string;
  code?: string;
  details?: unknown;
  raw?: unknown; // 保留原始错误，方便调试
}

/**
 * 约定后端可能返回的错误结构（可按后端实际格式调整）
 * Supports both ProblemDetails (RFC 7807) and legacy { message } format.
 */
export interface BackendErrorShape {
  message?: string;
  detail?: string; // ProblemDetails format
  title?: string; // ProblemDetails title
  code?: string;
  details?: unknown;
  errors?: Record<string, string[]>; // ProblemDetails validation errors
}

/**
 * 类型守卫：判断是否 axios error
 */
export function isAxiosError(error: unknown): error is AxiosError {
  return typeof error === "object" && error !== null && "isAxiosError" in error;
}

/**
 * 把未知错误统一转换成 ApiError
 */
export function normalizeApiError(error: unknown): ApiError {
  // Axios 错误
  if (isAxiosError(error)) {
    const status = error.response?.status;
    const data = error.response?.data as BackendErrorShape | undefined;

    // ProblemDetails uses 'detail', legacy format uses 'message'
    const message =
      data?.detail ||
      data?.message ||
      error.message ||
      "Request failed. Please try again.";

    return {
      status,
      message,
      code: data?.code,
      details: data?.details ?? data?.errors,
      raw: error,
    };
  }

  // 原生 Error
  if (error instanceof Error) {
    return {
      message: error.message || "Unexpected error.",
      raw: error,
    };
  }

  // 兜底
  return {
    message: "Network error. Please try again.",
    raw: error,
  };
}