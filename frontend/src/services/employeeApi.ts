import { post } from "./baseApi";

/**
 * Employee 模块 API 骨架
 * 后续会补：
 * - leave balance query
 * - employment type lookup
 * - policy summary
 */

export interface EmployeePlaceholderRequest {
  ping: true;
}

export interface EmployeePlaceholderResponse {
  ok: boolean;
}

export function employeeHealthCheck(): Promise<EmployeePlaceholderResponse> {
  return post<EmployeePlaceholderResponse, EmployeePlaceholderRequest>(
    "/employee/health",
    { ping: true },
  );
}