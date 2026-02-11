# ============================================
# FairWorkly Frontend Dockerfile
# ============================================
# Multi-stage build for development and production

# --------------------------------------------
# Base stage: Install dependencies
# --------------------------------------------
FROM node:20-alpine AS base
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm ci

# --------------------------------------------
# Development stage: Hot reload with Vite
# --------------------------------------------
FROM base AS development
COPY . .
EXPOSE 5173
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]

# --------------------------------------------
# Build stage: Create production build
# --------------------------------------------
FROM base AS build
COPY . .
ARG VITE_API_BASE_URL
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}
RUN npm run build

# --------------------------------------------
# Production stage: Serve with lightweight server
# --------------------------------------------
FROM node:20-alpine AS production
WORKDIR /app
RUN npm install -g serve
COPY --from=build /app/dist ./dist
EXPOSE 5173
CMD ["serve", "-s", "dist", "-l", "5173"]
