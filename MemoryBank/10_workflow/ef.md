# EF Migrations Workflow

## Когда читать

Читать перед созданием, удалением, проверкой или применением EF Core migrations.

## Главное правило

Все миграции выполнять только через `dotnet ef`, без ручной правки migration-файлов.

Если для контекста есть `IDesignTimeDbContextFactory`, предпочитать запуск миграций через infrastructure-проект без `--startup-project`.

## Проверенный workflow для CharacterManagement

1. Проверить, что отдельно собирается `CharacterManagement.Infrastructure`.
2. Запускать миграцию через `CharacterManagement.Infrastructure.csproj`.
3. Использовать `CharacterManagementDbContextFactory`, если он уже реализован.
4. При проблемах доступа или design-time build свериться с [`sandbox.md`](sandbox.md).

## Базовые команды

```powershell
dotnet build CharacterManagement.Infrastructure\CharacterManagement.Infrastructure.csproj --no-restore
dotnet ef migrations add <MigrationName> --project CharacterManagement.Infrastructure\CharacterManagement.Infrastructure.csproj --context CharacterManagementDbContext --no-build
```

## Важные ограничения

- Перед `dotnet ef migrations add` сначала отдельно проверять сборку целевого infrastructure-проекта.
- Не использовать полную сборку solution как критерий готовности миграции, если в solution есть старые несвязанные ошибки в других bounded contexts.
- Ошибки доступа к `obj/*.targets` при `dotnet ef` обычно связаны с sandbox.

