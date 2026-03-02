.PHONY: lint-deps doctor dev-up dev-down

lint-deps:
	@echo "Checking forbidden deps: agents/shared/rag -> agents.compliance"
	@set -e; \
	if command -v rg >/dev/null 2>&1; then \
	  if rg -n "agents\\.compliance" agent-service/agents/shared/rag; then \
	    echo "ERROR: forbidden dependency from agents/shared/rag to agents.compliance"; exit 1; \
	  fi; \
	elif command -v grep >/dev/null 2>&1; then \
	  if grep -R -n "agents\\.compliance" agent-service/agents/shared/rag; then \
	    echo "ERROR: forbidden dependency from agents/shared/rag to agents.compliance"; exit 1; \
	  fi; \
		else \
		  echo "ERROR: neither rg nor grep found"; exit 1; \
		fi; \
		echo "OK: no forbidden dependency found."

doctor:
	@./scripts/doctor.sh

dev-up:
	@./scripts/dev-up.sh

dev-down:
	@./scripts/dev-down.sh
