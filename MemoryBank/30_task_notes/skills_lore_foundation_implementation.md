# Skills и Lore Foundation — Implementation Plan

## Проблема

Каталог backgrounds уже описывает training в обычном skill, Lore и skill feat через `BackgroundGrantDescriptor`, но эти grants остаются декларативными строками:

- выбранный background не обучает персонажа навыкам;
- обязательные варианты вроде Nature/Occultism или Legal Lore/Warfare Lore не выбираются в wizard;
- открытые Lore-темы вроде terrain или visited city не имеют типизированного пользовательского выбора;
- training не сохраняется и отсутствует в character read-модели;
- будущие class grants не смогут корректно разрешить повторное обучение без общего skill foundation.

В результате character creation показывает описание будущего эффекта, но не формирует проверяемое состояние персонажа.

## Ожидаемый результат

Background package фактически выдаёт персонажу:

- trained rank в одном из 16 общих skills `Player Core`;
- trained rank в одной конкретной Lore subcategory;
- сохранённые и проверенные choices для grants, которые требуют выбора.

Сервер разрешает grants по типизированному каталогу, сохраняет training и возвращает его в list/details read-модели. Wizard собирает обязательные background skill/Lore choices, а карточка персонажа показывает полученное обучение.

Skill feats остаются декларативными до появления отдельного каталога и подсистемы feats.

## Источники правил

