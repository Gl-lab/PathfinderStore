# Character Creation Known Gaps

## Назначение

Этот документ фиксирует доменные данные и каталоги, которых не хватает для полного покрытия character creation.

Он не описывает баги текущего кода. Для расхождений реализации читать [implementation_notes.md](implementation_notes.md).

## Что не блокирует MVP

Текущий реализованный flow включает имя, `Ancestry`, ancestry package, ancestry boosts/flaws, `Background` с двумя boosts, `Class` с key ability boost и четыре финальных свободных boosts.

Следующие gaps известны, но не блокируют текущий MVP:

- Skills, spells, deity, equipment, languages и ancestry feats не участвуют в MVP flow.
- Derived statistics шире `AbilityScores` не считаются как часть текущего MVP.

## Каталоги, которые уже есть

| Область | Статус | Где смотреть |
|---|---|---|
| `Ancestry` base data | Есть нормализованный документ по 6 ancestry | [aon_player_core_ancestries_59_64.md](aon_player_core_ancestries_59_64.md) |
| `Background` baseline catalog | Есть baseline-каталог Core Rulebook | [background_catalog_core_rulebook.md](background_catalog_core_rulebook.md) |
| `Class` baseline catalog | Есть нормализованный каталог 8 классов Player Core | [class_catalog_player_core.md](class_catalog_player_core.md) |
| `Skill list` | Есть отдельный каталог Player Core | [skill_catalog_player_core.md](skill_catalog_player_core.md) |
| `Classes`, `Backgrounds`, `Archetypes` | Есть источник-ориентированный сводный документ | [aon_player_core_character_creation_sources.md](aon_player_core_character_creation_sources.md) |
| `Heritage` и ancestry feats 1 уровня | Есть нормативный нормализованный каталог для 6 ancestry Player Core | [ancestry_choices_player_core.md](ancestry_choices_player_core.md) |
| `Class Feats` | Есть research-каталог | [../../90_research/tempinfo/aon_player_core_class_feats.md](../../90_research/tempinfo/aon_player_core_class_feats.md) |

## Частично готовые области

### Background

Baseline-каталог Core Rulebook реализован в C# и участвует в character creation. Skill, Lore и skill feat пока выдаются только как декларативные grants.

Нет объединённого cross-source каталога и механизма фактического применения skill grants.

Следующий ожидаемый документ:

- `background_catalog_cross_source.md`

### Class

Нормализованный каталог восьми классов реализован в C# и участвует в character creation. Class id, base HP, безусловная key ability и декларативные стартовые rules доступны через API и frontend.

Starting proficiencies, class features, spells и mandatory choices пока не применяются. Racket-зависимая альтернативная key ability Rogue ожидает отдельный Rogue's Racket flow.

### Spells And Cantrips

Заклинания и cantrips упоминаются в class/ancestry feat docs, но отдельного каталога нет.

Следующий ожидаемый документ:

- `spell_catalog.md`

### Deities

Для `Cleric` deity является обязательной частью выбора, но отдельный каталог божеств не оформлен.

Следующий ожидаемый документ:

- `deity_catalog.md`

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

1. Добавить Hit Points как первый новый срез derived statistics; ability modifiers уже вычисляются из scores.
2. Типизировать skills/Lore и применить background grants.
3. Типизировать starting proficiencies классов.
4. Реализовывать обязательные class choices отдельными flows, начиная с Rogue's Racket.
5. Добавить languages, deity, spells/cantrips и equipment по мере готовности соответствующих подсистем.
