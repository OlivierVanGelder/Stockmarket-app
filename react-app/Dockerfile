FROM node:slim AS build

RUN corepack enable pnpm && corepack install -g pnpm

COPY . /frontend

WORKDIR /frontend
RUN pnpm i
RUN pnpm build

FROM lipanski/docker-static-website:latest

COPY --from=build /frontend/build .