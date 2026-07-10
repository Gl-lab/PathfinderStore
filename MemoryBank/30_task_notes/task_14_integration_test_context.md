# Task 14 Integration Test Context

## Статус документа

Historical note. Документ фиксирует состояние задачи Vikunja `#14` на `2026-04-04` и не является текущим планом разработки.

## Задача

- Vikunja task id: `28`
- Identifier: `#14`
- Title: `[Backend #4.5] Интеграционный тест: CreateNewAccountHandler + CharacterManagementDbContext`
- Статус на момент фиксации: `pending` (проверено 2026-04-04)

## Что уже есть в коде

- `CreateNewAccountHandler` реализован, выполняет проверку существующего аккаунта, добавляет `Account`, вызывает `IUnitOfWork.Commit()`.
- `CharacterManagementDbContext` реализован.
- `AccountRepository` реализован.
- CharacterManagement Infrastructure уже подключен к `Pathfinder.Web` через project references и DI.
- В solution уже есть только `CharacterManagement.Domain.Tests`, отдельного integration test проекта пока нет.

## Что отсутствует для #14

- Интеграционные тесты для связки `CreateNewAccountHandler + CharacterManagementDbContext`.
- Подключение `Microsoft.EntityFrameworkCore.InMemory` в тестовом проекте.
- Вспомогательная тестовая реализация `IUnitOfWork` (или явное переиспользование существующего `UnitOfWork`).

## Релевантные файлы

- `CharacterManagement.Application/UseCases/Accounts/CreateNewAccountHandler.cs`
- `CharacterManagement.Infrastructure/Data/CharacterManagementDbContext.cs`
- `CharacterManagement.Infrastructure/Repositories/AccountRepository.cs`
- `Pathfinder.Utils/UnitOfWork/IUnitOfWork.cs`
- `Pathfinder.Web/Utils/UnitOfWork.cs`
- `Pathfinder.Web/Extensions/ServiceCollectionExtensions.cs`
- `Pathfinder.Web/Startup.cs`
- `CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj`
- `Pathfinder.sln`

## Минимальный целевой объем реализации

1. Подготовить тестовый проект для инфраструктурного интеграционного сценария (новый `CharacterManagement.Infrastructure.Tests` или расширение существующего test-проекта).
2. Добавить `EF Core InMemory` и необходимые project references.
3. Реализовать `TestUnitOfWork : IUnitOfWork` в тестовом проекте.
4. Добавить тесты:
   - успешное создание аккаунта;
   - ошибка при повторном создании того же аккаунта (duplicate by `UserId`).
5. Добавить тестовый проект в `Pathfinder.sln` (если выбран вариант с новым проектом).

## Риски и замечания

- `UnitOfWork` находится в `Pathfinder.Web`, что создаёт нежелательную зависимость от web-слоя в тестах; предпочтительнее `TestUnitOfWork`.
- EF InMemory не полностью эквивалентен PostgreSQL, но достаточен для проверки поведения данного handler.
- Контекст `MemoryBank` частично устарел относительно фактического состояния кода (DI уже подключён, некоторые репозитории уже реализованы).
