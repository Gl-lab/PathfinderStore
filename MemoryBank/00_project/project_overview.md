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

- Vue 3 + TypeScript
- Vite
- Vuetify 3
- Pinia
- Vue Router 4
- Axios
- Vitest

Архитектурный стиль:

- Clean Architecture
- DDD-подход внутри доменных модулей
- bounded contexts для `Secure`, `CharacterManagement`, `Store`

## Карта solution

### Web

- `Pathfinder.Web` — REST API, startup/configuration, controllers, authentication policies.
- `pathfinder.frontend` — Vue 3 + TypeScript SPA, подключается к backend API.

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
- background-каталог и применение background boosts;
- class-каталог, key ability boost и четыре финальных свободных boosts;
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

- проект: `pathfinder.frontend`;
- запуск: `npm run dev`;
- dev URL/port: `:8080`;
- frontend подключается к backend на `:5001`.

Database:

- PostgreSQL database: `Pathfinder`;
- connection strings передаются через user secrets или environment variables;
- ключи: `DB:Secure`, `DB:CharacterManagement`;
- JWT key передаётся через user secrets или environment variable `Authentication__SecurityKey`;
- реальные секреты не должны попадать в tracked configuration files.

Seed users:

- `admin` / `123qwe`
- `memberuser` / `123qwe`

## Текущий статус

### Готово

- `Secure` интегрирован и используется в web-слое.
- `CharacterManagement.Infrastructure` подключён к `Pathfinder.Web`.
- Backend MVP создания персонажа реализован:
  - `GET /api/ancestries`;
  - `GET /api/backgrounds`;
  - `GET /api/classes`;
  - `GET /api/classes/rogue/rackets`;
  - `GET /api/classes/cleric/doctrines`;
  - `GET /api/classes/cleric/deities`;
  - `GET /api/spells` с фильтрами tradition/rank/kind;
  - `GET /api/skills`;
  - `GET /api/character`;
  - `GET /api/character/{id}`;
  - `POST /api/character`;
  - `DELETE /api/character/{id}`;
  - domain/application/infrastructure тесты для ключевых сценариев.

### Текущий character creation focus

Frontend MVP создания персонажа реализован на Vue 3. Текущий flow включает пол и постоянный аватар, полный Ancestry/Background/Class package, обязательные классовые выборы восьми классов Player Core baseline, четыре финальных свободных boosts и полный стартовый spell loadout Cleric, Bard, Druid, Witch и Wizard. Общий Player Core spell catalog фильтруется сервером по tradition, rank и kind; class flows сохраняют repertoire/preparation/spellbook, granted spells, отдельные slots и focus resources. Единый Player Core feat catalog покрывает ancestry, background skill и class feats первого уровня; общий inventory различает selected/granted provenance, обязательные class/skill feat slots валидируются, а поддерживаемые постоянные feat training effects участвуют в Skills/Lore и modifiers. Карточка серверно вычисляет maximum HP, Perception, Fortitude, Reflex, Will, modifiers всех general skills и сохранённых Lore entries с ability/proficiency breakdown. Parameterized/temporary feat choices, runtime spell/action/combat/inventory effects, equipment, progression и остальные combat statistics ожидают отдельные подсистемы.

Смотреть:

- [`../30_task_notes/mvp_character_creation_frontend.md`](../30_task_notes/mvp_character_creation_frontend.md)
- [`../30_task_notes/background_package_implementation.md`](../30_task_notes/background_package_implementation.md)
- [`../30_task_notes/priority_5_final_review.md`](../30_task_notes/priority_5_final_review.md)

## Как работать с этим обзором

Перед разработкой:

1. Проверить актуальную карточку в Vikunja.
2. Прочитать обязательные workflow-файлы из [`../10_workflow/`](../10_workflow/).
3. Если задача касается создания персонажа, свериться с [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md).
4. Если задача касается CharacterManagement backend, дополнительно смотреть [`project_pathfinder_character_domain.md`](project_pathfinder_character_domain.md).
