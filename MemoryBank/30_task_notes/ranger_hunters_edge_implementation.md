# Ranger Hunter's Edge — Implementation Plan

## Проблема

`class_choice.ranger.hunters_edge` пока является только декларативным rule. Новый Ranger можно сохранить без обязательного выбора Hunter's Edge, поэтому class package неполон и read-модель не способна воспроизвести выбранный вариант.

## Ожидаемый результат

При создании Ranger пользователь обязан выбрать один из трёх вариантов `Player Core`: Flurry, Outwit или Precision. Сервер принимает только stable id из собственного каталога, сохраняет выбор, возвращает его в read API и показывает в wizard, review и details. Боевые эффекты остаются типизированными descriptors и не исполняются преждевременно.

Legacy Ranger без сохранённого Edge остаётся читаемым. Требование применяется к новым create requests и к явному повторному `SetClassPackage`.

## Источники и каталог

- [`../20_domain/character_creation/class_catalog_player_core.md`](../20_domain/character_creation/class_catalog_player_core.md);
- [`../20_domain/character_creation/aon_player_core_character_creation_sources.md`](../20_domain/character_creation/aon_player_core_character_creation_sources.md);
- [Player Core Hunter's Edges](https://2e.aonprd.com/Sources.aspx?ID=216);
- [Flurry](https://2e.aonprd.com/HuntersEdge.aspx?ID=4&Redirected=1);
- [Precision](https://2e.aonprd.com/HuntersEdge.aspx?ID=5).

Нормализованный catalog v1:

| Edge | Stable id | Effect kind | Граница v1 |
|---|---|---|---|
| Flurry | `hunters_edge.flurry` | `MultipleAttackPenalty` | descriptor сокращённого MAP против hunted prey |
| Outwit | `hunters_edge.outwit` | `ConditionalBonuses` | descriptor условных skill/defense bonuses против hunted prey |
| Precision | `hunters_edge.precision` | `PrecisionDamage` | descriptor дополнительного precision damage |

Источник всех записей — `Player Core`, page 154. Level 17 Masterful Hunter upgrades не применяются к персонажу первого уровня и остаются вне v1.

## Граница задачи

### Входит

- domain model `HuntersEdge` и enum effect kind;
- repository из трёх `Player Core` entries;
- обязательность Edge для Ranger и запрет для non-Ranger;
- очистка stale choice при смене класса;
- `HuntersEdgeId` в create request, builder и handler;
- catalog endpoint `GET /api/classes/ranger/hunters-edges`;
- selected package в character read API;
- отдельная nullable persistence column `SelectedHuntersEdgeId`;
- wizard select, review, details и localization;
- domain, application, controller, persistence и frontend tests;
- EF migration, MemoryBank и code review.

### Не входит

- Hunt Prey action state и выбор текущей prey;
- изменение attack rolls, MAP, AC, skill checks или damage;
- Masterful Hunter upgrades;
- Warden's Boon, Shared Prey и связанные feats;
- Ranger class feat choice;
- combat engine.

## Domain и application design

`HuntersEdge` содержит `Id`, `Name`, `Source`, ordered `Effects` и `DeferredDependencies`. `HuntersEdgeEffectDescriptor` содержит stable effect id, `HuntersEdgeEffectKind`, fallback name/summary. Все entries имеют deferred dependency `ClassFeatureRules`.

`DraftCharacter.SetClassPackage` получает optional `HuntersEdge` рядом с остальными class-specific packages и проверяет симметричный контракт:

- `class.ranger` требует Edge;
- любой другой class запрещает Edge;
- успешная замена записывает `SelectedHuntersEdgeId`;
- замена класса очищает предыдущее значение;
- невалидный вызов не изменяет существующий package.

`CharacterBuilder` разрешает id только через `IHuntersEdgeRepository`. Handler передаёт request id. Validator проверяет presence/absence по `ClassId`; catalog-aware validation остаётся в builder/domain.

Read converter разрешает выбранный id через repository только когда значение сохранено. Это сохраняет чтение legacy Ranger с `null`. `CharacterClassPackageDto` получает nullable `HuntersEdgePackageDto`.

## Persistence

В `DraftCharacter` и EF model добавляется nullable `SelectedHuntersEdgeId` с max length 100. Migration создаётся только через `dotnet ef` после отдельной сборки infrastructure project. Backfill не выполняется: существующие Ranger остаются legacy incomplete records, но новые create requests без Edge отклоняются.

## Frontend flow

Catalog загружается вместе с остальными class catalogs. На Class step select виден только для Ranger. Смена класса очищает Edge. `canContinue` и submit требуют валидный catalog entry для Ranger и `null` для остальных классов.

Review и details показывают имя Edge и descriptors. Frontend не интерпретирует effect summary как игровые числа и не пытается выполнить combat rules.

## Этапы выполнения

1. Зафиксировать catalog, ids, typed effect kinds, legacy boundary и migration boundary.
2. Добавить domain model/repository и catalog tests.
3. Расширить aggregate с атомарной Ranger/non-Ranger validation и domain tests.
4. Подключить request, builder, handler, validator и integration tests.
5. Добавить DTO/mappers/use case/controller/IoC и API tests.
6. Добавить EF mapping и migration через `dotnet ef`, проверить pending model changes.
7. Добавить frontend types/helper/tests, select, review/details и localization.
8. Обновить MemoryBank и выполнить backend/frontend checks.
9. Провести отдельный code review, исправить замечания и повторить проверки.

## Review плана перед реализацией

Review выполнен до изменения production code.

1. **Legacy compatibility.** Нельзя заставлять converter отклонять уже сохранённого Ranger без нового поля. Обязательность ограничена новым create/mutation flow, read package остаётся nullable.
2. **Типизация без combat engine.** Числовое исполнение Flurry/Precision/Outwit не входит в задачу. Для различения семантики введён enum kind, а summary остаётся display/reference descriptor.
3. **Симметричная class validation.** Проверяется не только отсутствие Edge у Ranger, но и лишний Edge у non-Ranger; оба сценария должны сохранять старое состояние при ошибке.
4. **Отдельная колонка против преждевременного generic JSON.** Текущая модель использует явные nullable ids для Racket/Doctrine/Deity. В рамках 1.2 сохраняется этот паттерн; миграция generic class-choice storage не добавляется без отдельного архитектурного решения.
5. **Class Skills ordering.** Edge не выдаёт training и не меняет `N + INT`; уже реализованный class-skill flow не расширяется и остаётся после final boosts.
6. **Источник истины.** Клиент передаёт только id, а descriptor и effect kind всегда разрешаются из server catalog.

Открытых замечаний, блокирующих реализацию, после review нет.

## Критерии готовности

- catalog содержит ровно Flurry, Outwit и Precision с stable ids и typed effects;
- Ranger невозможно создать без catalog Edge, non-Ranger — с Edge;
- смена класса очищает stale Edge, invalid mutation атомарна;
- выбор сохраняется migration-backed полем и воспроизводится read API;
- legacy Ranger без Edge читается;
- wizard, review и details показывают выбор;
- combat rules остаются deferred;
- backend/frontend checks, EF pending-model check и code review проходят без открытых замечаний.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes --project CharacterManagement.Infrastructure/CharacterManagement.Infrastructure.csproj --context CharacterManagementDbContext --no-build`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- отдельный review domain atomicity, API/legacy compatibility, EF migration, C# style и frontend reset behavior.

## Статус реализации

Завершено 14 июля 2026 года.

- добавлен серверный каталог Flurry, Outwit и Precision с typed effect kinds и source metadata;
- новый Ranger обязан выбрать Edge, а non-Ranger не может передать этот выбор;
- stable id сохраняется отдельной nullable колонкой и возвращается в class package;
- legacy Ranger без сохранённого Edge остаётся читаемым;
- wizard, review и details отображают выбор и явно отмечают deferred effects;
- миграция `AddHuntersEdge` создана через `dotnet ef`.

## Review реализации перед коммитом

Review выполнен после реализации всего vertical slice и до коммита.

Найдены и исправлены два замечания:

1. `HuntersEdgeEffectDescriptor` первоначально доверял данным repository. В доменную модель добавлены проверки обязательных полей и допустимого enum kind, а также негативные тесты.
2. Legacy boundary был реализован nullable read-моделью, но не имел отдельного регрессионного теста именно для Ranger. Добавлен persistence/read test для legacy Ranger без `SelectedHuntersEdgeId`.

После исправлений открытых замечаний нет. Проверены атомарность class mutation, симметричная Ranger/non-Ranger validation, разрешение id только через server catalog, очистка frontend state при смене класса, nullable legacy read, минимальная EF migration и отсутствие исполнения combat mechanics.

Проверки после review:

- Domain tests: 108 passed;
- Infrastructure tests: 109 passed;
- frontend: 36 tests passed, lint и production build прошли;
- `Pathfinder.Web` build прошёл без warnings и errors;
- EF: `No changes have been made to the model since the last migration.`;
- `git diff --check` прошёл.
