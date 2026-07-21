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
- Cleric Doctrine, Deity package и primary Domain choice для Cloistered Cleric;
- divine skill, favored weapon proficiency, Divine Font и sanctification;
- Cleric spell loadout первого уровня: 5 cantrips, 2 prepared slots, 4 derived Divine Font slots и domain focus spell с Focus Pool `1`;
- maximum HP первого уровня, вычисляемые из ancestry, class и Constitution modifier;
- Perception, Fortitude, Reflex и Will первого уровня с ability/proficiency breakdown;
- modifiers полного каталога general skills и сохранённых Lore entries, включая Untrained состояние.
- Player Core language catalog, starting и дополнительные языки с server-side count/pool validation;
- единый completion report для обязательных packages, choices, spells, feats, training и languages;
- однонаправленная финализация `Draft` → `Completed` с UTC timestamp и блокировкой build/edit operations после завершения.
- Player Core starting equipment catalog, wealth/class kits, validated persisted loadout и equipped state;
- server-computed equipment cost, weapon/armor proficiency applicability, общая Масса и пороги нагрузки.

`AbilityScores` хранит шесть характеристик с базовым значением `10`; boost и flaw изменяют значение на `2`, а modifier вычисляется, а не хранится. Полные правила и границы незавершённых подсистем описаны в [`../20_domain/character_creation/README.md`](../20_domain/character_creation/README.md).

## Application и infrastructure

Слои `Domain`, `Application` и `Infrastructure` содержат aggregate, use cases/DTO/validators и EF Core persistence соответственно. `CharacterManagementDbContext` использует схему `character_management`; миграции расположены в `CharacterManagement.Infrastructure/Migrations`.

Каталоги ancestry, backgrounds, classes, skills, languages, feats, class choices, domains и tradition-aware spells доступны через application use cases и read-модели. Обновления aggregate, persistence, API и frontend выполняются одним vertical slice.

## Web API

Все endpoints требуют authenticated user.

- `GET /api/ancestries`
- `GET /api/backgrounds`
- `GET /api/classes`
- `GET /api/classes/rogue/rackets`
- `GET /api/classes/cleric/doctrines`
- `GET /api/classes/cleric/deities`
- `GET /api/classes/cleric/domains`
- `GET /api/classes/cleric/spells`
- `GET /api/classes/cleric/spells/available?deityId=...`
- `GET /api/skills`
- `GET /api/languages`
- `GET /api/languages/options?ancestryType=...&intelligenceScore=...`
- `GET /api/equipment`
- `GET /api/equipment/class-kits`
- `GET /api/character`
- `GET /api/character/{characterId}`
- `POST /api/character`
- `POST /api/character/{characterId}/finalize`
- `POST /api/character/{characterId}/hit-points`
- `PUT /api/character/{characterId}/gender`
- `DELETE /api/character/{characterId}`

Пустые legacy item endpoints удалены. Starting equipment catalog и loadout следуют ownership boundary из [`../20_domain/character_creation/equipment_inventory_boundary.md`](../20_domain/character_creation/equipment_inventory_boundary.md); runtime Store/Inventory остаётся отключённым.

## Tests

- `CharacterManagement.Domain.Tests` — инварианты aggregate и доменные правила.
- `CharacterManagement.Infrastructure.Tests` — persistence, handlers и API-интеграция.
- `pathfinder.frontend` — Vitest-покрытие wizard и отображения.

## Открытые границы

- skill feats и progression proficiency;
- spellcasting lifecycle, current spell slots/Focus Points и progression выше первого уровня;
- class features, runtime inventory и исполняемые ancestry effects;
- encounter conditions, Raise a Shield, multiple attack penalty и специализированные ancestry strikes;
- server-side access sources для uncommon languages;
- respec завершённого персонажа;
- оставшиеся remastered-поля ancestry: granted items и granted rules.

Подробный список — в [`../20_domain/character_creation/known_gaps.md`](../20_domain/character_creation/known_gaps.md), а порядок последующих задач — в [`../30_task_notes/character_creation_near_term_roadmap.md`](../30_task_notes/character_creation_near_term_roadmap.md).
