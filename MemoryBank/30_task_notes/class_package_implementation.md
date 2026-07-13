# Class Package Implementation Plan

## Проблема

Текущий character creation заканчивает формирование характеристик после `Background`. Пользователь не может выбрать `Class`, получить class key ability boost или увидеть базовые class-данные в карточке персонажа.

Без отдельного class package следующий этап — четыре финальных boosts — нельзя добавить с корректным порядком источников характеристик и независимым откатом каждого пакета.

## Ожидаемый результат

Новый персонаж создаётся с обязательным `Class` из baseline `Player Core` и допустимой безусловной key ability. Сервер проверяет выбор, применяет один class boost после background boosts, сохраняет class package и возвращает его в read-модели.

Frontend позволяет выбрать класс и key ability, показывает базовые HP и декларативные class rules. Proficiencies, features, spells, deity, skills и class branch choices не применяются к персонажу до появления соответствующих подсистем.

## Источник данных

Baseline задачи — восемь классов `Player Core`, перечисленных в [`../20_domain/character_creation/aon_player_core_character_creation_sources.md`](../20_domain/character_creation/aon_player_core_character_creation_sources.md):

- Bard;
- Cleric;
- Druid;
- Fighter;
- Ranger;
- Rogue;
- Witch;
- Wizard.

Перед реализацией данные нормализуются в отдельный C#-готовый catalog document по workflow [`../10_workflow/aon_elasticsearch_usage.md`](../10_workflow/aon_elasticsearch_usage.md). Другие книги не входят в baseline.

## Граница задачи

### Входит

- нормализованный каталог восьми классов `Player Core`;
- стабильные идентификаторы классов и декларативных rules;
- название, source, base HP и безусловно доступные key ability options;
- декларативные starting proficiencies, granted features, spell tradition и обязательные class choices;
- выбор `Class` и одной доступной key ability;
- отдельное применение и откат class boost;
- хранение class id и выбранной key ability;
- `GET /api/classes`;
- расширение `POST /api/character` и character read-модели;
- EF Core migration;
- отдельный шаг frontend wizard и отображение class package в карточке;
- domain, application, integration и frontend tests;
- обновление MemoryBank и финальный code review.

### Не входит

- фактический выбор Muse, Deity, Doctrine, Druidic Order, Hunter's Edge, Rogue's Racket, Patron, Arcane Thesis или Arcane School;
- racket-зависимые альтернативные key abilities Rogue;
- применение starting proficiencies, skills, class feats или class features;
- spellcasting, spell repertoire и выбор заклинаний;
- deity и anathema;
- familiar;
- starting equipment или wealth;
- вычисление полного HP персонажа и других derived statistics;
- четыре финальных свободных boosts;
- partial boosts и level-up progression;
- классы из `Player Core 2` и других источников.

## Доменные решения

1. Каталог `Class` живёт вне `DraftCharacter`, как каталоги `Ancestry` и `Background`.
2. `DraftCharacter` хранит подтверждённый class package: class id и выбранную key ability.
3. Class boost образует отдельный пакет из одного boost и применяется после background package.
4. Смена класса или key ability откатывает только предыдущий class boost и не изменяет ancestry/background boosts.
5. Key ability может совпадать с характеристикой, повышенной предыдущим пакетом: уникальность действует только внутри одновременного пакета.
6. Клиент передаёт class id и key ability, но не итоговый `AbilityScore`; итог вычисляет сервер.
7. `BaseHitPoints` возвращается как справочное class-значение. Полный HP персонажа в этом scope не вычисляется и не хранится.
8. Proficiencies, features, spell tradition и обязательные class choices представлены стабильными декларативными descriptors с зависимостями на отсутствующие подсистемы.
9. Для Rogue в текущем flow безусловно выбирается только `Dexterity`. Возможность другой key ability фиксируется как зависимость от будущего `Rogue's Racket` flow и не имитируется свободным выбором.
10. Существующие персонажи без Class остаются читаемыми после миграции, но новые запросы создания обязаны содержать полный class choice.

## Предлагаемая модель каталога

`ClassDefinition` содержит:

- `Id`;
- `Name`;
- `Source`;
- `BaseHitPoints`;
- `KeyAbilityOptions` — только доступные в текущем flow значения;
- `SpellTradition` — nullable/declarative;
- `Rules` — proficiencies, features и mandatory choices;
- `DeferredDependencies` — каталоги или flows, необходимые для фактического применения rules.

Декларативный rule descriptor содержит стабильный id, kind, fallback name/summary, признак обязательного выбора и список зависимостей.

## API-контракт

### Каталог

```text
GET /api/classes
```

Каждая запись содержит:

- `id`;
- `name`;
- `source`;
- `baseHitPoints`;
- `keyAbilityOptions`;
- nullable `spellTradition`;
- декларативные `rules` и `deferredDependencies`.

### Создание персонажа

В `POST /api/character` добавляются:

```json
{
  "classId": "class.fighter",
  "classKeyAbility": "Strength"
}
```

Оба поля обязательны для новых персонажей. Сервер отклоняет неизвестный класс и key ability, отсутствующую в доступных options класса.

### Read-модель

`CharacterDto` получает nullable `ClassPackage`:

- `classId`;
- `name`;
- `baseHitPoints`;
- `keyAbility`;
- декларативные rules.

Nullable-контракт сохраняет чтение записей, созданных до class migration.

## Этапы выполнения

1. Нормализовать данные восьми классов, stable ids, key ability rules и декларативные зависимости; отдельно перепроверить Rogue boundary.
2. Реализовать доменную модель, каталог, применение/откат class boost и unit-тесты.
3. Добавить application/API contracts для class catalog и character creation, серверную валидацию и тесты.
4. Добавить EF persistence, migration и round-trip integration tests с поддержкой legacy-персонажей без Class.
5. Расширить frontend wizard и карточку персонажа, добавить frontend tests и локализацию.
6. Обновить MemoryBank и выполнить полные проверки активного backend/frontend scope.
7. Провести отдельный code review, устранить замечания и повторить проверки.

После каждого этапа план пересматривается с учётом фактических class rules и найденных зависимостей. Расширение в subclass/spell/skill flows оформляется отдельными задачами, а не добавляется неявно.

## Критерии готовности

- каталог возвращает ровно восемь baseline-классов `Player Core`;
- каждый класс имеет stable id, source, base HP и хотя бы одну доступную key ability;
- Fighter и Ranger допускают `Strength` или `Dexterity`;
- Bard, Cleric, Druid, Witch и Wizard допускают зафиксированную class key ability;
- Rogue в текущем scope допускает `Dexterity`, а racket-зависимая ветка видна как deferred dependency;
- сервер отклоняет неизвестный класс и недопустимую key ability;
- class boost применяется после Background и может повторять характеристику из предыдущего пакета;
- смена class package откатывает только предыдущий class boost;
- class package сохраняется и восстанавливается из PostgreSQL;
- существующий персонаж без Class остаётся читаемым;
- frontend не позволяет продолжить без полного class choice;
- карточка персонажа показывает Class, base HP, key ability и декларативные rules;
- proficiencies, features и mandatory choices не изменяют отсутствующие доменные подсистемы;
- backend tests, frontend tests, build и lint проходят;
- финальный code review не содержит незакрытых замечаний в Class scope.

## Статус выполнения

- план и граница задачи зафиксированы;
- этап 1 завершён: class catalog и key ability rules нормализованы в [`../20_domain/character_creation/class_catalog_player_core.md`](../20_domain/character_creation/class_catalog_player_core.md);
- Rogue boundary подтверждён: альтернативная key ability зависит от будущего Racket flow;
- этапы 2–7 завершены: домен, каталог, application/API, EF migration, frontend и code review выполнены;
- code review добавил явный доменный инвариант порядка: Background package обязан существовать до Class package;
- domain tests: `34`, integration/application tests: `50`, frontend tests: `11`;
- `Pathfinder.Web` и frontend собираются, frontend lint проходит, `git diff --check` проходит;
- полный `Pathfinder.sln` по-прежнему блокируется ранее отключённым legacy `Store.Application` и его отсутствующими типами; Class scope этот known issue не затрагивает;
- оболочка frontend и защита маршрута проверены визуально; Class-шаг не проверен end-to-end без локальной пользовательской сессии и настроенного backend authentication.
