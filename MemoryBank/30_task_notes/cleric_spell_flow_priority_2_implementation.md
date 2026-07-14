# Cleric Spell Flow первого уровня — Priority 2 Implementation Plan

## Контекст и цель

План реализует `Приоритет 2` из [Character Creation Near-Term Roadmap](character_creation_near_term_roadmap.md) поверх `master` после завершения обязательных class choices.

Новый Cleric первого уровня должен:

- выбрать допустимый primary domain, если Doctrine даёт `Domain Initiate`;
- выбирать cantrips и prepared spells из проверенного Cleric spell catalog;
- получать отдельный вычисляемый Divine Font loadout;
- получать initial domain focus spell и стартовый focus pool;
- воспроизводить весь loadout после чтения сохранённого персонажа.

Под spell flow понимается создание персонажа и статическая read-модель. Casting, расходование slots и Focus Points, damage, healing, conditions, encounters и rest lifecycle не входят в этот приоритет.

## Нормативная граница

Baseline — remastered `Player Core` для Cleric первого уровня и deity/domain data, уже выбранные проектом:

- Cleric первого уровня подготавливает пять cantrips и два rank-1 spell slots;
- Divine Font даёт четыре дополнительных slots максимального доступного rank, заполненных выбранным `Heal` или `Harm`;
- Cloistered Cleric получает `Domain Initiate` и выбирает один primary domain Deity;
- Warpriest не получает domain choice на первом уровне без отдельного правила;
- initial domain spell создаёт focus pool с максимумом `1` Focus Point;
- Cleric готовит common divine spells, а Deity может дополнительно разрешить свои granted spells соответствующего rank.

Полный каталог всех книг, uncommon/rare access вне Deity, ranks выше первого, alternate domains и spell progression не включаются.

## Общие архитектурные решения

### Единая spell identity

`SpellDefinition` — единственный нормативный объект каталога. Stable id использует префикс `spell.` и связывает:

- divine cantrips;
- common divine rank-1 spells;
- rank-1 deity-granted spells, включая spells не из divine tradition;
- initial domain focus spells.

Domain и Deity хранят только stable references на записи этого каталога. В `DraftCharacter` не копируются name, traits, source или rules text.

### Сохранённое и вычисляемое состояние

Сохраняются:

- `SelectedClericDomainId`;
- пять stable ids подготовленных cantrips;
- два stable ids rank-1 prepared slots; одинаковый spell может занимать оба slots;
- существующие `SelectedDivineFont` и Deity choice.

Вычисляются:

- доступность spells для выбранного Cleric и Deity;
- количество cantrips, prepared slots и Font slots;
- четыре Font slot entries с `Heal` или `Harm`;
- initial domain focus spell;
- maximum Focus Points и источник focus pool.

Производные значения не принимаются от клиента и не сохраняются snapshot-ом.

### Legacy compatibility

Новые persistence-поля nullable или имеют пустые коллекции для чтения старых rows. Legacy Cleric остаётся читаемым, но новый create request обязан быть полным. Non-Cleric не может передавать domain или Cleric spell loadout.

## Slice 2.1 — Player Core Domain Catalog + Cleric Domain Choice

### Проблема

Deity уже возвращает `PrimaryDomainIds`, а Cloistered Doctrine — deferred `Domain Initiate`, но domain ids не разрешаются каталогом и выбор не сохраняется.

### Реализация

1. Нормализовать все primary domains текущего Deity catalog и initial focus spell references.
2. Добавить domain model и repository с проверкой уникальности ids и spell references.
3. Добавить `GET /api/classes/cleric/domains`.
4. Добавить nullable `ClericDomainId` в create contract и `SelectedClericDomainId` в aggregate.
5. Валидация:
   - Cloistered Cleric обязан выбрать один primary domain выбранной Deity;
   - Warpriest и non-Cleric запрещают domain choice;
   - неизвестный или не принадлежащий Deity domain отклоняется;
   - смена Doctrine, Deity или class очищает старый выбор до применения нового package.
6. Сохранять только domain id; создать EF migration через `dotnet ef`.
7. Вернуть выбранный domain в class package/read-модели.
8. Добавить selector, reset rules, review и details во frontend.

### Критерии готовности

