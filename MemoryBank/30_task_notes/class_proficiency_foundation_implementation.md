# Class Proficiency Foundation — Implementation Plan

## Проблема

Стартовые proficiencies восьми классов `Player Core` сейчас существуют только как текстовый `InitialProficiencies` rule в class-каталоге. Сервер не может типобезопасно различить Perception, saves, weapon attacks, defenses и class DC, а character read-модель и frontend показывают только декларативную сводку.

Из-за этого следующие derived statistics и class choices не имеют общего proficiency foundation. При этом раннее включение skills, spellcasting или doctrine/racket/patron effects смешало бы несколько независимых подсистем.

## Ожидаемый результат

Каждый из восьми реализованных классов содержит нормализованный типизированный набор стартовых proficiencies. Выбранный `ClassId` однозначно определяет class grants персонажа; API и карточка персонажа возвращают и группируют их по категории.

В задаче не появляется нового пользовательского выбора. Отдельное изменяемое состояние proficiency не сохраняется: baseline вычисляется из уже персистируемого `SelectedClassId` и class-каталога. Будущие повышения по уровню и grants из других источников должны накладываться отдельными слоями, не перезаписывая class baseline.

## Граница задачи

### Входит

- `ProficiencyRank`: `Untrained`, `Trained`, `Expert`, `Master`, `Legendary`;
- категории `Perception`, `SavingThrow`, `Attack`, `Defense`, `ClassDc`;
- стабильные target ids для Perception, трёх saves, групп атак, типов защиты и class DC;
- типизированные class proficiency grants с rank и source id;
- нормализованная матрица grants для восьми классов `Player Core`;
- замена текстового `InitialProficiencies` descriptor типизированным свойством class-каталога;
- class catalog API и character read-модель;
- группированное отображение в карточке персонажа и краткое отображение в class step/review wizard;
- domain, application, integration и frontend tests;
- обновление MemoryBank и финальный code review.

### Не входит

- proficiency bonus (`level + rank`) и итоговые modifiers;
- AC, save modifiers, Perception modifier, attack modifiers и class DC value;
- spell attack и spell DC;
- skill и Lore proficiency ranks, дополнительные class skills и Intelligence formula;
- deity favored weapon, Cleric Doctrine armor, Rogue's Racket, Druidic Order, Witch Patron и другие choice-dependent grants;
- class features, feats, spellcasting и equipment;
- level-up progression и proficiency increases;
- temporary bonuses, item/status/circumstance bonuses;
- классы `Player Core 2` и других источников.

## Нормализация данных

Первый этап должен перепроверить исходные class rules и зафиксировать полную C#-готовую матрицу. Особого внимания требуют:

- наличие trained class DC у каждого класса, даже если текущая текстовая сводка его опускает;
- точные группы оружия Rogue и остальных классов;
- отдельность `Unarmed` от simple/martial weapons;
- точные armor categories для каждого класса;
- исключение deity/doctrine/racket/order/patron-dependent grants;
- исключение spell attack/DC из этого scope.

Нормализованный документ расширяет [`../20_domain/character_creation/class_catalog_player_core.md`](../20_domain/character_creation/class_catalog_player_core.md) или выносится в отдельную связанную матрицу, если таблица заметно разрастётся.

## Предлагаемая доменная модель

### Rank и category

`ProficiencyRank` задаёт общий порядок ranks. `Untrained` является корректным эффективным значением, но отсутствующие baseline grants не требуется перечислять в каталоге.

`ProficiencyCategory` ограничивается пятью категориями текущего scope:

- `Perception`;
- `SavingThrow`;
- `Attack`;
- `Defense`;
- `ClassDc`.

### Target

`ProficiencyTarget` содержит stable `Id`, display `Name` и `Category`.

Минимальный каталог targets:

- `proficiency.perception`;
- `proficiency.save.fortitude`, `proficiency.save.reflex`, `proficiency.save.will`;
- `proficiency.attack.unarmed`, `proficiency.attack.simple_weapons`, `proficiency.attack.martial_weapons`, `proficiency.attack.advanced_weapons`;
- `proficiency.defense.unarmored`, `proficiency.defense.light_armor`, `proficiency.defense.medium_armor`, `proficiency.defense.heavy_armor`;
- class-specific targets вида `proficiency.class_dc.bard`.

Точные ids финализируются на этапе нормализации и после этого считаются публичным контрактом.

### Grant

`ProficiencyGrant` содержит ссылку на immutable `ProficiencyTarget`, `Rank` и `SourceGrantId`. В API target разворачивается в `TargetId`, `Name` и `Category`.

Для class baseline source имеет stable id вида `class.fighter.initial_proficiencies`. В одной class-записи target не может повторяться. Rank ниже `Trained` не хранится как grant.

`CharacterClass.InitialProficiencies` становится типизированной коллекцией grants. Старый `CharacterClassRuleKind.InitialProficiencies` удаляется после перевода всех восьми классов; dependency `ProficiencyRules` остаётся только у ещё декларативного additional-skills flow. Остальные декларативные rules сохраняются без изменений.

## Эффективные proficiencies и persistence

В этом срезе единственный типизированный источник — выбранный класс. Эффективный baseline определяется через `SelectedClassId -> CharacterClass.InitialProficiencies`.

