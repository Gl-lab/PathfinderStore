# Task 32 Account Initialization

## Цель

Обеспечить согласованное начальное состояние `Secure.User` и `CharacterManagement.Account` без административного backfill API.

## Что готово

- Vikunja `#32` переформулирована под чистое развёртывание без сохранения исторических dev-данных.
- В `CharacterManagement` добавлены seed-аккаунты для `admin` (`UserId = 1`) и `memberuser` (`UserId = 2`).
- Идентификаторы seed-аккаунтов совпадают с идентификаторами пользователей (`Id = 1/2`), а identity sequence новых аккаунтов начинается с `3`.
- Для `Account.UserId` задан уникальный индекс.
- Создана EF-миграция `SeedDefaultAccountsAndUniqueUserId`.
- Существующий `EnsureAccountExistsHandler` обеспечивает идемпотентное создание аккаунта при регистрации.
- Добавлены межконтекстные тесты начального seed и полного сценария регистрации через Secure, MassTransit и CharacterManagement.

## Принятые ограничения

- Существующие dev-базы нужно пересоздать; backfill исторических данных не поддерживается.
- Backfill API, отдельные logging и audit не требуются.
- Персонажи в начальные данные не добавляются.

## Связанные файлы

- [`../00_project/project_overview.md`](../00_project/project_overview.md)
- [`../10_workflow/vikunja.md`](../10_workflow/vikunja.md)
- `../../CharacterManagement.Domain/Entity/Account.cs`
- `../../CharacterManagement.Infrastructure/Data/CharacterManagementDbContext.cs`
- `../../Secure.Domain/`
- `../../Secure.Infrastructure/`

## Next steps

1. Перед локальной проверкой пересоздать базы `Secure` и `CharacterManagement`.
2. Применить актуальные миграции.
3. Проверить доступ seed-пользователей к CharacterManagement API.
