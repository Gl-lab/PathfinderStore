# Character Creation Known Gaps

## Назначение

Этот документ фиксирует доменные данные и каталоги, которых не хватает для полного покрытия character creation.

Он не описывает баги текущего кода. Для расхождений реализации читать [implementation_notes.md](implementation_notes.md).

## Что не блокирует MVP

Текущий реализованный flow включает имя, `Ancestry`, ancestry package, ancestry boosts/flaws, `Background` с двумя boosts, `Class` с key ability boost и четыре финальных свободных boosts.

Следующие gaps известны, но не блокируют текущий MVP:

- Class skills, skill feats, spells, equipment, languages и ancestry feats не участвуют в MVP flow. Deity package и Background skill/Lore training уже применяются; domains и granted spells пока возвращаются декларативно.
- Из derived statistics вычисляется только maximum HP первого уровня; current/temporary HP, AC, saves, perception и другие значения пока отсутствуют.

## Каталоги, которые уже есть

| Область | Статус | Где смотреть |
|---|---|---|
| `Ancestry` base data | Есть нормализованный документ по 6 ancestry | [aon_player_core_ancestries_59_64.md](aon_player_core_ancestries_59_64.md) |
| `Background` baseline catalog | Есть baseline-каталог Core Rulebook | [background_catalog_core_rulebook.md](background_catalog_core_rulebook.md) |
| `Class` baseline catalog | Есть нормализованный каталог 8 классов Player Core | [class_catalog_player_core.md](class_catalog_player_core.md) |
| `Class` initial proficiencies | Есть типизированная матрица Perception, saves, attacks, defenses и class DC | [class_proficiencies_player_core.md](class_proficiencies_player_core.md) |
| `Deity` | Есть Player Core каталог и Cleric choice flow | [deities_player_core.md](deities_player_core.md) |
| `Skill list` | Есть отдельный каталог Player Core | [skill_catalog_player_core.md](skill_catalog_player_core.md) |
| `Classes`, `Backgrounds`, `Archetypes` | Есть источник-ориентированный сводный документ | [aon_player_core_character_creation_sources.md](aon_player_core_character_creation_sources.md) |
| `Heritage` и ancestry feats 1 уровня | Есть нормативный нормализованный каталог для 6 ancestry Player Core | [ancestry_choices_player_core.md](ancestry_choices_player_core.md) |
| `Class Feats` | Есть research-каталог | [../../90_research/tempinfo/aon_player_core_class_feats.md](../../90_research/tempinfo/aon_player_core_class_feats.md) |

## Частично готовые области

### Background

Baseline-каталог Core Rulebook реализован в C# и участвует в character creation. Skill и Lore grants типизированы и применяются; skill feat пока остаётся декларативным.

Нет объединённого cross-source применения class/ancestry grants, replacement UI для повторного training и higher proficiency ranks.

Следующий ожидаемый документ:

- `background_catalog_cross_source.md`

### Class

Нормализованный каталог восьми классов реализован в C# и участвует в character creation. Class id, base HP, безусловная key ability и типизированные стартовые proficiencies доступны через API и frontend.

Starting proficiencies для Perception, saves, attacks, defenses и class DC вычисляются из выбранного класса и отображаются в карточке. Rogue's Racket, Cleric Doctrine и Cleric Deity choice реализованы. Skills, spell proficiencies, class features, spells и остальные mandatory choices пока не применяются.

### Spells And Cantrips

Заклинания и cantrips упоминаются в class/ancestry feat docs, но отдельного каталога нет.

Следующий ожидаемый документ:

- `spell_catalog.md`

### Deities

Player Core каталог оформлен в [deities_player_core.md](deities_player_core.md), а Cleric обязан выбрать допустимый Deity package. Divine skill, favored weapon proficiency, Font и sanctification применяются; domains и granted spells остаются typed descriptors до отдельных spell/domain flows.

### Equipment And Starting Wealth

Есть отдельные упоминания стартовых предметов, например `Clan Dagger`, но полного каталога стартового снаряжения и starting wealth нет.

Следующий ожидаемый документ:

- `equipment_starting_wealth_catalog.md`

### Languages

В ancestry-доках есть стартовые языки и правила дополнительных языков, но общего language catalog нет.

Следующий ожидаемый документ:

- `language_catalog.md`

### Ancestry Choice Effects

Heritage и ancestry feats 1 уровня уже каталогизированы, но их эффекты пока не исполняются доменом. Детальная матрица зависимостей находится в [ancestry_choices_player_core.md](ancestry_choices_player_core.md).

В частности, необходимы будущие каталоги или rule engines для spells, classes/class feats, skills/Lore, weapons/inventory, languages, combat conditions и resistances. Задача #43 может хранить выбор и его декларативные effects, но не должна имитировать отсутствующие механики.

## Remastered Vs Current MVP

Текущий MVP использует упрощённый ancestry catalog. Remastered-источники уже показывают данные, которых нет в MVP:

- ancestry-granted items;
- ancestry-granted rules/features;
- starting languages;
- additional language rules;
- unified vision model.

Это не означает, что MVP нужно срочно расширять. Это означает, что новые задачи по `Ancestry`, sheet display, import/export или full character creation должны явно выбирать источник: сохранить упрощённый MVP или мигрировать данные на remastered baseline.

## Рекомендуемый порядок закрытия gaps

Актуальная ближайшая очередь и критерии готовности зафиксированы в [Character Creation Near-Term Roadmap](../../30_task_notes/character_creation_near_term_roadmap.md).

1. Реализовывать оставшиеся обязательные class choices отдельными flows.
2. Добавить cross-source proficiency resolver вместе с первым независимым источником или level-up progression.
3. Добавить languages, spells/cantrips и equipment по мере готовности соответствующих подсистем.
4. Расширять derived statistics и combat HP state отдельными vertical slices.
