# Character Creation Known Gaps

## Назначение

Этот документ фиксирует доменные данные и каталоги, которых не хватает для полного покрытия character creation.

Он не описывает баги текущего кода. Для расхождений реализации читать [implementation_notes.md](implementation_notes.md).

## Что не блокирует MVP

Текущий реализованный flow включает имя, `Ancestry`, ancestry package, ancestry boosts/flaws, `Background` с двумя boosts, `Class` с key ability boost и четыре финальных свободных boosts.

Следующие gaps известны, но не блокируют текущий MVP:

- Skill feats, class feats, equipment, languages и исполняемые ancestry feats не участвуют в MVP flow. Deity package, primary Domain choice Cloistered Cleric, spell loadout, derived Domain focus pool и Background/class skill training уже применяются; исполнение spell effects пока остаётся за границей MVP.
- Из derived statistics вычисляются maximum HP, Saves, Perception и modifiers всех general skills и сохранённых Lore entries первого уровня. Current/temporary HP, AC, attacks, damage, class/spell DC и equipment bonuses пока отсутствуют.

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

Starting proficiencies для Perception, saves, attacks, defenses и class DC вычисляются из выбранного класса и отображаются в карточке. Все mandatory class choices первого уровня, Cleric Deity и primary Domain choice реализованы. Spell preparation, feats и исполняемые class features пока не применяются.

### Spells And Cantrips

Для Cleric реализован отдельный каталог divine cantrips и spells первого уровня. Единого tradition-aware Player Core catalog для Bard, Druid, Witch и Wizard пока нет; repertoire, preparation, spellbook и связанные focus packages этих классов остаются незавершёнными.

Следующий ожидаемый шаг — обобщить существующий Cleric catalog без потери stable identity и deity/domain references, а затем подключать остальные class flows отдельными vertical slices.

### Deities

Player Core каталог оформлен в [deities_player_core.md](deities_player_core.md), а Cleric обязан выбрать допустимый Deity package. Divine skill, favored weapon proficiency, Font и sanctification применяются. Cloistered Cleric выбирает primary domain из [cleric_domains_player_core.md](cleric_domains_player_core.md), получает разрешённый initial focus spell и derived Focus Pool с максимумом `1`. Deity-granted spells разрешаются через Cleric Spell Catalog v1 и доступны при подготовке только Cleric выбранного божества.

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

1. Завершить spell loadout остальных spellcasting-классов первого уровня.
2. Ввести общую feat subsystem для ancestry, background skill и class feats.
3. Добавить languages и явную финализацию character creation.
4. Добавить starting equipment после фиксации inventory ownership boundary.
5. Поверх equipment расширить карточку до AC, attacks, damage, DC и combat HP state.