- каталог покрывает каждый primary domain, используемый текущим Deity catalog;
- initial focus spell доступен как typed reference, но focus pool ещё не применяется;
- полный domain choice проходит domain, application, API, EF round-trip и frontend tests;
- legacy Cleric без domain читается.

## Slice 2.2 — Cleric Spell Catalog v1

### Проблема

Divine cantrips, rank-1 spells, deity-granted spells и domain spells пока не имеют общей проверяемой identity.

### Реализация

1. Добавить `SpellDefinition` с id, name, rank, kind, traditions, traits, rarity и source.
2. Нормализовать ограниченный каталог:
   - common divine cantrips из выбранного baseline;
   - common divine rank-1 spells;
   - все rank-1 granted spell references текущего Deity catalog;
   - initial focus spell каждого domain из slice 2.1.
3. Добавить referential-integrity tests для Deity и Domain catalogs.
4. Добавить Cleric availability resolver:
   - common divine cantrip/rank-1 spell доступен по tradition;
   - deity-granted rank-1 spell доступен только для выбранной Deity;
   - deity-granted spell при подготовке Cleric получает effective divine tradition, не изменяя глобальную catalog definition;
   - focus spell не попадает в prepared list.
5. Добавить `GET /api/classes/cleric/spells` с catalog metadata и признаками вида spell, но без character-specific slot counts.
6. Подключить catalog во frontend и покрыть фильтрацию helper tests.

### Критерии готовности

- каждый Deity rank-1 grant и каждый Domain initial spell разрешается ровно в одну запись каталога;
- API возвращает tradition, rank, traits, rarity, kind и source;
- catalog не включает обычные spells выше первого rank и содержит только initial rank-1 domain focus spells;
- выбор loadout и persistence ещё не добавляются в этот slice.

## Slice 2.3 — Prepared Spells + Divine Font Loadout

### Проблема

Cleric нельзя создать с пригодным для игры spell loadout, а клиент мог бы подменить slot counts или выбрать недоступный spell.

### Реализация

1. Добавить request choices `ClericCantripIds` и `ClericPreparedSpellIds`.
2. Добавить domain policy/resolver первого уровня с константами:
   - cantrips: `5` уникальных доступных cantrip ids;
   - prepared rank-1 slots: `2`, duplicate ids разрешены;
   - Divine Font slots: `4`, spell определяется сохранённым Font.
3. Проверять доступность через spell catalog + selected Deity, а не по данным клиента.
4. Запретить Cleric loadout у non-Cleric и неполный loadout у нового Cleric.
5. Сохранять только выбранные cantrip/prepared ids; настроить EF JSON conversion/value comparison и создать migration через `dotnet ef`.
6. Вернуть объяснимую read-модель с отдельными группами cantrips, prepared slots и Divine Font slots.
7. Добавить frontend selectors с точным remaining count, reset при смене class/Deity и отображение в review/details.
8. Покрыть duplicates prepared spells, чужой deity grant, focus spell в prepared list, slot tampering, round-trip и legacy read.

### Критерии готовности

- сервер единолично задаёт counts и availability;
- `Heal`/`Harm` Font loadout следует сохранённому Deity/Font choice и содержит четыре вычисляемых slots;
- после round-trip порядок prepared slots и cantrips воспроизводится;
- новый Cleric без полного loadout не создаётся.

## Slice 2.4 — Domain Focus Spell + Focus Pool

### Проблема

Domain choice возвращает только reference и не объясняет выданный focus spell или focus pool.

### Реализация

1. Добавить read-модель `ClericFocusSpellPackage`:
   - выбранный Domain;
   - resolved initial focus spell;
   - `MaximumFocusPoints = 1`;
   - typed source, указывающий на `Domain Initiate` и domain id.
2. Вычислять package из сохранённых Doctrine + Domain ids и spell catalog.
3. Не сохранять current Focus Points и не добавлять casting lifecycle.
4. Показать focus spell и pool отдельно от prepared/Font slots во frontend.
5. Добавить regression tests для Cloistered, Warpriest, non-Cleric, legacy и persistence readback.

### Критерии готовности

- Cloistered Cleric получает ровно один initial domain focus spell и pool `1`;
- Warpriest/non-Cleric не получает package без другого правила;
- read-модель объясняет spell и источник pool;
- prepared, Font и focus resources не смешиваются.

## Порядок выполнения, review и commits

