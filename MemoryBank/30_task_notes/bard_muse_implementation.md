# Bard Muse — Implementation Plan

## Проблема

`class_choice.bard.muse` пока является только декларативным rule. Новый Bard можно сохранить без обязательной Muse, поэтому class package не определяет выданные Muse Feat и Muse Spell и не может воспроизвести этот выбор в read API.

## Ожидаемый результат

При создании Bard пользователь обязан выбрать одну из четырёх Muse из `Player Core`: Enigma, Maestro, Polymath или Warrior. Сервер принимает только stable id из собственного каталога, сохраняет выбор, возвращает его в read API и показывает в wizard, review и details.

Muse Feat и Muse Spell возвращаются как типизированные ссылки. Они не применяются к состоянию персонажа до появления полноценных class-feat и spell-repertoire подсистем.

Legacy Bard без сохранённой Muse остаётся читаемым. Обязательность применяется к новым create requests и к явному повторному `SetClassPackage`.

## Источники и каталог

- [`../20_domain/character_creation/class_catalog_player_core.md`](../20_domain/character_creation/class_catalog_player_core.md);
- [`../20_domain/character_creation/aon_player_core_character_creation_sources.md`](../20_domain/character_creation/aon_player_core_character_creation_sources.md);
- [Player Core source catalog](https://2e.aonprd.com/Sources.aspx?ID=216);
- [Enigma](https://2e.aonprd.com/Muses.aspx?ID=5);
- [Maestro](https://2e.aonprd.com/Muses.aspx?ID=6);
- [Polymath](https://2e.aonprd.com/Muses.aspx?ID=7);
- [Warrior](https://2e.aonprd.com/Muses.aspx?ID=8).

Нормализованный catalog v1:

| Muse | Stable id | Page | Muse feat | Muse spell |
|---|---|---:|---|---|
| Enigma | `bard_muse.enigma` | 98 | `feat.bardic_lore` | `spell.sure_strike` |
| Maestro | `bard_muse.maestro` | 98 | `feat.lingering_composition` | `spell.soothe` |
| Polymath | `bard_muse.polymath` | 98 | `feat.versatile_performance` | `spell.phantasmal_minion` |
| Warrior | `bard_muse.warrior` | 98 | `feat.martial_performance` | `spell.fear` |

## Граница задачи

### Входит

- domain model `BardMuse` и typed benefit kind;
- server-owned repository из четырёх `Player Core` entries;
- обязательность Muse для Bard и запрет для non-Bard;
- очистка stale choice при смене класса;
- `BardMuseId` в create request, builder и handler;
- catalog endpoint `GET /api/classes/bard/muses`;
- selected package в character read API;
- nullable persistence column `SelectedBardMuseId`;
- wizard select, review, details и localization;
- domain, application, controller, persistence и frontend tests;
- EF migration, MemoryBank и отдельный code review.

### Не входит

- применение class feat к персонажу;
- добавление Muse Spell в spell repertoire;
- spell slots, known spells и spellcasting execution;
- feat prerequisites и feat effects;
- Multifarious Muse и дополнительные Muse из книг вне `Player Core`;
- retraining существующего персонажа.

## Domain и application design

`BardMuse` содержит `Id`, `Name`, `Source` и ровно два `BardMuseBenefitDescriptor`: `ClassFeat` и `RepertoireSpell`. Benefit descriptor содержит stable reference id, fallback name, kind и deferred dependencies.

`DraftCharacter.SetClassPackage` получает optional `BardMuse` рядом с остальными class-specific packages и проверяет симметричный контракт:

- `class.bard` требует Muse;
- любой другой class запрещает Muse;
- успешная замена записывает `SelectedBardMuseId`;
- смена класса очищает stale id и старые class effects;
- invalid mutation проверяется до `RemoveClassEffects` и не меняет существующее состояние.

`CharacterBuilder` разрешает request id только через `IBardMuseRepository`. Validator проверяет presence/absence по `ClassId`, а принадлежность каталогу остаётся server-owned проверкой builder/repository/domain.

Read converter разрешает сохранённый id только при ненулевом значении. Поэтому legacy Bard с `SelectedBardMuseId == null` возвращает nullable Muse package, а не становится нечитаемым.

## Deferred benefits

Оба benefit descriptor типизированы, чтобы UI и будущие подсистемы не угадывали назначение по строковому id:

- `ClassFeat` зависит от `ClassFeatCatalog`;
- `RepertoireSpell` зависит от `SpellCatalog`.

В рамках v1 эти ссылки только подтверждают, что конкретная Muse должна дать конкретный feat и spell. Они не записываются как уже полученный feat или известное заклинание, поскольку соответствующего агрегатного состояния и правил ещё нет.

## Persistence и API

В `DraftCharacter` и EF model добавляется nullable `SelectedBardMuseId` с max length 100. Migration создаётся только через `dotnet ef` после актуальной сборки; backfill не выполняется.

Catalog DTO возвращает source и typed benefits. Create request передаёт только `BardMuseId`. Read package возвращает выбранную Muse и benefits.

После реализации catalog choice `class_choice.bard.muse` больше не должен объявлять отсутствующий `ClassChoiceCatalog`. Общие deferred dependencies `ClassFeatCatalog` и `SpellCatalog` остаются, потому что применение benefit ещё не реализовано.

## Frontend flow

Catalog загружается вместе с остальными class catalogs. Select виден только для Bard. Смена класса очищает Muse. `canContinue` и submit требуют существующую catalog Muse для Bard и `null` для остальных классов.

Review и details показывают имя Muse, Muse Feat и Muse Spell с явной deferred-меткой. Frontend не изменяет class training: в отличие от Druidic Order, Muse не выдаёт skill grant.

## Этапы выполнения

1. Зафиксировать проверенный catalog, stable ids, benefit kinds и границы deferred mechanics.
2. Добавить domain model/repository и catalog tests.
3. Расширить aggregate симметричной Bard/non-Bard validation и domain tests.
4. Подключить request, builder, handler, validator и integration tests.
5. Добавить DTO/mappers/use case/controller/IoC и API tests.
6. Добавить EF mapping и migration через `dotnet ef`, проверить pending model changes.
7. Добавить frontend types/helper/tests, select, review/details и localization.
8. Обновить MemoryBank и выполнить backend/frontend checks.
9. Провести отдельный code review, исправить замечания и повторить проверки.

## Review плана перед реализацией

Review выполнен до изменения production code.

1. **Верный remaster catalog.** В scope входят ровно четыре Muse, перечисленные `Player Core`: Enigma, Maestro, Polymath и Warrior. Legacy-названия и варианты из других книг не смешиваются с catalog v1.
2. **Spell semantics.** Muse Spell — это дополнительная запись spell repertoire, а не focus spell и не автоматически castable effect. Поэтому используется отдельный kind `RepertoireSpell`.
3. **Source of truth.** Клиент передаёт только Muse id. Feat и spell всегда разрешаются из server catalog и не могут быть подменены request payload.
4. **Legacy compatibility.** Nullable persistence/read package сохраняет чтение старых Bard без Muse; обязательность ограничена новым create/mutation flow.
5. **Deferred mechanics.** Descriptor не выдаёт персонажу feat и не создаёт repertoire entry до появления соответствующих доменных моделей.
6. **Atomicity и stale state.** Class mutation валидирует Muse до удаления старого package. Смена класса очищает Muse, frontend также сбрасывает выбор.
7. **No class-training coupling.** У Muse нет skill grant, поэтому Class Skills Foundation не расширяется и существующие training choices не пересобираются при смене Muse.
8. **Persistence scope.** Используется отдельная nullable колонка в стиле Racket, Doctrine, Edge и Order; generic storage для class choices не вводится внутри вертикального среза.

Открытых замечаний, блокирующих реализацию, после review нет.

## Критерии готовности

- catalog содержит ровно Enigma, Maestro, Polymath и Warrior с проверенными feat/spell references;
- Bard невозможно создать без catalog Muse, non-Bard — с Muse;
- invalid mutation атомарна, смена класса очищает stale Muse;
- stable id сохраняется migration-backed полем и воспроизводится read API;
- legacy Bard без Muse читается;
- wizard, review и details показывают Muse и typed deferred benefits;
- class feat и spell repertoire mechanics не имитируются;
- backend/frontend checks, EF pending-model check и code review проходят без открытых замечаний.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes --project CharacterManagement.Infrastructure/CharacterManagement.Infrastructure.csproj --startup-project Pathfinder.Web/Pathfinder.Web.csproj --context CharacterManagementDbContext --no-build`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- отдельный review domain atomicity, catalog/API/legacy compatibility, EF migration, C# style и frontend stale-state behavior.

## Результат реализации и code review

Vertical slice реализован 14 июля 2026 года в соответствии с проверенным планом.

Во время отдельного code review исправлены следующие замечания:

1. Старые domain-tests, использовавшие Bard как нейтральный class fixture, переведены на валидный Muse package, чтобы они продолжали проверять key ability и замену class boost, а не новую обязательность Muse.
2. В финальный wizard review добавлены обе typed deferred-ссылки Muse; до исправления там отображалось только имя выбранной Muse, хотя class step и details показывали feat/spell.
3. Реализованный выбор Muse больше не объявляется в class catalog как отсутствующая зависимость `ClassChoiceCatalog`; deferred dependencies оставлены у class-feat и spell-repertoire mechanics.
4. EF migration проверена вручную: она содержит только nullable `SelectedBardMuseId` длиной 100.

Открытых замечаний после review нет. Финальный прогон: 120 domain tests, 128 infrastructure tests и 39 frontend tests; Web build, frontend lint/build, `git diff --check` и EF pending-model check прошли. Предупреждение Vite о размере существующего main chunk не относится к correctness среза.
