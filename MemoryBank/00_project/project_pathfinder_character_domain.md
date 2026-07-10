# CharacterManagement Project Map

## Назначение

Этот документ описывает текущее состояние bounded context `CharacterManagement`: доменную модель, application/use cases, infrastructure, API и известные ограничения.

Если нужны нормативные правила Pathfinder 2e, читать не этот файл, а [`../20_domain/character_creation/`](../20_domain/character_creation/).

## Роль bounded context

`CharacterManagement` отвечает за пользовательский контур персонажей:

- локальный `Account`, связанный с пользователем из `Secure`;
- создание персонажа в рамках MVP;
- применение ancestry rules;
- хранение draft characters;
- чтение списка/карточки персонажей;
- удаление персонажей.

Граница между `Secure` и `CharacterManagement` сейчас важна: `Secure.User` является источником пользователей, а `CharacterManagement.Account` нужен для работы character API.

## Domain

### `Account`

Локальная модель аккаунта в `CharacterManagement`.

Связь:

- `Secure.User.Id` -> `CharacterManagement.Account.UserId`

Текущий риск:

- исторически или в seed-данных может существовать пользователь без `CharacterManagement.Account`.
- актуальный блок работ по этому месту: Vikunja `#32`, [`../30_task_notes/task_32_account_backfill.md`](../30_task_notes/task_32_account_backfill.md).

### `DraftCharacter`

Агрегат персонажа.

Содержит:

- владельца через `Account`;
- имя;
- выбранный `AncestryType`;
- итоговые `AbilityScores`;
- применённые free boosts.

Основные сценарии:

- создание;
- переименование;
- установка ancestry;
- применение/сброс free boosts;
- проверка инвариантов.

### `AbilityScores`

Value object для шести характеристик:

- `Strength`
- `Dexterity`
- `Constitution`
- `Intelligence`
- `Wisdom`
- `Charisma`

Правила:

- базовое значение каждой характеристики — `10`;
- boost применяет `+2`;
- flaw применяет `-2`;
- modifier вычисляется в `Characteristic`, а не хранится отдельно.

### `Ancestry`

Справочная доменная модель ancestry.

Текущие поля:

- `AncestryType`
- `AbilityBoosts`
- `AbilityFlaws`
- `BaseHitPoints`
- `Size`
- `BaseSpeed`
- `Darkvision`
- `LowLightVision`

Важно: текущая модель покрывает MVP, но ещё не хранит все remastered-поля вроде languages, granted items и granted rules.

## Application

### Создание и чтение персонажей

Актуальные use cases:

- `CreateCharacterCommand` / `CreateCharacterHandler`
- `GetCharactersCommand` / `GetCharactersHandler`
- `GetCharacterByIdCommand` / `GetCharacterByIdHandler`
- `DeleteCharacterCommand` / `DeleteCharacterHandler`

Создание персонажа идёт через `CharacterBuilder`:

1. найти `Account` по `UserId`;
2. создать `DraftCharacter`;
3. применить ancestry;
4. применить free boosts;
5. добавить персонажа в `Account`;
6. сохранить через `IUnitOfWork`.

### Ancestry catalog

Актуальные use cases:

- `GetAncestriesCommand`
- `GetAncestriesHandler`

Данные берутся из `IAncestryRepository` и мапятся в `AncestryDto`.

### Account lifecycle

Актуальные use cases:

- `CreateNewAccountCommand` / `CreateNewAccountHandler`
- `EnsureAccountExistsCommand` / `EnsureAccountExistsHandler`

`CreateNewAccountHandler` используется event flow регистрации. `EnsureAccountExistsHandler` поддерживает idempotent-сценарий создания аккаунта, но полноценный административный backfill ещё вынесен в задачу `#32`.

### Что сознательно не реализовано в MVP

`CharacterBuilder` содержит заглушечные методы для будущих этапов:

- `SetBackground`
- `SetClass`
- `SetInventory`
- `SetAlignment`
- `SetDeity`
- `SetAge`
- `SetGender`

Это не баг текущего MVP: `Background`, `Class`, skills, spells, equipment и narrative fields находятся вне текущего backend MVP.

## Infrastructure

### EF Core

`CharacterManagementDbContext` содержит:

- `Account`;
- `DraftCharacter`;
- owned `AbilityScores`;
- jsonb mapping для `AppliedFreeBoosts`;
- schema `character_management`.

Миграции лежат в `CharacterManagement.Infrastructure/Migrations`.

Для migrations читать [`../10_workflow/ef.md`](../10_workflow/ef.md).

### Repositories

Актуальные repositories:

- `AccountRepository`
- `CharacterRepository`
- `AncestryRepository`

`AncestryRepository` сейчас является hardcoded MVP-каталогом шести ancestry.

## Web API

### `AncestriesController`

Route:

- `GET /api/ancestries`

Возвращает ancestry-каталог для текущего authenticated user.

### `CharacterController`

Route:

- `GET /api/character`
- `GET /api/character/{characterId}`
- `POST /api/character`
- `DELETE /api/character/{characterId}`

Контроллер:

- берёт `UserId` из `ClaimTypes.NameIdentifier`;
- вызывает MediatR use cases;
- мапит validation/domain/data errors в HTTP responses.

В контроллере всё ещё есть legacy item endpoints:

- `GET /api/character/items`
- `DELETE /api/character/items/drop`

Их не считать частью Character Creation MVP.

## Tests

Текущие test projects:

- `CharacterManagement.Domain.Tests`
- `CharacterManagement.Infrastructure.Tests`

Покрытые зоны:

- domain rules для ancestry/free boosts;
- command validators;
- create/get/delete character use cases;
- ancestry handler;
- account creation scenarios;
- controller scenarios.

## Известные gaps

- Frontend wizard создания персонажа ещё не реализован.
- `Background` и `Class` не оформлены как C#-каталоги.
- `Ancestry` модель пока не хранит languages, granted items, granted rules.
- Есть расхождение между текущей MVP-моделью vision flags и remastered ancestry facts.
- Нужно закрыть `Secure.User` -> `CharacterManagement.Account` backfill и уникальность `Account.UserId`.

Подробные доменные gaps по Character Creation должны быть вынесены в `../20_domain/character_creation/known_gaps.md` на следующем этапе задачи `#40`.

## Куда смотреть дальше

- [`project_overview.md`](project_overview.md) — общий обзор проекта.
- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md) — обязательные правила MVP.
- [`../30_task_notes/mvp_character_creation_backend.md`](../30_task_notes/mvp_character_creation_backend.md) — статус backend MVP.
- [`../30_task_notes/mvp_character_creation_frontend.md`](../30_task_notes/mvp_character_creation_frontend.md) — статус frontend MVP.
- [`../30_task_notes/task_32_account_backfill.md`](../30_task_notes/task_32_account_backfill.md) — текущий backend фокус.

