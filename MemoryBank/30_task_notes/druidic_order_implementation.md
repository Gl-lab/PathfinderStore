# Druidic Order — Implementation Plan

## Проблема

`class_choice.druid.order` пока является только декларативным rule. Новый Druid можно сохранить без обязательного Order, поэтому class package не содержит order skill, granted feat и order spell.

## Ожидаемый результат

При создании Druid пользователь обязан выбрать один из четырёх вариантов `Player Core`: Animal, Leaf, Storm или Untamed. Сервер принимает только stable id из собственного каталога, сохраняет выбор и возвращает его в read API. Order skill включается в общий Class Skills Foundation flow с обычным правилом replacement, а class feat и focus spell остаются типизированными ссылками до появления соответствующих подсистем.

Legacy Druid без сохранённого Order остаётся читаемым. Обязательность применяется к новым create requests и явному повторному `SetClassPackage`.

## Источники и каталог

- [`../20_domain/character_creation/class_catalog_player_core.md`](../20_domain/character_creation/class_catalog_player_core.md);
- [`../20_domain/character_creation/aon_player_core_character_creation_sources.md`](../20_domain/character_creation/aon_player_core_character_creation_sources.md);
- [Animal](https://2e.aonprd.com/DruidicOrders.aspx?ID=8);
- [Leaf](https://2e.aonprd.com/DruidicOrders.aspx?ID=9);
- [Storm](https://2e.aonprd.com/DruidicOrders.aspx?ID=10);
- [Untamed](https://2e.aonprd.com/DruidicOrders.aspx?ID=11).

Нормализованный catalog v1:

| Order | Stable id | Page | Order skill | Granted class feat | Order spell |
|---|---|---:|---|---|---|
| Animal | `druidic_order.animal` | 125 | `skill.athletics` | `feat.animal_companion` | `spell.heal_animal` |
| Leaf | `druidic_order.leaf` | 125 | `skill.diplomacy` | `feat.leshy_familiar` | `spell.cornucopia` |
| Storm | `druidic_order.storm` | 125 | `skill.acrobatics` | `feat.storm_born` | `spell.tempest_surge` |
| Untamed | `druidic_order.untamed` | 126 | `skill.intimidation` | `feat.untamed_form` | `spell.untamed_shift` |

## Граница задачи

### Входит

- domain model `DruidicOrder` и typed benefit kind;
- server-owned repository из четырёх `Player Core` entries;
- обязательность Order для Druid и запрет для non-Druid;
- очистка stale choice при смене класса;
- `DruidicOrderId` в create request, builder и handler;
- catalog endpoint `GET /api/classes/druid/orders`;
- selected package в character read API;
- nullable persistence column `SelectedDruidicOrderId`;
- включение order skill в Class Skills Foundation resolver;
- standard replacement, если background или другой ранний grant уже обучил order skill;
- wizard select, class-training step, review, details и localization;
- domain, application, controller, persistence и frontend tests;
- EF migration, MemoryBank и code review.

### Не входит

- animal companion и familiar state;
- применение granted class feat;
- focus points, focus-spell casting и spell details;
- edicts/anathema enforcement;
- Order Explorer и дополнительные orders из книг вне `Player Core`;
- retraining существующего персонажа.

## Domain и application design

`DruidicOrder` содержит `Id`, `Name`, `Source`, один `ClassSkillGrantDescriptor` и два `DruidicOrderBenefitDescriptor`: `ClassFeat` и `FocusSpell`. Benefit descriptor содержит stable reference id, fallback name, kind и deferred dependencies. Catalog entries не исполняют feat/spell mechanics.

`DraftCharacter.SetClassPackage` получает optional `DruidicOrder` и проверяет симметричный контракт:

- `class.druid` требует Order;
- любой другой class запрещает Order;
- успешная замена записывает `SelectedDruidicOrderId`;
- смена класса очищает stale id и ранее разрешённый class training;
- invalid mutation не меняет старое состояние.

`CharacterBuilder` разрешает request id только через `IDruidicOrderRepository`. Validator проверяет presence/absence по `ClassId`. Read converter разрешает сохранённый id только при ненулевом значении, поэтому legacy Druid без Order остаётся читаемым.

## Интеграция с Class Skills Foundation

Order skill не применяется в `SetClassPackage`: class training всё ещё выполняется только после final boosts. `SetClassTraining` разрешает уже сохранённый Order через repository, проверяет соответствие выбранному id и передаёт его grant в `ClassTrainingResolver` вместе с базовыми grants класса.

Resolver работает с объединённым ordered grant set:

1. базовые class grants;
2. feature grants выбранного Order;
3. `N + INT` additional choices.

Order grant использует тот же `ClassSkillGrantChoice` и `ClassTrainingTargetChoice`, поэтому duplicate training требует стандартную replacement skill/Lore. Additional choice count не меняется: order skill является отдельным фиксированным grant.

Frontend строит тот же effective grant set из выбранного catalog Order. Смена Order пересоздаёт class grant choices и сбрасывает ещё не сохранённые class-training targets, чтобы не отправить stale grant id.

## Persistence и API

В `DraftCharacter` и EF model добавляется nullable `SelectedDruidicOrderId` с max length 100. Migration создаётся только через `dotnet ef`; backfill не выполняется.

Catalog DTO возвращает source, skill grant, typed benefits и deferred dependencies. Create request передаёт только `DruidicOrderId`. Read package возвращает выбранный order, skill grant и benefits.

## Этапы выполнения

1. Зафиксировать проверенный catalog, stable ids и границы deferred mechanics.
2. Добавить domain model/repository и catalog tests.
3. Расширить aggregate симметричной Druid/non-Druid validation и domain tests.
4. Интегрировать order skill в общий resolver и покрыть normal/replacement/stale cases.
5. Подключить request, builder, handler, validator и integration tests.
6. Добавить DTO/mappers/use case/controller/IoC и API tests.
7. Добавить EF mapping и migration через `dotnet ef`, проверить pending model changes.
8. Добавить frontend catalog/types/helper/tests, select, effective grants, review/details и localization.
9. Обновить MemoryBank, выполнить backend/frontend checks.
10. Провести отдельный code review, исправить замечания и повторить проверки.

## Review плана перед реализацией

Review выполнен до изменения production code.

1. **Skill ordering.** Order skill нельзя применять при выборе класса: Intelligence и полный набор ранних training sources ещё не окончательны. Grant добавляется только в существующий post-final-boost Class Skills flow.
2. **Duplicate replacement.** Order skill не получает отдельный resolver. Он использует `ClassSkillGrantDescriptor` и общий replacement contract, иначе backend и frontend могли бы по-разному обрабатывать конфликт с background.
3. **Source of truth.** Create request передаёт только order id. Skill, feat и spell всегда разрешаются из server catalog; клиент не может подменить grants.
4. **Legacy compatibility.** Nullable read package не отклоняет старого Druid без Order. Требование действует только в новом create/mutation flow.
5. **Deferred mechanics.** Feat/spell представлены typed references, но не добавляются в character state как будто feat, companion, familiar, focus pool или spellcasting уже реализованы.
6. **Atomicity и stale state.** Class mutation валидирует Order до `RemoveClassEffects`; смена class/Order очищает class training. Frontend при смене Order пересобирает grant choices.
7. **Persistence scope.** Используется отдельная nullable колонка в том же стиле, что Racket/Doctrine/Edge; generic class-choice storage остаётся отдельным архитектурным решением.

Открытых замечаний, блокирующих реализацию, после review нет.

## Критерии готовности

- catalog содержит ровно Animal, Leaf, Storm и Untamed с проверенными skill/feat/spell references;
- Druid невозможно создать без catalog Order, non-Druid — с Order;
- invalid mutation атомарна, смена класса очищает stale Order и training;
- order skill и duplicate replacement проходят общий Class Skills Foundation flow;
- stable id сохраняется migration-backed полем и воспроизводится read API;
- legacy Druid без Order читается;
- wizard, review и details показывают выбранный Order и deferred benefits;
- companion/familiar/feat/spell mechanics не имитируются;
- backend/frontend checks, EF pending-model check и code review проходят без открытых замечаний.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes --project CharacterManagement.Infrastructure/CharacterManagement.Infrastructure.csproj --context CharacterManagementDbContext --no-build`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- отдельный review domain atomicity, effective skill grants, API/legacy compatibility, EF migration, C# style и frontend reset behavior.

## Результат реализации и code review

Vertical slice реализован 14 июля 2026 года в соответствии с проверенным планом.

Во время отдельного code review исправлены следующие замечания:

1. Источники training от `druidic_order.*.skill.*` включены в общую очистку class training; добавлен domain-test смены Druid на другой класс.
2. Реализованный выбор Order больше не объявляется в class catalog как отсутствующая зависимость `ClassChoiceCatalog`; deferred-метки оставлены только у ещё не реализованных feat/spell mechanics.
3. Первая пустая EF migration, полученная из устаревшей сборки, удалена через `dotnet ef migrations remove`; после актуальной сборки migration пересоздана и содержит только nullable `SelectedDruidicOrderId`.
4. Защитный domain-test дополнен обязательным дополнительным training от положительного Intelligence modifier, чтобы проверять очистку на валидном состоянии персонажа.
5. В frontend read-контракте выровнено форматирование новых nullable package-полей.

Открытых замечаний после review нет. Финальный прогон: 115 domain tests, 118 infrastructure tests и 38 frontend tests; Web build, frontend lint/build, `git diff --check` и EF pending-model check прошли. Предупреждение Vite о размере существующего main chunk не относится к correctness среза.