Отдельная JSONB-колонка не добавляется, потому что она дублировала бы immutable catalog data и могла бы рассинхронизироваться. Существующие персонажи с class package автоматически получают grants после обновления каталога; персонажи без класса возвращают пустой набор.

Foundation должен оставить явную точку расширения для будущего resolver:

- объединение grants по `TargetId`;
- эффективный rank равен максимальному rank среди источников;
- удаление одного источника не удаляет grants других источников;
- одинаковые grants разных источников остаются различимыми для диагностики.

Сам multi-source resolver и persistence level-up grants не входят в текущую задачу, если они не нужны для чистого контракта модели.

## API-контракт

### Class catalog

`GET /api/classes` получает `initialProficiencies`. Каждая запись содержит:

- `targetId`;
- `name`;
- `category`;
- `rank`;
- `sourceGrantId`.

Текстовый `InitialProficiencies` rule больше не возвращается. Остальные class rules и deferred dependencies сохраняются.

### Character read-модель

`CharacterDto` получает `proficiencies` с тем же типизированным содержимым либо эквивалентной группированной DTO-моделью. Для персонажа с классом данные разрешаются строго через class repository; неизвестный persisted `ClassId` остаётся ошибкой целостности, а не молчаливым fallback.

Create request не изменяется: отдельного proficiency input от клиента нет.

## Frontend

- обновить TypeScript-контракты ranks, categories и grants;
- в class step и review заменить текстовую сводку типизированным кратким представлением;
- в карточке персонажа сгруппировать Perception, Saves, Attacks, Defenses и Class DC;
- rank локализовать, target names пока можно получать из API;
- не вычислять числовой proficiency bonus;
- сохранить корректное отображение legacy-персонажа без класса.

## Этапы выполнения

1. Перепроверить AoN-данные восьми классов и зафиксировать матрицу target/rank, отдельно разобрав class DC и choice-dependent исключения.
2. Добавить rank/category/target/grant, target catalog и доменные инварианты с unit-тестами.
3. Перевести восемь записей class-каталога с текстового descriptor на typed grants и покрыть полноту/уникальность каталога тестами.
4. Провести grants через application DTO, class catalog API и character read-модель; добавить controller/query/integration tests, включая legacy без класса.
5. Обновить frontend class step, review и карточку персонажа; добавить локализацию и frontend tests.
6. Обновить MemoryBank и выполнить проверки активного backend/frontend scope. Отдельно подтвердить, нужна ли EF migration; ожидаемое решение — migration не нужна.
7. Провести отдельный code review, устранить замечания и повторить затронутые проверки.

После каждого этапа оставшийся план пересматривается. Выявленные choice-dependent или spell proficiency rules фиксируются как зависимости и не расширяют текущий scope.

## Проверки

- domain tests для enum ordering, target/grant invariants и class completeness;
- catalog tests: ровно восемь классов, уникальные targets внутри класса, только допустимые ranks/categories;
- точечные assertions для saves, attacks, defenses и class DC каждого класса;
- application/controller tests для typed catalog contract;
- query tests для персонажа с классом, без класса и с неизвестным class id;
- frontend component/helper tests для группировки и локализации ranks;
- build `CharacterManagement.Infrastructure` и `Pathfinder.Web`;
- frontend test, lint и build;
- `dotnet ef migrations has-pending-model-changes`, даже если migration не ожидается;
- `git diff --check` и проверка C# style;
- полная solution build запускается диагностически, но известные legacy-ошибки `Store.Application` не считаются дефектом этого scope.

## Критерии готовности

- все восемь классов содержат проверенный typed baseline;
- Perception, saves, attacks, defenses и class DC представлены stable targets и rank;
- class catalog и character read-модель не зависят от разбора `Summary`;
- смена выбранного класса меняет весь baseline без остатка предыдущего класса;
- клиент не может передать или подменить proficiency grants;
- legacy-персонажи с сохранённым class id получают baseline без backfill;
- персонаж без класса остаётся читаемым и имеет пустой proficiency набор;
- spell, skill и choice-dependent proficiencies не имитируются;
- backend/frontend проверки проходят;
- финальный code review не содержит незакрытых замечаний в scope задачи.

## Статус

- план и граница задачи зафиксированы;
- этап 1 завершён: AoN-матрица восьми классов зафиксирована в [`../20_domain/character_creation/class_proficiencies_player_core.md`](../20_domain/character_creation/class_proficiencies_player_core.md);
- этапы 2–5 завершены: доменная модель, восемь class baselines, API/read-модель и frontend реализованы;
- этап 6 завершён: отдельная EF migration не требуется, pending model changes отсутствуют;
- этап 7 завершён: code review проведён, открытых замечаний в scope нет;
- Domain tests: `77/77`;
- Infrastructure tests: `78/78`;
- `Pathfinder.Web` build, frontend tests `21/21`, lint и build проходят;
- полная solution build сохраняет известные `53` ошибки legacy `Store.Application`, не связанные с этим scope.

## Результаты code review

На финальном review исправлены:

- отсутствие проверки неизвестного `ProficiencyCategory`;
- возможность создать class DC target с пустым class suffix;
- сохранение mutable-ссылки на переданную коллекцию initial proficiencies;
- отсутствие regression test для read-модели персонажа с классом без class repository.

После исправлений все затронутые проверки повторены. Незакрытых замечаний нет.
