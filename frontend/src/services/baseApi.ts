import type { AxiosRequestConfig } from "axios";
import httpClient from "./httpClient";
import { normalizeApiError } from "../shared/types/api.types";

/**
 * 基础 API 工具层
 * - 统一 try/catch + 错误格式
 * - 提供强类型 get/post/put/del
 */
export async function get<TResponse>(
  url: string,
  config?: AxiosRequestConfig,
): Promise<TResponse> {
  try {
    const res = await httpClient.get<TResponse>(url, config);
    return res.data;
  } catch (err) {
    throw normalizeApiError(err);
  }
}

export async function post<TResponse, TBody = unknown>(
  url: string,
  body?: TBody,
  config?: AxiosRequestConfig,
): Promise<TResponse> {
  try {
    const res = await httpClient.post<TResponse>(url, body, config);
    return res.data;
  } catch (err) {
    throw normalizeApiError(err);
  }
}

export async function put<TResponse, TBody = unknown>(
  url: string,
  body?: TBody,
  config?: AxiosRequestConfig,
): Promise<TResponse> {
  try {
    const res = await httpClient.put<TResponse>(url, body, config);
    return res.data;
  } catch (err) {
    throw normalizeApiError(err);
  }
}

export async function del<TResponse>(
  url: string,
  config?: AxiosRequestConfig,
): Promise<TResponse> {
  try {
    const res = await httpClient.delete<TResponse>(url, config);
    return res.data;
  } catch (err) {
    throw normalizeApiError(err);
  }
}