# Character Creation Known Gaps

## Назначение

Этот документ фиксирует доменные данные и каталоги, которых не хватает для полного покрытия character creation.

Он не описывает баги текущего кода. Для расхождений реализации читать [implementation_notes.md](implementation_notes.md).

## Что не блокирует MVP

Текущий реализованный flow включает имя, `Ancestry`, ancestry package, ancestry boosts/flaws, `Background` с двумя boosts, `Class` с key ability boost и четыре финальных свободных boosts.

Следующие gaps известны, но не блокируют текущий MVP:

- Feat inventory первого уровня участвует в MVP flow: ancestry/background/class feats выбираются или выдаются, валидируются и сохраняют provenance. Поддерживаемые постоянные Skill/Lore training effects применяются; parameterized, temporary, spell, action, combat, weapon и inventory effects остаются deferred. Equipment, languages и исполнение spell effects пока за границей MVP.
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
| `Ancestry`, background skill и class feats 1 уровня | Есть нормализованный Player Core catalog и общий character inventory | [../../30_task_notes/feat_catalog_player_core_implementation.md](../../30_task_notes/feat_catalog_player_core_implementation.md) |
| `Languages` | Есть нормализованный Player Core catalog и ancestry language rules | [language_catalog.md](language_catalog.md) |

## Частично готовые области

### Background

Baseline-каталог Core Rulebook реализован в C# и участвует в character creation. Skill и Lore grants применяются; fixed skill feat выдаётся в общий inventory, а выбор `Martial Disciple` валидируется и сохраняется.

Поддерживаемый cross-source feat training объединён с Background/Class training. Конфликт постоянного feat training возвращается как typed replacement requirement; сам replacement choice и higher proficiency ranks пока не реализованы.

Следующий ожидаемый документ:

- `background_catalog_cross_source.md`

### Class

Нормализованный каталог восьми классов реализован в C# и участвует в character creation. Class id, base HP, безусловная key ability и типизированные стартовые proficiencies доступны через API и frontend.

Starting proficiencies для Perception, saves, attacks, defenses и class DC вычисляются из выбранного класса и отображаются в карточке. Все mandatory class choices первого уровня, Cleric Deity, primary Domain и обязательные class/skill feat slots реализованы. Стартовые spell loadouts всех spellcasting-классов применяются; runtime casting и исполняемые combat class features пока отсутствуют.

### Spells And Cantrips

Реализован единый tradition-aware Player Core catalog и стартовые loadout flows Cleric, Bard, Druid, Witch и Wizard: repertoire, preparation, spellbook, granted spells, class-specific slots и focus packages. Runtime casting, расходование ресурсов, rest lifecycle и исполнение effects остаются отдельной подсистемой.

### Deities

Player Core каталог оформлен в [deities_player_core.md](deities_player_core.md), а Cleric обязан выбрать допустимый Deity package. Divine skill, favored weapon proficiency, Font и sanctification применяются. Cloistered Cleric выбирает primary domain из [cleric_domains_player_core.md](cleric_domains_player_core.md), получает разрешённый initial focus spell и derived Focus Pool с максимумом `1`. Deity-granted spells разрешаются через Cleric Spell Catalog v1 и доступны при подготовке только Cleric выбранного божества.

### Equipment And Starting Wealth

Есть отдельные упоминания стартовых предметов, например `Clan Dagger`, но полного каталога стартового снаряжения и starting wealth нет.

Следующий ожидаемый документ:

- `equipment_starting_wealth_catalog.md`

### Languages

Общий Player Core catalog, ancestry language rules, выбор и persistence дополнительных языков реализованы. Единый server-side completion report проверяет языки вместе с остальными обязательными пакетами. Источники access к uncommon языкам и явная финализация draft остаются открытыми.

### Ancestry Choice Effects

Heritage и ancestry feats 1 уровня каталогизированы и входят в общий feat inventory. Фиксированные training effects Lore feats и `Prairie Rider` применяются доменом; остальные эффекты остаются typed dependencies. Детальная матрица находится в [ancestry_choices_player_core.md](ancestry_choices_player_core.md).

Для оставшихся ancestry effects необходимы parameter/daily choices и будущие rule engines для weapons/inventory, languages, actions, combat conditions и resistances. Текущая реализация хранит выбор и явно обозначает эти зависимости, не имитируя отсутствующие механики.

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

1. Добавить явную финализацию character creation и persisted status.
2. Добавить starting equipment после фиксации inventory ownership boundary.
3. Поверх equipment расширить карточку до AC, attacks, damage, DC и combat HP state.