1. Закоммитить этот plan после отдельного review.
2. Выполнять slices строго `2.1 -> 2.2 -> 2.3 -> 2.4`.
3. После каждого slice:
   - проверить diff и соответствие границе slice;
   - выполнить targeted tests, build/lint и `git diff --check`;
   - исправить review findings;
   - обновить статус этого документа;
   - создать отдельный commit.
4. После `2.4` выполнить cross-review всего диапазона Priority 2 относительно plan commit, повторить полные backend/frontend checks и обновить roadmap/MemoryBank отдельным финальным commit при необходимости.

## Общие проверки

Backend:

```powershell
dotnet test CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj --no-restore
dotnet test CharacterManagement.Infrastructure.Tests/CharacterManagement.Infrastructure.Tests.csproj --no-restore
dotnet build Pathfinder.Web/Pathfinder.Web.csproj --no-restore
```

Frontend:

```powershell
Set-Location pathfinder.frontend
npm test
npm run lint
npm run build
```

Миграции создаются только через `dotnet ef` по workflow из [`../10_workflow/ef.md`](../10_workflow/ef.md).

## Риски и принятые ограничения

- AoN source tags для отдельных remastered domain spells могут отличаться от source Deity; source хранится на самой spell definition, а не выводится из domain.
- Green Faith использует elemental domains, поэтому domain catalog проверяется по фактическому множеству `PrimaryDomainIds`, а не по ручному списку core deities.
- Spell rules text намеренно не копируется: UI получает metadata, необходимую для выбора, а подробное исполнение spell effects остаётся будущей подсистемой.
- Current spell slots и Focus Points — runtime state будущей карточки/encounter flow и сейчас не сохраняются.
- Character progression выше первого уровня, heightened preparation и archetype access требуют отдельной модели и не расширяют этот план.

## Plan review checklist

- [x] Каждый roadmap slice имеет проверяемый vertical result.
- [x] Нет зависимости по кругу между Domain и Spell catalogs.
- [x] Сохраняются только пользовательские choices, derived state вычисляется.
- [x] Legacy read и новый strict create разделены явно.
- [x] Deity-granted spell access не расширяет divine list глобально.
- [x] Prepared, Divine Font и Focus Pool моделируются раздельно.
- [x] EF migrations запланированы только там, где меняется persistence.
- [x] Frontend не рассчитывает slot counts или eligibility самостоятельно.
- [x] Review/test/commit gate присутствует после каждого slice.
- [x] Scope не включает casting engine, progression, feats или equipment.

## Статус

- implementation plan проверен по актуальному коду, roadmap и Player Core rules;
- plan review исправил effective divine tradition для deity-granted spells и границу rank-1 focus catalog;
- slice 2.1 завершён: нормализованы 39 primary domains, добавлены catalog API, выбор Cloistered Cleric, nullable persistence, read-модель и frontend flow;
- миграция `AddClericDomain` создана через `dotnet ef`, legacy rows остаются читаемыми;
- review 2.1 исправил устаревшую frontend test command в плане и naming статического catalog field;
- backend tests, frontend tests, lint и production build для 2.1 проходят;
- slice 2.2 завершён: добавлены 94 spell definitions, referential-integrity checks, Cleric availability resolver, catalog API и frontend contract;
- slice 2.3 завершён: сервер валидирует и сохраняет 5 cantrips и 2 prepared slots, выводит 4 derived Divine Font slots, предоставляет deity-specific options API и отдельный wizard step;
- миграция `AddClericSpellLoadout` создана через `dotnet ef`; legacy rows получают пустые JSONB-коллекции;
- проверки 2.3: Domain tests `151/151`, Infrastructure tests `174/174`, frontend tests `54/54`, lint и production build проходят;
- slice 2.4 завершён: Domain Initiate разрешает полную focus spell definition и derived one-point Focus Pool с явным source grant ID;
- проверки 2.4: Domain tests `155/155`, Infrastructure tests `174/174`, frontend tests `55/55`, lint и production build проходят;
- все slices 2.1–2.4 реализованы; итоговый cross-review завершён и зафиксирован в [priority_2_final_review.md](priority_2_final_review.md).
- финальный quality gate после cross-review: Domain tests `155/155`, Infrastructure tests `175/175`, frontend tests `55/55`, Web/Infrastructure build, EF model check, lint и production build проходят.
