# Witch Patron — Implementation Plan

## Проблема

`class_choice.witch.patron` пока является только декларативным rule. Новый Witch сохраняется без Patron, поэтому не определены spell tradition, patron skill, lesson, hex cantrip, familiar spell и patron-specific familiar ability.

## Ожидаемый результат

При создании Witch пользователь обязан выбрать один из семи Patron Themes из `Player Core`. Сервер разрешает выбор только из собственного каталога, выводит из него tradition и patron skill, сохраняет stable id и возвращает полный typed package в read API.

Patron skill применяется через общий Class Skills Foundation flow. Lesson, hex cantrip, familiar spell и familiar ability остаются проверенными deferred descriptors до появления spellcasting/familiar подсистем.

Wilding Steward содержит обязательную вложенную ветку: familiar learns `summon animal` или `summon plant or fungus`. Для остальных Patron единственное familiar spell выбирается каталогом автоматически.

Legacy Witch без Patron остаётся читаемым. Обязательность применяется к новым create requests и явному повторному `SetClassPackage`.

## Источники и каталог

- [`../20_domain/character_creation/class_catalog_player_core.md`](../20_domain/character_creation/class_catalog_player_core.md);
- [`../20_domain/character_creation/aon_player_core_character_creation_sources.md`](../20_domain/character_creation/aon_player_core_character_creation_sources.md);
- [Faith's Flamekeeper](https://2e.aonprd.com/Patrons.aspx?ID=12);
- [The Inscribed One](https://2e.aonprd.com/Patrons.aspx?ID=13);
- [The Resentment](https://2e.aonprd.com/Patrons.aspx?ID=14);
- [Silence in Snow](https://2e.aonprd.com/Patrons.aspx?ID=15);
- [Spinner of Threads](https://2e.aonprd.com/Patrons.aspx?ID=16);
- [Starless Shadow](https://2e.aonprd.com/Patrons.aspx?ID=17);
- [Wilding Steward](https://2e.aonprd.com/Patrons.aspx?ID=18).

Нормализованный catalog v1:

| Patron | Stable id | Page | Tradition | Skill | Lesson | Hex cantrip | Familiar spell options | Familiar ability |
|---|---|---:|---|---|---|---|---|---|
| Faith's Flamekeeper | `witch_patron.faiths_flamekeeper` | 184 | Divine | Religion | `lesson.fervors_grasp` | `spell.stoke_the_heart` | `spell.command` | `familiar_ability.restored_spirit` |
| The Inscribed One | `witch_patron.inscribed_one` | 184 | Arcane | Arcana | `lesson.glyphs_supremacy` | `spell.discern_secrets` | `spell.runic_weapon` | `familiar_ability.flowing_script` |
| The Resentment | `witch_patron.resentment` | 184 | Occult | Occultism | `lesson.strengths_impermanence` | `spell.evil_eye` | `spell.enfeeble` | `familiar_ability.ongoing_misery` |
| Silence in Snow | `witch_patron.silence_in_snow` | 185 | Primal | Nature | `lesson.winters_chill` | `spell.clinging_ice` | `spell.gust_of_wind` | `familiar_ability.freezing_rime` |
| Spinner of Threads | `witch_patron.spinner_of_threads` | 185 | Occult | Occultism | `lesson.fates_vicissitudes` | `spell.nudge_fate` | `spell.sure_strike` | `familiar_ability.balanced_luck` |
| Starless Shadow | `witch_patron.starless_shadow` | 185 | Occult | Occultism | `lesson.nights_terrors` | `spell.shroud_of_night` | `spell.fear` | `familiar_ability.stalking_night` |
| Wilding Steward | `witch_patron.wilding_steward` | 185 | Primal | Nature | `lesson.wild_speech` | `spell.wilding_word` | `spell.summon_animal`, `spell.summon_plant_or_fungus` | `familiar_ability.keen_senses` |

## Граница задачи

### Входит

- domain model `WitchPatron`, typed benefit descriptors и familiar-spell options;
- server-owned repository из семи `Player Core` entries;
- обязательность Patron для Witch и запрет для non-Witch;
- обязательный nested familiar-spell choice только при нескольких options;
- Patron-derived spell tradition и skill grant;
- общий duplicate replacement contract Class Skills Foundation;
- очистка stale Patron, familiar spell и training при смене класса;
- `WitchPatronId` и `WitchPatronFamiliarSpellId` в create request/builder/handler;
- catalog endpoint `GET /api/classes/witch/patrons`;
- selected package в read API;
- nullable persistence columns `SelectedWitchPatronId` и `SelectedWitchPatronFamiliarSpellId`;
- wizard, class-training, review, details и localization;
- domain/application/controller/persistence/frontend tests, EF migration, MemoryBank и code review.

### Не входит

- создание familiar entity и выбор обычных familiar abilities;
- исполнение patron-specific familiar ability;
- spell slots, spell preparation, casting и familiar spell storage;
- focus points и исполнение hex cantrip;
- дополнительные Lessons, Patron feats и Patron Themes вне `Player Core`;
- retraining существующего персонажа.

## Domain и application design

`WitchPatron` содержит `Id`, `Name`, `Source`, `SpellTradition`, один `ClassSkillGrantDescriptor` и typed benefits:

- ровно один `Lesson`;
- ровно один `HexCantrip`;
- один или несколько `FamiliarSpell` options;
- ровно одну `FamiliarAbility`.

`DraftCharacter.SetClassPackage` получает optional `WitchPatron` и familiar-spell id. Контракт:

- `class.witch` требует Patron;
- non-Witch запрещает Patron и familiar spell id;
- при одном familiar spell option сервер выбирает его автоматически и запрещает отдельный payload, чтобы клиент не подменял derived data;
- при нескольких options request обязан передать ровно один id из Patron catalog;
- successful mutation сохраняет оба stable id;
- invalid mutation проверяется до `RemoveClassEffects` и атомарно сохраняет старый package;
- смена класса очищает Patron, familiar spell и patron-derived class training.

`CharacterBuilder` разрешает request id только через `IWitchPatronRepository`; familiar-spell id проверяется самим catalog object. Validator проверяет shape по class, а catalog-aware вложенную ветку проверяют builder/domain.

## Интеграция с Class Skills Foundation

Patron skill не применяется в `SetClassPackage`, потому что полный skill flow выполняется после final boosts. `SetClassTraining` разрешает уже сохранённый Patron через repository, проверяет соответствие id и передаёт `witch_patron.<id>.skill.patron` как feature grant в общий `ClassTrainingResolver`.

Duplicate с Background/Racket/другим ранним training требует обычный replacement skill или Lore. Additional count остаётся `3 + Intelligence modifier`; patron skill — отдельный fixed grant.

Frontend строит effective Witch class с добавленным skill grant и Patron-derived `spellTradition`. Смена Patron сбрасывает class-training choices и nested familiar-spell choice.

## Deferred benefits

Lesson и familiar ability зависят от `ClassFeatureRules`/`FamiliarRules`; hex и familiar spells — от `SpellCatalog`. Descriptors не создают familiar, spellbook/repertoire или focus pool и не исполняют эффекты.

Read package возвращает selected familiar spell отдельно от полного списка options, чтобы сохранённый Wilding Steward choice воспроизводился однозначно.

## Persistence и API

Обе колонки nullable и имеют max length 100. Legacy rows не backfill-ятся. Migration создаётся только через `dotnet ef` после актуальной сборки и должна содержать ровно две новые колонки.

Catalog DTO возвращает source, tradition, skill grant и typed benefits. Create payload передаёт только Patron id и, если требуется, выбранный familiar-spell id.

После реализации `class_choice.witch.patron` больше не объявляет отсутствующий `ClassChoiceCatalog`; `SpellCatalog` и `FamiliarRules` остаются deferred dependencies.

## Frontend flow

Patron select показывается только для Witch. Для Wilding Steward появляется второй select из двух server-owned familiar spell options; для остальных Patron единственный spell отображается как derived benefit. `canContinue` и submit используют catalog identity и nested-choice completeness.

Effective class grants patron skill в class-training step с общим replacement UI. Review/details показывают Patron, tradition, skill, lesson, hex, selected familiar spell и familiar ability с deferred labels.

## Этапы выполнения

1. Зафиксировать проверенный catalog, ids, nested Wilding choice и deferred boundaries.
2. Добавить domain model/repository и catalog tests.
3. Расширить aggregate симметричной validation, nested choice, atomicity и stale cleanup tests.
4. Интегрировать patron skill в общий resolver и покрыть normal/replacement cases.
5. Подключить request, builder, handler, validator и integration tests.
6. Добавить DTO/mappers/use case/controller/IoC и API/read/legacy tests.
7. Добавить EF mapping и migration через `dotnet ef`, проверить pending model changes.
8. Добавить frontend types/helper/tests, dependent selects, effective class, review/details и localization.
9. Обновить MemoryBank, выполнить checks и отдельный code review.
10. Исправить замечания review и повторить полный прогон перед коммитом.

## Review плана перед реализацией

Review выполнен до изменения production code.

1. **Nested Wilding choice.** Patron id сам по себе недостаточен для Wilding Steward. Выбранный familiar spell хранится отдельно; единственные options других Patron не требуют лишнего клиентского payload.
2. **Derived tradition.** Tradition не принимается отдельным request field и не дублируется в persistence. Она всегда выводится из server catalog и подставляется в effective/read class package.
3. **Skill ordering.** Patron skill применяется только в post-final-boost Class Skills flow, а не при class package mutation.
4. **Duplicate replacement.** Patron использует `ClassSkillGrantDescriptor` и общий resolver/UI; отдельный Witch-only replacement алгоритм не создаётся.
5. **Source of truth.** Клиент передаёт только stable ids. Tradition, skill, lesson, hex, spell options и familiar ability разрешаются сервером.
6. **Legacy compatibility.** Read converter разрешает Patron только при сохранённом id; legacy Witch без Patron остаётся читаемым.
7. **Deferred mechanics.** Catalog descriptors не имитируют familiar/spell/focus subsystems.
8. **Atomicity и stale state.** Patron и nested choice валидируются до удаления старого package; class change очищает оба id и patron training provenance.
9. **Persistence scope.** Две nullable колонки необходимы, потому что Wilding choice не выводится однозначно из Patron id. Generic class-choice storage остаётся отдельным решением.

Открытых замечаний, блокирующих реализацию, после review нет.

## Критерии готовности

- catalog содержит ровно семь `Player Core` Patron Themes с проверенными tradition/skill/lesson/hex/spell/familiar descriptors;
- Witch невозможно создать без Patron, non-Witch — с Patron;
- Wilding Steward требует один из двух familiar spells, остальные Patron не принимают лишний nested choice;
- tradition и patron skill выводятся из catalog;
- duplicate replacement использует Class Skills Foundation;
- invalid mutation атомарна, class change очищает Patron, nested choice и training;
- оба stable id воспроизводятся migration-backed read API, legacy Witch без Patron читается;
- wizard/review/details показывают полный typed package;
- familiar и spell execution не имитируются;
- backend/frontend checks, EF pending-model check и code review проходят без открытых замечаний.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes --project CharacterManagement.Infrastructure/CharacterManagement.Infrastructure.csproj --startup-project Pathfinder.Web/Pathfinder.Web.csproj --context CharacterManagementDbContext --no-build`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- отдельный review nested-choice atomicity, effective tradition/skill grants, API/legacy compatibility, EF migration, C# style и frontend reset behavior.

## Результат реализации и code review

Vertical slice реализован 14 июля 2026 года в соответствии с проверенным планом.

Во время отдельного code review исправлены следующие замечания:

1. Новый optional `IWitchPatronRepository` и параметр Patron в read mapper перенесены в конец публичных позиционных сигнатур, чтобы сохранить совместимость существующих вызовов.
2. В API-тесты добавлены прямые проверки endpoint контроллера и legacy Witch без Patron; legacy package остаётся читаемым с nullable Patron и tradition.
3. Описание `Keen Senses` исправлено на временное imprecise sense в соответствии с источником.
4. Реализованный Patron choice больше не объявляется как отсутствующий `ClassChoiceCatalog`; deferred dependencies сохранены только для spell, familiar и class-feature mechanics.
5. EF migration проверена вручную: она содержит только две nullable колонки длиной 100 для Patron и выбранного familiar spell.

Открытых замечаний после review нет. Финальный прогон: 127 domain tests, 140 infrastructure tests и 42 frontend tests; Web build, frontend lint/build, `git diff --check` и EF pending-model check прошли. Предупреждение Vite о размере существующего main chunk не относится к correctness среза.
