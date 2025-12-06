import { post } from "./baseApi";

/**
 * Documents 模块 API 骨架
 * 后续会补：
 * - generate offer/contract/warning letter
 * - template upload & diff
 */

export interface DocumentsPlaceholderRequest {
  ping: true;
}

export interface DocumentsPlaceholderResponse {
  ok: boolean;
}

export function documentsHealthCheck(): Promise<DocumentsPlaceholderResponse> {
  return post<DocumentsPlaceholderResponse, DocumentsPlaceholderRequest>(
    "/documents/health",
    { ping: true },
  );
}