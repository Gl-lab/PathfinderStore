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
- bounded contexts для `Secure`, `CharacterManagement`, `CampaignManagement`, `ItemCatalog`, `Inventory`, `Commerce`; legacy `Store` отключён

## Карта solution

### Web

- `Pathfinder.Web` — REST API, startup/configuration, controllers, authentication policies и integration adapters между bounded contexts.
- `pathfinder.frontend` — Vue 3 + TypeScript SPA, подключается к backend API.

### CharacterManagement

- `CharacterManagement.Domain` — доменная модель создания и управления персонажами.
- `CharacterManagement.Application` — use cases, DTO, validation, builders, repository contracts.
- `CharacterManagement.Infrastructure` — EF Core context, migrations, repositories, event consumers.
- `CharacterManagement.Domain.Tests` — unit tests доменной логики.
- `CharacterManagement.Infrastructure.Tests` — integration tests для application/infrastructure/API сценариев.

### CampaignManagement

- `CampaignManagement.Domain` — кампании, членство, приглашения, партии, назначения персонажей и корневое хранилище партии.
- `CampaignManagement.Application` — campaign use cases, DTO и межконтекстные порты.
- `CampaignManagement.Infrastructure` — EF Core context, миграции, repositories и адаптеры к `Secure`/`CharacterManagement`.
- `CampaignManagement.Domain.Tests` — unit-тесты доменных инвариантов.
- `CampaignManagement.Infrastructure.Tests` — persistence и application/infrastructure сценарии кампаний.

### ItemCatalog

- `ItemCatalog.Domain` — устойчивые виды предметов, неизменяемые ревизии, типизированные компоненты правил и конфигурации.
- `ItemCatalog.Application` — административные сценарии, проверка области управления и repository contracts.
- `ItemCatalog.Infrastructure` — отдельная EF Core schema `item_catalog`, миграции и repositories.
- `ItemCatalog.Domain.Tests` — unit-тесты инвариантов каталога.
- `ItemCatalog.Infrastructure.Tests` — persistence и application/infrastructure сценарии каталога.

### Inventory

- `Inventory.Domain` — campaign-scoped экземпляры, корневые контейнеры, стопки, расположение, версии, gift/exchange aggregates, ограничения передачи и неизменяемый аудит.
- `Inventory.Application` — use cases дарения, обмена, партийного хранилища и принудительного перемещения ведущим, а также межконтекстные порты авторизации.
- `Inventory.Infrastructure` — отдельная EF Core schema `inventory` и persistence.
- `Inventory.Domain.Tests` — unit-тесты доменных инвариантов экземпляров и контейнеров.
- `Inventory.Infrastructure.Tests` — persistence-тесты Inventory.

### Commerce

- `Commerce.Domain` — campaign-scoped поселения, магазины, ручные предложения, ценовые политики, кошельки, денежный журнал, резервы и сделки.
- `Commerce.Application` — административные и пользовательские торговые сценарии, DTO и межконтекстные порты.
- `Commerce.Infrastructure` — отдельная EF Core schema `commerce`, persistence и адаптеры к `CampaignManagement`, `ItemCatalog` и `Inventory`.
- `Commerce.Domain.Tests` — unit-тесты торговых и денежных инвариантов.
- `Commerce.Infrastructure.Tests` — persistence и application/infrastructure сценарии торговли.

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

### CampaignManagement

Отвечает за кампании, предметные роли `GameMaster`/`Player`, приглашения и членство, партии, назначение контролируемых персонажей, campaign-scoped доступ к карточке и контракт корневого хранилища партии. Глобальные роли `Secure` не подменяют членство в конкретной кампании.

### ItemCatalog

Отвечает за stable item key, последовательность неизменяемых ревизий, global/campaign scope, типизированные attack/armor/shield/equipment/consumption/charge/durability components и неизменяемые конфигурации из ревизии, размера, материала и постоянных улучшений. Ревизии проходят lifecycle `Draft -> Published -> Retired`; глобальными описаниями управляет системный администратор, campaign-описаниями — активный ведущий точной кампании.

### Inventory

Отвечает за физические экземпляры в границе кампании, личные/партийные/магазинные/мировые root containers, стопки, текущее расположение, optimistic version и идемпотентный журнал операций. Реализованы подтверждаемое дарение, резервируемый атомарный обмен, общее хранилище по политике партии, запрет передачи, аудит действий и отдельное принудительное перемещение ведущим. Completed-character starting equipment мигрирует в экземпляры после однозначного назначения кампании; точная конфигурация сохраняет применявшуюся catalog revision.

### Commerce и legacy Store

`Commerce` является целевой реализацией магазинов и ручной торговли. Он хранит поселения, магазины, конечный ручной ассортимент, ценовые политики, кошельки, резервы и снимки завершённых сделок. Физические экземпляры остаются в `Inventory`; описание и базовая цена читаются из `ItemCatalog`; campaign access разрешается через `CampaignManagement`.

