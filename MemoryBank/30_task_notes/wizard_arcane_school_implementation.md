# Wizard Arcane School — Implementation Plan

## Цель

Реализовать вертикальный срез обязательного выбора Arcane School для Wizard: проверенный Player Core catalog, строгая серверная валидация, persistence, create/read API, wizard/review/details и тесты. Curriculum, school spells и особые benefits возвращаются как typed descriptors; spellbook, spell slots, focus pool, class feats и Drain Bonded Item пока не исполняются.

## Источники и baseline

Каталог отобран через AoN Elasticsearch по `category:arcane-school` и `primary_source:"Player Core"`, затем каждая запись сверена на официальной странице Archives of Nethys:

- [School of Ars Grammatica](https://2e.aonprd.com/ArcaneSchools.aspx?ID=16) — Player Core p. 198;
- [School of the Boundary](https://2e.aonprd.com/ArcaneSchools.aspx?ID=17) — p. 199;
- [School of Civic Wizardry](https://2e.aonprd.com/ArcaneSchools.aspx?ID=18) — p. 199;
- [School of Mentalism](https://2e.aonprd.com/ArcaneSchools.aspx?ID=19) — p. 200;
- [School of Protean Form](https://2e.aonprd.com/ArcaneSchools.aspx?ID=20) — p. 200;
- [School of Unified Magical Theory](https://2e.aonprd.com/ArcaneSchools.aspx?ID=21) — p. 200;
- [School of Battle Magic](https://2e.aonprd.com/ArcaneSchools.aspx?ID=22) — p. 199.

Stable ids используют `arcane_school.<snake_case>`. Spell references используют `spell.<snake_case>` и явно хранят rank; uncommon curriculum entries дополнительно помечаются `IsUncommon`.

## Нормализованный каталог

### School of Ars Grammatica

- `arcane_school.ars_grammatica`;
- ranks 0–2: `message`, `sigil`; `command`, `disguise_magic`, `runic_body`, `runic_weapon`; `dispel_magic`, `translate`;
- ranks 3–5: `enthrall`, `veil_of_privacy` (uncommon); `dispelling_globe` (uncommon), `suggestion`; `sending`, `truespeech` (uncommon);
- ranks 6–9: `repulsion`, `spellwrack`; `contingency`, `planar_seal` (uncommon); `quandary`, `unrelenting_observation`; `detonate_magic` (uncommon);
- initial school spell: `protective_wards`; advanced: `rune_of_observation`.

### School of the Boundary

- `arcane_school.boundary`;
- ranks 0–2: `telekinetic_hand`, `void_warp`; `grim_tendrils`, `phantasmal_minion`, `summon_undead`; `darkness`, `see_the_unseen`;
- ranks 3–5: `bind_undead`, `ghostly_weapon`; `flicker`, `translocate`; `banishment`, `invoke_spirits`;
- ranks 6–9: `teleport` (uncommon), `vampiric_exsanguination`; `eclipse_burst`, `interplanar_teleport` (uncommon); `quandary`, `unrelenting_observation`; `massacre`;
- initial: `fortify_summoning`; advanced: `spiral_of_horrors`.

### School of Civic Wizardry

- `arcane_school.civic_wizardry`;
- ranks 0–2: `prestidigitation`, `read_aura`; `hydraulic_push`, `pummeling_rubble`, `summon_construct`; `revealing_light`, `water_walk`;
- ranks 3–5: `cozy_cabin`, `safe_passage`; `creation`, `unfettered_movement`; `control_water`, `wall_of_stone`;
- ranks 6–9: `disintegrate`, `wall_of_force`; `planar_palace` (uncommon), `retrocognition`; `earthquake`, `pinpoint` (uncommon); `foresight`;
- initial: `earthworks`; advanced: `community_restoration`.

### School of Mentalism

- `arcane_school.mentalism`;
- ranks 0–2: `daze`, `figment`; `dizzying_colors`, `sleep`, `sure_strike`; `illusory_creature`, `stupefy`;
- ranks 3–5: `dream_message`, `mind_reading` (uncommon); `nightmare`, `vision_of_death`; `hallucination`, `illusory_scene`;
- ranks 6–9: `never_mind`, `phantasmal_calamity`; `project_image`, `warp_mind`; `disappearance`, `uncontrollable_dance`; `phantasmagoria`;
- initial: `charming_push`; advanced: `invisibility_cloak`.

### School of Protean Form

- `arcane_school.protean_form`;
- ranks 0–2: `gouging_claw`, `tangle_vine`; `jump`, `pest_form`, `spider_sting`; `enlarge`, `humanoid_form`;
- ranks 3–5: `feet_to_fins`, `vampiric_feast`; `mountain_resilience`, `vapor_form`; `elemental_form`, `toxic_cloud`;
- ranks 6–9: `cursed_metamorphosis`, `petrify`; `duplicate_foe`, `fiery_body`; `desiccate`, `monstrosity_form`; `metamorphosis`;
- initial: `scramble_body`; advanced: `shifting_form`.

### School of Battle Magic

- `arcane_school.battle_magic`;
- ranks 0–2: `shield`, `telekinetic_projectile`; `breathe_fire`, `force_barrage`, `mystic_armor`; `mist`, `resist_energy`;
- ranks 3–5: `earthbind`, `fireball`; `wall_of_fire`, `weapon_storm`; `howling_blizzard`, `impaling_spike`;
- ranks 6–9: `chain_lightning`, `disintegrate`; `energy_aegis`, `true_target`; `arctic_rift`, `desiccate`; `falling_stars`;
- initial: `force_bolt`; advanced: `energy_absorption`.

### School of Unified Magical Theory

- `arcane_school.unified_magical_theory`;
- curriculum отсутствует;
- initial school spell: `hand_of_the_apprentice`; advanced: `interdisciplinary_incantation`;
- typed deferred benefits:
  - `arcane_school.unified_magical_theory.benefit.extra_class_feat`;
  - `arcane_school.unified_magical_theory.benefit.extra_spellbook_spell_choice`;
  - `arcane_school.unified_magical_theory.benefit.additional_drain_bonded_item_uses`.

Выбор произвольного 1st-rank spell, фактическая выдача feat и изменение Drain Bonded Item не входят в этот срез: они требуют будущих Spellbook/Class Feat/Class Feature подсистем.

## Domain и application design

Добавить:

- `ArcaneSchool` с `Id`, `Name`, `Source`, `HasCurriculum`, `CurriculumSpells`, `Benefits`;
- `ArcaneSchoolCurriculumSpellDescriptor` с `Id`, `Name`, `Rank`, `IsUncommon`;
- `ArcaneSchoolBenefitKind`: `InitialSchoolSpell`, `AdvancedSchoolSpell`, `ExtraClassFeat`, `ExtraSpellbookSpellChoice`, `AdditionalDrainBondedItemUses`;
- `ArcaneSchoolBenefitDescriptor` с typed kind, summary и deferred dependencies;
- `IArcaneSchoolRepository` и immutable Player Core repository.

Инварианты:

- id начинается с `arcane_school.`;
- spell ids начинаются с `spell.`, rank находится в диапазоне 0–9, curriculum ids уникальны внутри школы;
- каждая школа имеет ровно один initial и один advanced school spell;
- обычная школа имеет curriculum ranks 0–9, Unified не имеет curriculum и содержит все три особых benefit kinds;
- advanced school spell является descriptor будущего feat path и не считается выданным на первом уровне.

## Aggregate, request и persistence

- добавить nullable `DraftCharacter.SelectedArcaneSchoolId`;
- `SetClassPackage` принимает optional `ArcaneSchool` в конце сигнатуры;
- Wizard требует School, non-Wizard запрещает School;
- валидация выполняется до `RemoveClassEffects`, смена класса очищает School;
- request/builder/handler/validator принимают только `ArcaneSchoolId`;
- EF mapping: одна nullable колонка длиной 100, migration только через `dotnet ef`;
- legacy Wizard без School остаётся читаемым, но новый create flow его не принимает.

## API и frontend

- `GET /api/classes/wizard/arcane-schools`;
- catalog DTO возвращает source, full normalized curriculum и typed benefits;
- character read package возвращает выбранную School тем же структурированным package;
- wizard показывает School select только для Wizard, curriculum по rank и typed benefits;
- review/details показывают School, initial/advanced school spell и особые Unified benefits;
- class change очищает School;
- никаких spellbook/slot/focus-pool mutations на клиенте или сервере.

После реализации school rule больше не ссылается на отсутствующий `ClassChoiceCatalog`, но top-level Wizard dependency сохраняется до завершения Arcane Thesis в 1.7.

## Этапы выполнения

1. Зафиксировать проверенный каталог и границы deferred mechanics.
2. Добавить domain model/repository и catalog tests.
3. Расширить aggregate/request/builder/handler/validator и atomicity tests.
4. Добавить DTO/mappers/use case/controller/IoC и API/read/legacy tests.
5. Добавить EF mapping и migration, проверить migration diff.
6. Добавить frontend types/helper/tests, select, curriculum/benefit display, review/details и localization.
7. Обновить roadmap и MemoryBank.
8. Выполнить отдельный code review, исправить замечания и повторить полный прогон перед коммитом.

## Review плана перед реализацией

Review выполнен до изменения production-кода.

1. **Полнота каталога.** Player Core содержит семь школ, а не шесть; Ars Grammatica включена в baseline.
2. **Unified exception.** Unified Magical Theory не получает искусственный normal-school shape. Отсутствие curriculum и три замещающих benefit явно моделируются.
3. **Level boundary.** Advanced school spell отображается как будущий benefit, но не выдаётся персонажу первого уровня.
4. **Spellbook boundary.** Произвольный spell Unified не выбирается без общего SpellCatalog/Spellbook flow; descriptor сохраняет обязательство без фиктивной механики.
5. **Thesis coexistence.** После 1.6 Wizard временно требует School, но ещё не Thesis. `ClassChoiceCatalog` остаётся у Wizard из-за незавершённой 1.7, а снимается с school rule.
6. **Atomicity и legacy.** Новый create требует School; invalid mutation не стирает предыдущий class package; legacy Wizard остаётся читаемым.
7. **API payload.** Full curriculum намеренно возвращается в catalog/read DTO: frontend не парсит rules text и не владеет справочником.
8. **Signature compatibility.** Новые optional параметры добавляются только в конец существующих публичных сигнатур.
9. **Одна persistence колонка.** Curriculum и benefits server-owned и не дублируются в БД; выбранная School полностью воспроизводится по stable id.

Открытых замечаний, блокирующих реализацию, после review нет.

## Критерии готовности

- catalog содержит ровно семь Player Core школ с проверенными curriculum/focus/Unified descriptors;
- Wizard невозможно создать без School, non-Wizard — с School;
- invalid mutation атомарна, смена класса очищает School;
- stable id сохраняется migration-backed полем и воспроизводится read API;
- legacy Wizard без School читается;
- wizard/review/details показывают School и typed package;
- spellbook, slots, focus pool, feats и Drain Bonded Item не имитируются;
- backend/frontend checks, EF pending-model check и code review проходят без открытых замечаний.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes --project CharacterManagement.Infrastructure/CharacterManagement.Infrastructure.csproj --startup-project Pathfinder.Web/Pathfinder.Web.csproj --context CharacterManagementDbContext --no-build`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- отдельный review catalog completeness, Unified exception, aggregate atomicity, API/legacy compatibility, migration scope, C# style и frontend reset behavior.

## Результат реализации и code review

Реализован полный вертикальный срез выбора Arcane School: семь проверенных Player Core школ, отдельная модель Unified Magical Theory, серверные инварианты, create/read API, nullable persistence, migration, wizard/review/details UI и локализация.

Финальный review подзадачи подтвердил:

- catalog и uncommon markers совпадают с зафиксированным нормативным baseline;
- advanced school spells и особые benefits Unified представлены только deferred descriptors и не имитируют отсутствующие spellbook, feat, focus-pool или class-feature mechanics;
- Wizard/non-Wizard validation выполняется до удаления прежних class effects;
- public optional parameters добавлены в конец сигнатур, legacy Wizard без School остаётся читаемым;
- migration добавляет только nullable `SelectedArcaneSchoolId`, pending model changes отсутствуют;
- frontend очищает School при смене класса и не содержит собственного справочника spells.

В ходе review исправлен порядок валидации `ArcaneSchoolBenefitKind`: неизвестное enum-значение теперь всегда отклоняется как неизвестный kind до вычисления требуемого id prefix.

Проверки после review:

- domain tests: 134 passed;
- infrastructure/API tests: 151 passed;
- frontend tests: 44 passed;
- Web build, frontend lint и production build: passed;
- EF pending-model check и `git diff --check`: passed.

Открытых замечаний по подзадаче 1.6 не осталось.
