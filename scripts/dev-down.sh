#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PID_DIR="$ROOT_DIR/.dev/pids"

stop_from_pid_file() {
  local name="$1"
  local pid_file="$PID_DIR/${name}.pid"

  if [[ ! -f "$pid_file" ]]; then
    echo "[SKIP] $name pid file not found"
    return 0
  fi

  local pid
  pid="$(cat "$pid_file" 2>/dev/null || true)"
  if [[ -z "$pid" ]]; then
    echo "[WARN] $name pid file is empty, removing stale file"
    rm -f "$pid_file"
    return 0
  fi

  if ! kill -0 "$pid" >/dev/null 2>&1; then
    echo "[SKIP] $name pid $pid is not running, removing stale file"
    rm -f "$pid_file"
    return 0
  fi

  echo "[RUN]  stopping $name (pid=$pid)"
  kill -TERM "$pid" >/dev/null 2>&1 || true

  for _ in $(seq 1 15); do
    if ! kill -0 "$pid" >/dev/null 2>&1; then
      break
    fi
    sleep 1
  done

  if kill -0 "$pid" >/dev/null 2>&1; then
    echo "[WARN] $name did not exit after TERM, sending KILL"
    kill -KILL "$pid" >/dev/null 2>&1 || true
  fi

  rm -f "$pid_file"
  echo "[OK]   $name stopped"
}

echo "== FairWorkly Dev Down =="

stop_from_pid_file "frontend"
stop_from_pid_file "backend"
stop_from_pid_file "agent"

echo
echo "Current listeners (5173/5680/8000):"
lsof -nP -iTCP:5173 -iTCP:5680 -iTCP:8000 -sTCP:LISTEN 2>/dev/null || echo "none"
