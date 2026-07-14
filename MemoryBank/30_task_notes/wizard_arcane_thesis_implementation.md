# Wizard Arcane Thesis — Implementation Plan

## Цель

Завершить обязательные классовые выборы Wizard первого уровня: добавить проверенный Player Core catalog из пяти Arcane Thesis, обязательный независимый выбор Thesis вместе с Arcane School, persistence, create/read API и UI. Эффекты Thesis моделируются typed descriptors с явными milestone levels и deferred dependencies; отсутствующие feat, familiar, spell preparation, spell-slot и item mechanics не имитируются.

## Источники и baseline

Каталог отобран через AoN Elasticsearch по `category:"arcane-thesis"` и `primary_source:"Player Core"`, затем записи сверены с официальными страницами Archives of Nethys:

- [Experimental Spellshaping](https://2e.aonprd.com/ArcaneThesis.aspx?ID=6) — Player Core p. 195;
- [Improved Familiar Attunement](https://2e.aonprd.com/ArcaneThesis.aspx?ID=7) — p. 195;
- [Spell Blending](https://2e.aonprd.com/ArcaneThesis.aspx?ID=8) — p. 195;
- [Spell Substitution](https://2e.aonprd.com/ArcaneThesis.aspx?ID=9) — p. 195;
- [Staff Nexus](https://2e.aonprd.com/ArcaneThesis.aspx?ID=10) — p. 195.

Stable ids используют `arcane_thesis.<snake_case>`. Effect ids принадлежат Thesis и используют `arcane_thesis.<thesis>.effect.<effect>`.

## Нормализованный каталог эффектов

### Experimental Spellshaping

- `arcane_thesis.experimental_spellshaping`;
- `first_level_spellshape_feat_choice`, milestone `[1]`: один wizard spellshape feat 1-го уровня;
- `daily_spellshape_feat_choice`, milestone `[4]`: начиная с 4-го уровня ежедневный временный wizard spellshape feat не выше половины уровня;
- dependencies: `ClassFeatCatalog`, для daily choice также `ClassFeatureRules`.

### Improved Familiar Attunement

- `arcane_thesis.improved_familiar_attunement`;
- `familiar_feat_grant`, milestone `[1]`: Familiar wizard feat;
- `familiar_ability_progression`, milestones `[1, 6, 12, 18]`: дополнительные familiar abilities;
- `drain_familiar_replacement`, milestone `[1]`: familiar становится arcane bond, Drain Familiar заменяет Drain Bonded Item;
- dependencies: `ClassFeatCatalog`, `FamiliarRules`, `ClassFeatureRules` по соответствующему эффекту.

### Spell Blending

- `arcane_thesis.spell_blending`;
- `spell_slot_blending`, milestone `[1]`: обмен двух slots одного rank на bonus slot до двух ranks выше, а также отдельная ветка обмена slot на два cantrips;
- dependencies: `SpellSlotRules`, `SpellPreparationRules`.

### Spell Substitution

- `arcane_thesis.spell_substitution`;
- `prepared_spell_substitution`, milestone `[1]`: десятиминутная замена подготовленного spell на spell из spellbook;
- dependencies: `SpellCatalog`, `SpellPreparationRules`.

### Staff Nexus

- `arcane_thesis.staff_nexus`;
- `makeshift_staff`, milestone `[1]`: makeshift staff с выбранными cantrip и spell 1-го rank из spellbook;
- `staff_charge_preparation`, milestone `[1]`: подготовка staff через расход spell;
- `staff_charge_progression`, milestones `[8, 16]`: увеличение числа spells, расходуемых для charges;
- dependencies: `ItemCatalog`, `SpellCatalog`, `SpellSlotRules`, `SpellPreparationRules`, `ClassFeatureRules` по соответствующему эффекту.

Фактический выбор spellshape feat, familiar abilities, staff spells или prepared slots не входит в эту подзадачу: соответствующие общие каталоги и runtime-механики отсутствуют.

## Domain и application design

Добавить:

- `ArcaneThesis` с `Id`, `Name`, `Source`, `Effects`;
- `ArcaneThesisEffectKind`: `FirstLevelSpellshapeFeatChoice`, `DailySpellshapeFeatChoice`, `FamiliarFeatGrant`, `FamiliarAbilityProgression`, `DrainFamiliarReplacement`, `SpellSlotBlending`, `PreparedSpellSubstitution`, `MakeshiftStaff`, `StaffChargePreparation`, `StaffChargeProgression`;
- `ArcaneThesisEffectDescriptor` с `Id`, typed `Kind`, `Name`, `Summary`, `MilestoneLevels`, `DeferredDependencies`;
- недостающие `CharacterClassDependencyType`: `SpellPreparationRules`, `SpellSlotRules`, `ItemCatalog`;
- `IArcaneThesisRepository` и immutable Player Core repository.

Инварианты:

- Thesis id начинается с `arcane_thesis.`, effect id — с полного Thesis prefix;
- каждая Thesis содержит хотя бы один effect, effect ids и kinds уникальны внутри Thesis;
- kind известен, name/summary непустые, milestone levels непустые, положительные, уникальные и возрастающие;
- каждый effect содержит хотя бы одну deferred dependency, потому что ни одна из зависимых подсистем пока не исполняет эти эффекты.

## Aggregate, request и persistence

- добавить nullable `DraftCharacter.SelectedArcaneThesisId`;
- append-only расширить `SetClassPackage`, builder и converter optional-параметрами;
- Wizard требует одновременно School и Thesis; non-Wizard запрещает оба;
- обе проверки выполняются до `RemoveClassEffects`; смена класса очищает School и Thesis;
- request/validator/handler принимают только `ArcaneThesisId`, nested effect choices не принимаются;
- EF хранит одну nullable колонку длиной 100, migration создаётся только через `dotnet ef`;
- legacy Wizard без School/Thesis остаётся читаемым, новый create flow требует оба выбора.

## API и frontend

- `GET /api/classes/wizard/arcane-theses` возвращает source и typed effects;
- character read package возвращает выбранную Thesis с теми же structured effects;
- wizard показывает независимые School и Thesis selects, effects Thesis и milestone levels;
- review/details показывают Thesis и typed effects;
- смена класса очищает оба Wizard choices;
- frontend не содержит собственного Thesis catalog и не создаёт фиктивные feat/familiar/spell/staff state.

После реализации thesis rule и top-level Wizard больше не объявляют `ClassChoiceCatalog`: School и Thesis catalogs реализованы. Top-level Wizard dependencies обновляются до объединения реально отсутствующих подсистем Thesis (`ClassFeatCatalog`, `FamiliarRules`, `SpellPreparationRules`, `SpellSlotRules`, `ItemCatalog`) с уже существующими dependencies; те же границы остаются точнее указаны на отдельных effects.

## Этапы выполнения

1. Зафиксировать проверенный каталог, effect taxonomy и deferred boundaries.
2. Добавить domain model/repository и catalog/invariant tests.
3. Расширить aggregate/request/builder/handler/validator и atomicity tests.
4. Добавить DTO/mappers/use case/controller/IoC и API/read/legacy tests.
5. Добавить EF mapping и migration, проверить migration diff.
6. Добавить frontend types/helper/tests, select, effect display, review/details и localization.
7. Обновить roadmap и MemoryBank.
8. Выполнить отдельный code review, исправить замечания и повторить полный прогон перед коммитом.

## Review плана перед реализацией

Review выполняется до изменения production-кода по следующим вопросам:

1. Полон ли Player Core catalog и не смешаны ли legacy/дополнительные Thesis.
2. Не требует ли level-1 выбор несуществующие nested catalogs.
3. Достаточно ли effect taxonomy различает feat, familiar, slot, preparation и item mechanics.
4. Не выдаются ли later-level эффекты на первом уровне.
5. Атомарны ли совместные School/Thesis инварианты и сохранена ли legacy readability.
6. Добавлены ли новые optional параметры только в конец публичных сигнатур.
7. Ограничена ли persistence одной server-owned stable-id колонкой.
8. Можно ли после 1.7 снять `ClassChoiceCatalog` с thesis rule и top-level Wizard без потери информации.

Review выполнен до изменения production-кода.

1. **Полнота каталога.** Player Core содержит ровно пять Thesis. Legacy `Metamagical Experimentation` не добавляется вторым вариантом: remaster baseline использует `Experimental Spellshaping`; Staff Nexus входит в Player Core catalog.
2. **Nested choices.** Выбор Thesis обязателен сейчас, но feat, familiar ability, staff spell и prepared-slot choices отложены до общих каталогов. Это соответствует границе roadmap, где spell loadout и feats ещё не считаются завершёнными.
3. **Effect taxonomy.** Десять kinds не смешивают постоянный first-level feat, ежедневный later-level feat, familiar progression, bond replacement, slot conversion, preparation substitution и staff lifecycle.
4. **Level boundary.** Milestone levels отделяют эффекты 4/6/8/12/16/18 уровней; descriptors не применяются к level-1 character state.
5. **Atomicity и legacy.** Новый Wizard требует одновременно School и Thesis до удаления прежнего package. Read converter по-прежнему допускает legacy Wizard без одного или обоих stable ids.
6. **Signature compatibility.** `ArcaneThesis`/repository parameters добавляются только после уже существующих Arcane School parameters.
7. **Persistence scope.** Хранится только nullable `SelectedArcaneThesisId`; effects и milestones остаются server-owned catalog data.
8. **Dependency closure.** После удаления `ClassChoiceCatalog` top-level Wizard получает объединение фактических deferred dependencies Thesis, поэтому отсутствие downstream mechanics не скрывается.

Открытых замечаний, блокирующих реализацию, после review нет.

## Критерии готовности

- catalog содержит ровно пять Player Core Thesis с нормативными typed effects и milestones;
- новый Wizard требует School и Thesis, non-Wizard запрещает оба;
- invalid mutation атомарна, смена класса очищает оба выбора;
- Thesis stable id сохраняется migration-backed полем и воспроизводится read API;
- legacy Wizard без Thesis читается;
- wizard/review/details отображают Thesis и effects;
- feat, familiar, spell preparation, slots и staff mechanics не имитируются;
- backend/frontend checks, EF pending-model check и code review проходят без открытых замечаний.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes --project CharacterManagement.Infrastructure/CharacterManagement.Infrastructure.csproj --startup-project Pathfinder.Web/Pathfinder.Web.csproj --context CharacterManagementDbContext --no-build`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- отдельный review catalog completeness, effect/dependency boundaries, aggregate atomicity, API/legacy compatibility, migration scope, C# style и frontend reset behavior.

## Результат реализации и code review

Реализован полный вертикальный срез Arcane Thesis: пять Player Core Thesis, десять typed effect kinds с milestone levels, серверные инварианты совместного выбора School/Thesis, create/read API, nullable persistence, migration, wizard/review/details UI и локализация. Вложенные feat, familiar, spell и staff choices намеренно оставлены deferred до появления общих подсистем.

В ходе отдельного code review исправлены три замечания:

1. Новые `CharacterClassDependencyType` перенесены в конец enum, чтобы не менять числовые значения существующих членов.
2. Неизвестный `ArcaneThesisEffectKind` теперь отклоняется до проверки id shape; добавлен regression-тест.
3. Makeshift Staff получил недостающую явную зависимость `SpellPreparationRules`; top-level Wizard dependency closure закреплён тестом и больше не содержит `ClassChoiceCatalog`.

Review также подтвердил append-only расширение публичных сигнатур, атомарность aggregate mutation, читаемость legacy Wizard без Thesis, одну nullable migration column и отсутствие клиентской имитации downstream mechanics.

Проверки после review:

- domain tests: 144 passed;
- infrastructure/API tests: 161 passed;
- frontend tests: 46 passed;
- Web build, frontend lint и production build: passed;
- EF pending-model check и `git diff --check`: passed.

Открытых замечаний по подзадаче 1.7 не осталось.
