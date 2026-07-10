# Task 32 Account Backfill

## Цель

Устранить расхождения между `Secure.User` и `CharacterManagement.Account`: добавить административный backfill, который создаёт отсутствующие `Account` для существующих и seed-пользователей.

## Что готово

- Scope и HTTP-контракт зафиксированы в Vikunja `#32`.
- Задача декомпозирована на подзадачи по admin boundary, service/endpoint, uniqueness migration, logging/audit, seed data и tests.
- `project_overview.md` указывает этот блок как текущий backend-фокус.

## Что не готово

- Parent task `#32` и дочерние задачи `#34`-`#39` остаются открытыми.
- Backfill service и endpoint ещё нужно реализовать.
- Нужно обеспечить уникальность `Account.UserId` на уровне БД.
- Нужно добавить logging/audit и тесты.
- Нужно исправить seed data `Name` / `Surname`.

## Связанные файлы

- [`../00_project/project_overview.md`](../00_project/project_overview.md)
- [`../10_workflow/vikunja.md`](../10_workflow/vikunja.md)
- `../../CharacterManagement.Domain/Entity/Account.cs`
- `../../CharacterManagement.Infrastructure/Data/CharacterManagementDbContext.cs`
- `../../Secure.Domain/`
- `../../Secure.Infrastructure/`

## Next steps

1. Начинать работу с актуальной карточки Vikunja `#32` и её дочерних задач.
2. Перед C#-правками читать [`../10_workflow/feedback_csharp_style.md`](../10_workflow/feedback_csharp_style.md).
3. Для миграций читать [`../10_workflow/ef.md`](../10_workflow/ef.md).
4. После реализации обновить эту заметку и соответствующие карточки Vikunja.

