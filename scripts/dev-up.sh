#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
LOG_DIR="$ROOT_DIR/.dev/logs"
PID_DIR="$ROOT_DIR/.dev/pids"

mkdir -p "$LOG_DIR" "$PID_DIR"

is_port_listening() {
  local port="$1"
  lsof -nP -iTCP:"$port" -sTCP:LISTEN >/dev/null 2>&1
}

wait_for_port() {
  local port="$1"
  local timeout_seconds="$2"
  local elapsed=0
  while [[ "$elapsed" -lt "$timeout_seconds" ]]; do
    if is_port_listening "$port"; then
      return 0
    fi
    sleep 1
    elapsed=$((elapsed + 1))
  done
  return 1
}

require_env() {
  local key="$1"
  local value="${!key-}"
  if [[ -z "$value" ]]; then
    echo "[FAIL] missing required shell env var: $key"
    return 1
  fi
  return 0
}

start_service() {
  local name="$1"
  local port="$2"
  local workdir="$3"
  local command="$4"
  local log_file="$LOG_DIR/${name}.log"
  local pid_file="$PID_DIR/${name}.pid"

  if is_port_listening "$port"; then
    echo "[SKIP] $name already listening on :$port"
    return 0
  fi

  echo "[RUN]  starting $name on :$port"
  (
    cd "$workdir"
    nohup bash -lc "$command" >"$log_file" 2>&1 &
    echo $! >"$pid_file"
  )

  if wait_for_port "$port" 60; then
    echo "[OK]   $name started on :$port (log: $log_file)"
  else
    echo "[FAIL] $name failed to listen on :$port within 60s"
    echo "------ tail $log_file ------"
    tail -n 60 "$log_file" || true
    exit 1
  fi
}

echo "== FairWorkly Dev Up =="

require_env "OPENAI_API_KEY"
require_env "AGENT_SERVICE_KEY"

BACKEND_SERVICE_KEY="${AiSettings__ServiceKey-}"
if [[ -z "$BACKEND_SERVICE_KEY" ]]; then
  BACKEND_SERVICE_KEY="${AISETTINGS__SERVICEKEY-}"
fi
if [[ -z "$BACKEND_SERVICE_KEY" ]]; then
  echo "[FAIL] missing required shell env var: AiSettings__ServiceKey"
  echo "       (or AISETTINGS__SERVICEKEY)"
  exit 1
fi

if [[ "$BACKEND_SERVICE_KEY" != "${AGENT_SERVICE_KEY-}" ]]; then
  echo "[FAIL] AiSettings__ServiceKey must match AGENT_SERVICE_KEY"
  exit 1
fi

"$ROOT_DIR/scripts/doctor.sh" --config-only

if command -v pgrep >/dev/null 2>&1; then
  uvicorn_count="$( (pgrep -f "uvicorn master_agent.main:app" 2>/dev/null || true) | wc -l | tr -d ' ' )"
  module_count="$( (pgrep -f "python -m master_agent.main" 2>/dev/null || true) | wc -l | tr -d ' ' )"
  if [[ "$uvicorn_count" -gt 0 && "$module_count" -gt 0 ]]; then
    echo "[FAIL] Conflicting agent process types detected."
    echo "       Stop one runtime mode first (recommended: keep uvicorn --reload only)."
    exit 1
  fi
fi

start_service \
  "agent" \
  "8000" \
  "$ROOT_DIR/agent-service" \
  "exec .venv/bin/uvicorn master_agent.main:app --host 0.0.0.0 --port 8000 --reload"

start_service \
  "backend" \
  "5680" \
  "$ROOT_DIR/backend" \
  "exec dotnet run --project src/FairWorkly.API --no-restore"

start_service \
  "frontend" \
  "5173" \
  "$ROOT_DIR/frontend" \
  "exec npm run dev -- --host localhost --port 5173"

echo
if ! "$ROOT_DIR/scripts/doctor.sh"; then
  echo "Doctor reported issues after startup."
  exit 1
fi

echo
echo "All services are up."
echo "To stop: make dev-down"
