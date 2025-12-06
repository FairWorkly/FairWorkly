import {
  useMutation,
  type UseMutationOptions,
} from "@tanstack/react-query";
import type { ApiError } from "../types/api.types";

/**
 * 统一 Mutation 包装
 * - 这里先不做强制 toast
 * - 后面接 notificationsSlice 再加
 */
export function useApiMutation<TData, TVariables = void>(
  options: UseMutationOptions<TData, ApiError, TVariables>,
) {
  return useMutation<TData, ApiError, TVariables>({
    ...options,
  });
}