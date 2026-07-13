# CharacterManagement Project Map

## Назначение

`CharacterManagement` — bounded context пользовательских персонажей. Этот документ описывает текущую инженерную карту реализации; нормативные правила Pathfinder 2e находятся в [`../20_domain/character_creation/`](../20_domain/character_creation/).

## Граница контекста

- `Secure.User` — источник пользователя и авторизации.
- `CharacterManagement.Account` — локальная запись, связывающая пользователя с его персонажами.
- `DraftCharacter` — aggregate root создаваемого персонажа.

Связь `Secure.User.Id -> CharacterManagement.Account.UserId` должна быть уникальной. Исторические записи без `Account` обрабатываются отдельной задачей: [`task_32_account_backfill.md`](../30_task_notes/task_32_account_backfill.md).

## Текущий character creation flow

Реализованный draft включает:

- имя и базовые сведения;
- Ancestry, heritage, ancestry feat, boosts/flaws и final free boosts;
- Background: два boosts, trained general skill и Lore;
- Class: key ability boost, base HP и стартовые proficiencies;
- Rogue's Racket;
- Cleric Doctrine и Deity package: divine skill, favored weapon proficiency, Divine Font и sanctification;
- maximum HP первого уровня, вычисляемые из ancestry, class и Constitution modifier.

`AbilityScores` хранит шесть характеристик с базовым значением `10`; boost и flaw изменяют значение на `2`, а modifier вычисляется, а не хранится. Полные правила и границы незавершённых подсистем описаны в [`../20_domain/character_creation/README.md`](../20_domain/character_creation/README.md).

## Application и infrastructure

Слои `Domain`, `Application` и `Infrastructure` содержат aggregate, use cases/DTO/validators и EF Core persistence соответственно. `CharacterManagementDbContext` использует схему `character_management`; миграции расположены в `CharacterManagement.Infrastructure/Migrations`.

Каталоги ancestry, backgrounds, classes, skills, racket, doctrines и deities доступны через application use cases и read-модели. Обновления aggregate, persistence, API и frontend выполняются одним vertical slice.

## Web API

Все endpoints требуют authenticated user.

- `GET /api/ancestries`
- `GET /api/backgrounds`
- `GET /api/classes`
- `GET /api/classes/rogue/rackets`
- `GET /api/classes/cleric/doctrines`
- `GET /api/classes/cleric/deities`
- `GET /api/skills`
- `GET /api/character`
- `GET /api/character/{characterId}`
- `POST /api/character`
- `DELETE /api/character/{characterId}`

Legacy item endpoints в `CharacterController` не относятся к текущему character creation flow.

## Tests

- `CharacterManagement.Domain.Tests` — инварианты aggregate и доменные правила.
- `CharacterManagement.Infrastructure.Tests` — persistence, handlers и API-интеграция.
- `pathfinder.frontend` — Vitest-покрытие wizard и отображения.

## Открытые границы

- skill feats, дополнительные class skills и progression proficiency;
- domain choice, spell preparation и Divine Font slots;
- spells, class features, equipment, languages и исполняемые ancestry effects;
- расширенные derived statistics и состояние current/temporary HP;
- remastered-поля ancestry: languages, granted items и granted rules.

Подробный список — в [`../20_domain/character_creation/known_gaps.md`](../20_domain/character_creation/known_gaps.md), а порядок последующих задач — в [`../30_task_notes/character_creation_near_term_roadmap.md`](../30_task_notes/character_creation_near_term_roadmap.md).
