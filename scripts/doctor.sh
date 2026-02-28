#!/usr/bin/env bash
set -u

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CONFIG_ONLY=0
if [[ "${1:-}" == "--config-only" ]]; then
  CONFIG_ONLY=1
fi

FAILURES=0
WARNINGS=0

ok() {
  echo "[OK]   $1"
}

warn() {
  WARNINGS=$((WARNINGS + 1))
  echo "[WARN] $1"
}

fail() {
  FAILURES=$((FAILURES + 1))
  echo "[FAIL] $1"
}

require_cmd() {
  local cmd="$1"
  if command -v "$cmd" >/dev/null 2>&1; then
    ok "command '$cmd' found"
  else
    fail "command '$cmd' is missing"
  fi
}

read_env_value() {
  local file="$1"
  local key="$2"
  if [[ ! -f "$file" ]]; then
    echo ""
    return 0
  fi

  local line
  if command -v rg >/dev/null 2>&1; then
    line="$(rg -n "^${key}=" "$file" | tail -n 1 | sed -E 's/^[0-9]+://')"
  else
    line="$(grep -E "^${key}=" "$file" | tail -n 1)"
  fi

  if [[ -z "${line:-}" ]]; then
    echo ""
  else
    echo "${line#*=}"
  fi
}

read_json_value() {
  local file="$1"
  local path="$2"
  python3 - "$file" "$path" <<'PY'
import json
import sys
from pathlib import Path

file_path = Path(sys.argv[1])
path = sys.argv[2].split(".")
if not file_path.exists():
    print("")
    raise SystemExit(0)

try:
    data = json.loads(file_path.read_text(encoding="utf-8"))
except Exception:
    print("")
    raise SystemExit(0)

node = data
for key in path:
    if not isinstance(node, dict) or key not in node:
        print("")
        raise SystemExit(0)
    node = node[key]

if node is None:
    print("")
elif isinstance(node, (dict, list)):
    print(json.dumps(node))
else:
    print(str(node))
PY
}

is_port_listening() {
  local port="$1"
  lsof -nP -iTCP:"$port" -sTCP:LISTEN >/dev/null 2>&1
}

print_port_listeners() {
  local port="$1"
  lsof -nP -iTCP:"$port" -sTCP:LISTEN 2>/dev/null | tail -n +2
}

echo "== FairWorkly Doctor =="

require_cmd "python3"
require_cmd "lsof"
require_cmd "curl"
require_cmd "dotnet"
require_cmd "node"

FRONTEND_ENV="$ROOT_DIR/frontend/.env"
AGENT_ENV="$ROOT_DIR/agent-service/.env"
BACKEND_SETTINGS="$ROOT_DIR/backend/src/FairWorkly.API/appsettings.Development.json"

if [[ -f "$FRONTEND_ENV" ]]; then
  ok "frontend env exists: frontend/.env"
else
  fail "missing frontend/.env"
fi

if [[ -f "$AGENT_ENV" ]]; then
  ok "agent env exists: agent-service/.env (optional for non-secret config)"
else
  warn "agent-service/.env not found (ok if using shell env vars only)"
fi

if [[ -f "$BACKEND_SETTINGS" ]]; then
  ok "backend settings exists: backend/src/FairWorkly.API/appsettings.Development.json"
else
  fail "missing backend/src/FairWorkly.API/appsettings.Development.json"
fi

FRONTEND_API_BASE="$(read_env_value "$FRONTEND_ENV" "VITE_API_BASE_URL")"
BACKEND_AI_BASE_FILE="$(read_json_value "$BACKEND_SETTINGS" "AiSettings.BaseUrl")"
BACKEND_AI_BASE_ENV="${AiSettings__BaseUrl:-${AISETTINGS__BASEURL:-}}"
BACKEND_AI_BASE="${BACKEND_AI_BASE_ENV:-$BACKEND_AI_BASE_FILE}"

BACKEND_AI_KEY_FILE="$(read_json_value "$BACKEND_SETTINGS" "AiSettings.ServiceKey")"
BACKEND_AI_KEY_ENV="${AiSettings__ServiceKey:-${AISETTINGS__SERVICEKEY:-}}"
BACKEND_AI_KEY="${BACKEND_AI_KEY_ENV:-$BACKEND_AI_KEY_FILE}"

AGENT_SERVICE_KEY_FILE="$(read_env_value "$AGENT_ENV" "AGENT_SERVICE_KEY")"
AGENT_SERVICE_KEY_EFFECTIVE="${AGENT_SERVICE_KEY:-$AGENT_SERVICE_KEY_FILE}"

OPENAI_API_KEY_FILE="$(read_env_value "$AGENT_ENV" "OPENAI_API_KEY")"
OPENAI_API_KEY_EFFECTIVE="${OPENAI_API_KEY:-$OPENAI_API_KEY_FILE}"

if [[ "$FRONTEND_API_BASE" == "http://localhost:5680/api" || "$FRONTEND_API_BASE" == "/api" ]]; then
  ok "frontend VITE_API_BASE_URL=$FRONTEND_API_BASE"
elif [[ -z "$FRONTEND_API_BASE" ]]; then
  fail "frontend VITE_API_BASE_URL is missing"