- [AoN: Chapter 4 — Skills](https://2e.aonprd.com/Rules.aspx?ID=171) фиксирует key attribute, trained rank и правило повторного training: повтор обычного skill заменяется другим skill, повтор Lore — другой Lore.
- [AoN: Lore](https://2e.aonprd.com/Skills.aspx?ID=41) определяет каждую Lore subcategory как отдельный skill и допускает узкие темы, согласованные с GM.
- [AoN: Character Creation](https://2e.aonprd.com/Rules.aspx?ID=66) фиксирует типичный background package: один общий skill, один Lore skill и skill feat.
- Нормализованный проектный список находится в [`../20_domain/character_creation/skill_catalog_player_core.md`](../20_domain/character_creation/skill_catalog_player_core.md).

## Граница задачи

### Входит

- каталог 16 общих skills со stable id, display name и key ability;
- отдельные типы для general skill и Lore subcategory;
- trained-only proficiency state для skills и Lore;
- нормализация всех skill/Lore grants 35 существующих backgrounds;
- fixed, finite-choice и open Lore grants;
- выбор background skill/Lore в create request и wizard;
- серверная валидация choices только по выбранному background;
- обратимое применение background training вместе с заменой background package;
- provenance training от конкретного background grant;
- persistence через EF migration;
- list/details read-модель и отображение в wizard, review и карточке;
- legacy-совместимость для существующих characters без training state;
- domain, application/integration и frontend tests;
- обновление MemoryBank и финальный code review.

### Не входит

- применение background skill feats;
- class fixed skills и дополнительные class skills;
- дополнительные skills от Intelligence;
- Expert, Master и Legendary ranks;
- skill increases и level progression;
- полный skill modifier, checks, DC и actions;
- proficiency bonus с добавлением level;
- ancestry feats/effects, которые выдают training;
- deity/order/racket/school и другие class-dependent skill grants;
- GM workflow для утверждения произвольной Lore;
- редактирование уже сохранённого персонажа отдельным endpoint.

## Нормализация данных

### General skills

Каталог содержит ровно 16 skills и использует существующий формат stable id `skill.<slug>`:

| Skill id | Name | Key ability |
|---|---|---|
| `skill.acrobatics` | Acrobatics | Dexterity |
| `skill.arcana` | Arcana | Intelligence |
| `skill.athletics` | Athletics | Strength |
| `skill.crafting` | Crafting | Intelligence |
| `skill.deception` | Deception | Charisma |
| `skill.diplomacy` | Diplomacy | Charisma |
| `skill.intimidation` | Intimidation | Charisma |
| `skill.medicine` | Medicine | Wisdom |
| `skill.nature` | Nature | Wisdom |
| `skill.occultism` | Occultism | Intelligence |
| `skill.performance` | Performance | Charisma |
| `skill.religion` | Religion | Wisdom |
| `skill.society` | Society | Intelligence |
| `skill.stealth` | Stealth | Dexterity |
| `skill.survival` | Survival | Wisdom |
| `skill.thievery` | Thievery | Dexterity |

`Lore` не является семнадцатой записью этого каталога.

### Lore

Каждая Lore subcategory является отдельным обучаемым навыком с Intelligence как стандартной key ability.

Представление разделяется на:

- catalog Lore с заранее определённым stable id, например `lore.warfare`;
- custom Lore для открытого background grant, содержащую нормализованный identity и отдельное display name.

Для custom Lore сервер:

- принимает тему, а не готовый технический id;
- обрезает пробелы и проверяет непустое значение и ограничение длины;
- формирует детерминированный canonical key после Unicode/case normalization;
- сохраняет display name отдельно от canonical identity;
- запрещает маскировать general skill добавлением слова `Lore`.

Точный алгоритм canonical key и ограничения длины фиксируются тестами до изменения API.

### Background grants

`BackgroundGrantDescriptor` перестаёт использовать произвольный `Id` одновременно как grant id и target id. Для каждого skill/Lore grant явно задаются:

- стабильный `GrantId`;
- `Kind`;
- fixed target, список допустимых target options либо признак open Lore;
- summary и оставшиеся deferred dependencies.

После этого:

- fixed grant не принимает choice от клиента;
- finite-choice grant требует один target id из `Options`;
- open Lore grant требует валидную custom Lore topic;
- лишние, отсутствующие и не принадлежащие background choices отклоняются;
- `SkillFeat` остаётся декларативным и не включается в training state.

## Доменная модель

### Catalog

Вводятся:

- `SkillDefinition` — валидированный stable id с префиксом `skill.`, name, key ability и source;
- `ISkillRepository` — получение полного каталога и поиск по id;
- `LoreId`/`LoreTopic` — identity конкретной Lore subcategory;
- catalog/policy для известных и custom Lore choices.

### Character training

`DraftCharacter` хранит две разные коллекции:

- trained general skills;
- trained Lore subcategories.

Каждая запись содержит target identity и provenance (`BackgroundGrantId`). Для Lore дополнительно сохраняется display name.

В v1 наличие записи означает только rank `Trained`. Общий `ProficiencyRank` и более высокие ranks не вводятся преждевременно; следующая задача по class proficiency должна расширить это представление без изменения stable ids.

### Применение background package

Background boosts и training применяются одной согласованной операцией либо двумя внутренними шагами builder, которые не позволяют сохранить частично разрешённый package.

Правила замены:

1. Все новые choices валидируются до изменения персонажа.
2. Старые boosts и training выбранного background удаляются.
3. Применяются новые boosts и training.
4. При ошибке прежнее состояние остаётся неизменным.
5. Повторное применение того же package идемпотентно.

### Повторные grants

PF2e не повышает skill до Expert при повторном получении trained rank.

Foundation фиксирует следующие правила:

- одна target identity хранится не более одного раза;
- повторное применение того же source grant не создаёт дубликат;
- повтор general skill из другого source требует replacement general skill;
- повтор Lore из другого source требует replacement Lore;
- молчаливое удаление grant, автоматическое повышение rank и произвольный выбор первого свободного skill запрещены.

В текущем vertical slice единственным применяемым source является Background, поэтому межисточниковый replacement UI откладывается до class skill task. Доменный контракт и тесты должны сделать будущую коллизию явной, а не зафиксировать неверное поведение.

## API

### Skill catalog

Добавляется read-only endpoint:

```text
GET /api/skills
```

Он возвращает 16 general skills:

```json
{
  "id": "skill.nature",
  "name": "Nature",
  "keyAbility": "Wisdom"
}
```

Отдельный бесконечный `GET /api/lore` не создаётся. Известные Lore options приходят в нормализованных background grants, а open Lore задаётся пользователем.

### Background catalog

Background grant contract явно различает:

- `Fixed`;
- `Choice` с `options`;
- `OpenLore`.

Опции возвращают типизированный target id и display name, чтобы frontend не разбирал строки `skill.*`/`lore.*`.

### Create request

`CreateCharacterRequestDto` получает коллекцию background training choices, привязанную к `GrantId`.

Choice содержит ровно одно из:

- выбранный target id для finite choice;
- custom Lore topic для open Lore.

Fixed grants в request не дублируются. Сервер сам добавляет их из background catalog.

### Character read-модель

`CharacterDto` получает отдельный блок training:

- general skills: id, name, key ability, source grant;
- Lore: id/canonical identity, display name, key ability, source grant.

Legacy character без сохранённых данных возвращает пустые коллекции, а не ошибку и не приблизительно восстановленные данные.

## Persistence

Training является состоянием персонажа и сохраняется. Предпочтительный v1-вариант — две JSONB-коллекции в `Character`:

- `TrainedSkills`;
- `TrainedLore`.

Для обеих коллекций нужны явные EF conversion/value comparer и безопасный default `[]`. Миграция создаётся только через `dotnet ef`.

Выбор JSONB подтверждён для v1: ближайшая class task добавляет источники training и ranks к bounded state одного персонажа, но не требует поиска персонажей по конкретному skill. Записи должны хранить stable target id и source grant id, чтобы следующий vertical slice мог объединять grants без смены identity. Переход к отдельным таблицам остаётся возможным, если появятся cross-character queries.

## Frontend flow

### Wizard

Background step дополнительно показывает:

- fixed skill/Lore как read-only результат;
- select для finite skill/Lore options;
- text field для open Lore topic.

Смена background очищает только относящиеся к нему training choices. Перейти дальше нельзя, пока все обязательные skill/Lore grants не разрешены.

Skill feat продолжает отображаться как deferred grant и не блокирует создание.

### Review и details

Review показывает итоговый trained skill и Lore до submit. Character details показывает сохранённые training entries отдельными группами. Dashboard в v1 не расширяется, чтобы не перегружать карточку списка.

Frontend не вычисляет grants по background id и не нормализует custom Lore identity — это делает сервер.

## Этапы выполнения

1. Перепроверить 16 general skills, все 35 background grants, finite/open choices и правила duplicate training; зафиксировать окончательные catalog contracts и persistence decision.
2. Реализовать типизированный skill catalog, Lore identity/policy и domain tests каталога/нормализации.
3. Нормализовать `BackgroundGrantDescriptor` и `BackgroundRepository`; добавить catalog/application tests для fixed, finite-choice и open Lore grants.
4. Добавить training state и атомарное применение background grants в `DraftCharacter`; покрыть применение, замену, идемпотентность, invalid choices и duplicate policy domain tests.
5. Расширить builder, create request/validator/handler, skill endpoint и character read-модель; добавить application/integration tests, включая legacy character.
6. Добавить EF mapping и migration через `dotnet ef`; проверить round-trip PostgreSQL model и отсутствие неожиданных model changes.
7. Расширить frontend API types и Background step, review/details UI, локализацию и frontend tests.
8. Обновить MemoryBank и выполнить проверки активного backend/frontend scope.
9. Провести отдельный code review, исправить замечания и повторить затронутые проверки.

После каждого этапа оставшийся план пересматривается. Class skills, skill feats, higher proficiency ranks и skill modifiers не добавляются неявно; необходимость такого расширения оформляется отдельной задачей.

## Критерии готовности

- endpoint skills возвращает ровно 16 уникальных stable ids с правильными key abilities;
- Lore моделируется отдельно и не является одной общей записью skill catalog;
- каждый из 35 backgrounds разрешается в один trained general skill и одну Lore subcategory;
- fixed grants не требуют клиентского ввода;
- finite choices принимают только catalog options;
- open Lore требует валидную нормализованную тему;
- лишние и отсутствующие grant choices отклоняются;
- skill feat не применяется как training и остаётся декларативным;
- замена background откатывает его прежние boosts и training;
- повторное применение package не создаёт дубликатов;
- duplicate training не повышает rank и не теряется молча;
- training сохраняется через EF и возвращается после нового DbContext;
- legacy character читается с пустыми training collections;
- create request не принимает итоговые training collections напрямую;
- wizard блокирует продолжение при неразрешённых choices;
- review и details показывают фактически выбранные skill/Lore;
- domain, application/integration и frontend tests проходят;
- финальный code review не содержит незакрытых замечаний в scope задачи.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes` для `CharacterManagementDbContext`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- полный `Pathfinder.sln` как диагностическая проверка с учётом известного legacy blocker в `Store.Application`.

## Статус

- план и граница задачи зафиксированы;
- этап 1 завершён: подтверждены 16 general skills, fixed/finite/open background grants, duplicate boundary и JSONB persistence;
- этапы 2–9 завершены: domain catalog/Lore identity, normalized background grants, aggregate training, API/read-модель, EF migration, frontend flow, документация и финальный code review выполнены;
- domain tests: `69`, application/integration tests: `69`, frontend tests: `18`;
- migration `AddCharacterTraining` создана через `dotnet ef`, pending model changes отсутствуют;
- frontend lint и production build проходят;
- полный `Pathfinder.sln` сохраняет известный legacy blocker: `53` ошибки в `Store.Application` вне scope задачи;
- на code review запрещено молчаливое разрешение training без skill catalog, добавлена проверка разрешимости всех 35 backgrounds и исправлен двойной суффикс `Lore` во frontend preview;
- открытых замечаний в scope Skills и Lore foundation нет;
- MCP Vikunja недоступен, поэтому tracker task пока не создавалась и не обновлялась.

## Связанные документы

- [Near-term roadmap](character_creation_near_term_roadmap.md)
- [Backend MVP](mvp_character_creation_backend.md)
- [Frontend MVP](mvp_character_creation_frontend.md)
- [Skill catalog](../20_domain/character_creation/skill_catalog_player_core.md)
- [Background catalog](../20_domain/character_creation/background_catalog_core_rulebook.md)
- [Target full domain rules](../20_domain/character_creation/domain_rules_target_full.md)
