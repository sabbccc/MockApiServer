# -----------------------------
# Runtime base
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# -----------------------------
# Build stage
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY MockApiServer.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish --no-restore

# -----------------------------
# Final image
# -----------------------------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Run as non-root
RUN addgroup --system appgroup && adduser --system appuser --ingroup appgroup
RUN mkdir -p /app/logs && chown -R appuser:appgroup /app/logs

USER appuser
ENTRYPOINT ["dotnet", "MockApiServer.dll"]
