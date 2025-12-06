import {
  useQuery,
  type UseQueryOptions,
  type QueryKey,
} from "@tanstack/react-query";
import type { ApiError } from "../types/api.types";

/**
 * 统一 Query 包装
 * - 默认只对“可能是临时问题”的错误轻度重试
 */
function defaultRetryCount(failureCount: number, error: ApiError) {
  const status = error.status;

  // 4xx 一般不重试
  if (status && status >= 400 && status < 500) return false;

  // 网络/5xx 允许小幅重试
  return failureCount < 1;
}

export function useApiQuery<TData, TQueryKey extends QueryKey = QueryKey>(
  options: UseQueryOptions<TData, ApiError, TData, TQueryKey>,
) {
  return useQuery<TData, ApiError, TData, TQueryKey>({
    staleTime: 30_000,
    ...options,
    retry: (failureCount, error) => defaultRetryCount(failureCount, error),
  });
}