else
  warn "frontend VITE_API_BASE_URL=$FRONTEND_API_BASE (expected http://localhost:5680/api or /api)"
fi

if [[ "$BACKEND_AI_BASE" == "http://localhost:8000" ]]; then
  ok "backend AiSettings:BaseUrl=$BACKEND_AI_BASE"
elif [[ -z "$BACKEND_AI_BASE" ]]; then
  fail "backend AiSettings:BaseUrl is missing"
else
  warn "backend AiSettings:BaseUrl=$BACKEND_AI_BASE (expected http://localhost:8000 for local dev)"
fi

if [[ -z "$BACKEND_AI_KEY" ]]; then
  fail "backend AiSettings:ServiceKey is missing"
elif [[ -n "$BACKEND_AI_KEY_ENV" ]]; then
  ok "backend AiSettings:ServiceKey is set from shell env"
else
  fail "backend AiSettings:ServiceKey loaded from appsettings.Development.json; set shell env AiSettings__ServiceKey instead"
fi

if [[ -z "$AGENT_SERVICE_KEY_EFFECTIVE" ]]; then
  fail "agent AGENT_SERVICE_KEY is missing"
elif [[ -n "${AGENT_SERVICE_KEY:-}" ]]; then
  ok "agent AGENT_SERVICE_KEY is set from shell env"
elif [[ -n "$AGENT_SERVICE_KEY_FILE" ]]; then
  fail "agent AGENT_SERVICE_KEY loaded from agent-service/.env; set shell env AGENT_SERVICE_KEY instead"
else
  fail "agent AGENT_SERVICE_KEY is missing"
fi

if [[ -n "$BACKEND_AI_KEY" && -n "$AGENT_SERVICE_KEY_EFFECTIVE" ]]; then
  if [[ "$BACKEND_AI_KEY" == "$AGENT_SERVICE_KEY_EFFECTIVE" ]]; then
    ok "backend and agent service keys match"
  else
    fail "backend AiSettings:ServiceKey does not match agent AGENT_SERVICE_KEY"
  fi
fi

if [[ -z "$OPENAI_API_KEY_EFFECTIVE" ]]; then
  fail "OPENAI_API_KEY is missing (required for online LLM mode)"
elif [[ -n "${OPENAI_API_KEY:-}" ]]; then
  ok "OPENAI_API_KEY is set from shell env"
elif [[ -n "$OPENAI_API_KEY_FILE" ]]; then
  fail "OPENAI_API_KEY loaded from agent-service/.env; set shell env OPENAI_API_KEY instead"
else
  fail "OPENAI_API_KEY is missing"
fi

if [[ "$AGENT_SERVICE_KEY_EFFECTIVE" == change-me-* || "$BACKEND_AI_KEY" == change-me-* ]]; then
  warn "using placeholder service key; replace with a unique local value"
fi

if [[ -n "$AGENT_SERVICE_KEY_FILE" ]]; then
  warn "agent-service/.env contains AGENT_SERVICE_KEY; remove it to avoid local secret leakage"
fi

if [[ -n "$OPENAI_API_KEY_FILE" ]]; then
  warn "agent-service/.env contains OPENAI_API_KEY; remove it to avoid local secret leakage"
fi

if command -v pgrep >/dev/null 2>&1; then
  UVICORN_COUNT="$(pgrep -f "uvicorn master_agent.main:app" 2>/dev/null | wc -l | tr -d ' ')"
  MODULE_COUNT="$(pgrep -f "python -m master_agent.main" 2>/dev/null | wc -l | tr -d ' ')"
  if [[ "$UVICORN_COUNT" -gt 0 && "$MODULE_COUNT" -gt 0 ]]; then
    fail "conflicting agent runtimes detected (uvicorn and python -m master_agent.main)"
  else
    ok "agent runtime mode is consistent"
  fi
else
  warn "pgrep is unavailable; skipped agent duplicate-process check"
fi

if [[ "$CONFIG_ONLY" -eq 0 ]]; then
  for entry in "frontend:5173" "backend:5680" "agent:8000"; do
    service="${entry%%:*}"
    port="${entry##*:}"
    if is_port_listening "$port"; then
      ok "$service is listening on :$port"
      listeners="$(print_port_listeners "$port")"
      if [[ -n "$listeners" ]]; then
        echo "$listeners" | sed 's/^/       /'
      fi
    else
      fail "$service is not listening on :$port"
    fi
  done

  if is_port_listening 8000; then
    agent_code="$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8000/health || true)"
    if [[ "$agent_code" == "200" ]]; then
      ok "agent /health responded 200"
    else
      fail "agent /health returned HTTP $agent_code"
    fi
  fi

  if is_port_listening 5680; then
    backend_code="$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5680/swagger/index.html || true)"
    if [[ "$backend_code" == "200" ]]; then
      ok "backend swagger endpoint responded 200"
    else
      warn "backend swagger endpoint returned HTTP $backend_code"
    fi
  fi
fi

echo
echo "Summary: failures=$FAILURES warnings=$WARNINGS"
if [[ "$FAILURES" -gt 0 ]]; then
  exit 1
fi
exit 0
