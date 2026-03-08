FROM python:3.11-slim

WORKDIR /app

ENV POETRY_VIRTUALENVS_CREATE=false \
    POETRY_NO_INTERACTION=1 \
    PYTHONUNBUFFERED=1

RUN pip install --no-cache-dir poetry==1.8.3

COPY pyproject.toml poetry.lock ./
RUN poetry install --no-root

COPY . .

EXPOSE 8000

CMD ["poetry", "run", "uvicorn", "master_agent.main:app", "--host", "0.0.0.0", "--port", "8000"]
