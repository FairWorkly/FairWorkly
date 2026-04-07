# ── Stage 1: dependency export ────────────────────────────────────────────────
# Poetry is only used here to export a plain requirements.txt.
# It never touches the final image.
FROM python:3.11.12-slim AS builder

WORKDIR /build

# Install Poetry — must match the version that generated poetry.lock (2.3.2).
# poetry-plugin-export is required separately in Poetry 2.x (no longer bundled).
RUN pip install --no-cache-dir "poetry==2.3.2" poetry-plugin-export

# Copy lock files only — changes here invalidate only the export step
COPY pyproject.toml poetry.lock ./

# Export production deps to a plain requirements.txt (no Poetry at runtime)
RUN poetry export \
        --without-hashes \
        --only main \
        --format requirements.txt \
        --output requirements.txt


# ── Stage 2: production image ──────────────────────────────────────────────────
FROM python:3.11.12-slim AS runtime

WORKDIR /app

ENV PYTHONUNBUFFERED=1 \
    PYTHONDONTWRITEBYTECODE=1 \
    PIP_NO_CACHE_DIR=1 \
    PIP_DISABLE_PIP_VERSION_CHECK=1

# Copy only the exported requirements — no Poetry, no cache
COPY --from=builder /build/requirements.txt ./requirements.txt

# Install all production dependencies with no cache written to disk
RUN pip install --no-cache-dir -r requirements.txt

# Copy application source (after deps so this layer is rebuilt less often)
COPY . .

RUN adduser --disabled-password --gecos "" appuser
USER appuser

EXPOSE 8000

CMD ["uvicorn", "master_agent.main:app", "--host", "0.0.0.0", "--port", "8000"]