Административные команды опубликованы под `api/commerce-admin/campaigns/{campaignId}`, пользовательские резервы, покупки и продажи — под `api/commerce/campaigns/{campaignId}`.

Целевая архитектура зафиксирована в [`../20_domain/store/target_architecture_togaf.md`](../20_domain/store/target_architecture_togaf.md). Legacy-реализации `TeamsController` и `Store.*` не считаются целевой моделью; `Store.*` остаётся отключённым в `Pathfinder.Web`.

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
- ключи: `DB:Secure`, `DB:CharacterManagement`, `DB:CampaignManagement`, `DB:ItemCatalog`, `DB:Inventory`;
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
  - `POST /api/character/{id}/finalize`;
  - `DELETE /api/character/{id}`;
  - `GET|POST /api/campaigns` и `POST /api/campaigns/{id}/archive`;
  - invitation, membership и role endpoints под `/api/campaign-membership`;
  - party и character-assignment endpoints под `/api/campaign-parties`;
  - `GET /api/campaigns/{campaignId}/characters/{characterId}`;
  - `POST /api/campaigns/{campaignId}/characters/{characterId}/hit-points`;
  - `POST /api/item-catalog-admin/drafts`;
  - `POST /api/item-catalog-admin/definitions/{id}/revisions/{revision}/publish`;
  - `POST /api/item-catalog-admin/definitions/{id}/revisions/{revision}/retire`;
  - domain/application/infrastructure тесты для ключевых сценариев.

### Текущий character creation focus

Frontend MVP создания персонажа реализован на Vue 3. Текущий flow включает пол и постоянный аватар, полный Ancestry/Background/Class package, обязательные классовые выборы восьми классов Player Core baseline, четыре финальных свободных boosts, стартовое снаряжение и полный spell loadout Cleric, Bard, Druid, Witch и Wizard. Общий Player Core spell catalog фильтруется сервером по tradition, rank и kind; class flows сохраняют repertoire/preparation/spellbook, granted spells, отдельные slots и focus resources. Единый Player Core feat catalog покрывает ancestry, background skill и class feats первого уровня; starting inventory различает selected/granted provenance, обязательные class/skill feat slots валидируются, а поддерживаемые постоянные feat training effects участвуют в Skills/Lore и modifiers. Боевая карточка серверно вычисляет maximum/current/temporary HP, AC, Strikes, class DC, spell attack/DC, Perception, saves и modifiers Skills/Lore с объяснимыми breakdown. Кампании поддерживают приглашения, контекстные роли, одну активную партию, назначение персонажей и campaign-scoped карточку. Версионируемый `ItemCatalog` является источником описаний и точных конфигураций. Completed-character loadout однозначно назначенной кампании мигрирует в runtime `Inventory`, а production combat adapter читает экипированные экземпляры через точную revision; не назначенные персонажи сохраняют переходный starting fallback. Backend Inventory поддерживает подтверждаемое дарение, атомарный обмен, партийное хранилище и отдельную аудируемую команду ведущего. Backend Commerce поддерживает ручные campaign-scoped магазины, кошельки, резервы, покупку и продажу физических экземпляров через Inventory. Encounter actions/conditions, Raise a Shield, runtime spell/feat effects, progression, transfer/trade UI и read projections остаются отдельными подсистемами.

Смотреть:

- [`../30_task_notes/mvp_character_creation_frontend.md`](../30_task_notes/mvp_character_creation_frontend.md)
- [`../30_task_notes/background_package_implementation.md`](../30_task_notes/background_package_implementation.md)
- [`../30_task_notes/priority_5_final_review.md`](../30_task_notes/priority_5_final_review.md)
- [`../30_task_notes/priority_8_final_review.md`](../30_task_notes/priority_8_final_review.md)
- [`../30_task_notes/priority_9_final_review.md`](../30_task_notes/priority_9_final_review.md)
- [`../30_task_notes/priority_10_final_review.md`](../30_task_notes/priority_10_final_review.md)
- [`../30_task_notes/priority_11_final_review.md`](../30_task_notes/priority_11_final_review.md)
- [`../30_task_notes/priority_12_final_review.md`](../30_task_notes/priority_12_final_review.md)
- [`../30_task_notes/priority_13_final_review.md`](../30_task_notes/priority_13_final_review.md)

## Как работать с этим обзором

Перед разработкой:

1. Проверить актуальную карточку в Vikunja.
2. Прочитать обязательные workflow-файлы из [`../10_workflow/`](../10_workflow/).
3. Если задача касается создания персонажа, свериться с [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md).
4. Если задача касается CharacterManagement backend, дополнительно смотреть [`project_pathfinder_character_domain.md`](project_pathfinder_character_domain.md).
