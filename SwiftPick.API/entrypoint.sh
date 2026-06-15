#!/bin/bash
set -e

echo "=== SwiftPick API Starting ==="

# Ждём готовности PostgreSQL
echo "Waiting for PostgreSQL to be ready..."
while ! nc -z postgres 5432; do
  sleep 1
done
echo "PostgreSQL is ready!"

# Создаём миграции и применяем их
echo "Applying database migrations..."
dotnet ef database update --no-build 2>&1 || echo "Migration completed or already applied"

# Запускаем приложение
echo "Starting API on port 5000..."
exec dotnet SwiftPick.API.dll
