# Pathfinder Project Overview

## Что это

Pathfinder 2e Web App — учебное/хобби веб-приложение для игроков Pathfinder 2e.

Основной продуктовый фокус:

- создание и управление персонажами;
- просмотр карточки персонажа;
- будущие inventory/store сценарии.

Проект строится как ASP.NET Core backend + Vue.js frontend с разделением backend-кода по bounded contexts.

## Технологии

Backend:

- ASP.NET Core 8 / .NET 8
- C# 12
- PostgreSQL
- EF Core 8 + Npgsql
- JWT authentication
- MediatR
- MassTransit

Frontend:

- Vue.js 2
- Vuetify 2
- Vuex
- Vue Router
- Axios

Архитектурный стиль:

- Clean Architecture
- DDD-подход внутри доменных модулей
- bounded contexts для `Secure`, `CharacterManagement`, `Store`

## Карта solution

### Web

- `Pathfinder.Web` — REST API, startup/configuration, controllers, authentication policies.
- `pathfinder.vue` — Vue.js SPA, подключается к backend API.

### CharacterManagement

- `CharacterManagement.Domain` — доменная модель создания и управления персонажами.
- `CharacterManagement.Application` — use cases, DTO, validation, builders, repository contracts.
- `CharacterManagement.Infrastructure` — EF Core context, migrations, repositories, event consumers.
- `CharacterManagement.Domain.Tests` — unit tests доменной логики.
- `CharacterManagement.Infrastructure.Tests` — integration tests для application/infrastructure/API сценариев.

### Secure

- `Secure.Domain` — пользователи, роли, permissions.
- `Secure.Application` — application contracts/use cases.
- `Secure.Infrastructure` — ASP.NET Identity, persistence, seed/auth infrastructure.

### Store

- `Store.Domain`
- `Store.Application`
- `Store.Infrastructure`

Store сейчас не является активным направлением и временно отключён в `Pathfinder.Web`.

### Shared

- `Pathfinder.Utils` — общие интерфейсы и утилиты, включая `IUnitOfWork`, `IAggregateRoot`, `ValueObject`.
- `Infrasturture.Shared` — базовая инфраструктурная реализация репозиториев.
- `Domain.Contracts` — межконтекстные события, например `UserRegisteredEvent`.

## Ключевые доменные области

### Secure

Отвечает за:

- регистрацию и вход пользователей;
- роли и permissions;
- JWT authentication;
- seed-пользователей;
- публикацию событий о регистрации.

Ключевая модель: `User -> Role -> Permission`.

### CharacterManagement

Отвечает за:

- `Account`, связанный с пользователем из `Secure`;
- создание персонажа;
- ancestry-каталог;
- применение ancestry boosts/flaws/free boosts;
- список, карточку и удаление персонажей.

Ключевая модель: `Account -> DraftCharacter -> AbilityScores -> Characteristic`.

Нормативные правила создания персонажа лежат в [`../20_domain/character_creation/`](../20_domain/character_creation/).

### Store

Задуман для магазина и inventory, но сейчас не в приоритете.

Практическое правило: не трогать Store без явной задачи.

## Dev setup

Backend:

- основной проект: `Pathfinder.Web`;
- dev profile: `Pathfinder.Web`;
- dev URL/port: `:5001`.

Frontend:

- проект: `pathfinder.vue`;
- запуск: `npm run serve`;
- dev URL/port: `:8080`;
- frontend подключается к backend на `:5001`.

Database:

- PostgreSQL database: `Pathfinder`;
- connection strings хранятся в user secrets;
- ключи: `DB:Secure`, `DB:CharacterManagement`;
- JWT key хранится в user secrets как `Authentication:SecurityKey`.

Seed users:

- `admin` / `123qwe`
- `memberuser` / `123qwe`

## Текущий статус

### Готово

- `Secure` интегрирован и используется в web-слое.
- `CharacterManagement.Infrastructure` подключён к `Pathfinder.Web`.
- Backend MVP создания персонажа реализован:
  - `GET /api/ancestries`;
  - `GET /api/character`;
  - `GET /api/character/{id}`;
  - `POST /api/character`;
  - `DELETE /api/character/{id}`;
  - domain/application/infrastructure тесты для ключевых сценариев.

### Активный backend-фокус

Согласованное начальное состояние `Secure.User` и `CharacterManagement.Account` обеспечивается seed-аккаунтами для системных пользователей и идемпотентной синхронизацией новых регистраций.

Смотреть:

- Vikunja `#32`
- [`../30_task_notes/task_32_account_backfill.md`](../30_task_notes/task_32_account_backfill.md)

### Активный frontend-фокус

Frontend MVP создания персонажа ещё не реализован.

Смотреть:

- [`../30_task_notes/mvp_character_creation_frontend.md`](../30_task_notes/mvp_character_creation_frontend.md)

## Как работать с этим обзором

Перед разработкой:

1. Проверить актуальную карточку в Vikunja.
2. Прочитать обязательные workflow-файлы из [`../10_workflow/`](../10_workflow/).
3. Если задача касается создания персонажа, свериться с [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md).
4. Если задача касается CharacterManagement backend, дополнительно смотреть [`project_pathfinder_character_domain.md`](project_pathfinder_character_domain.md).
