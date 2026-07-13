# Class Catalog — Player Core

## Назначение

Документ фиксирует C#-готовый baseline для первого class package в character creation. Он нормализует восемь классов `Player Core`, их key ability boost, base HP и декларативные стартовые rules.

Нормализация сверена 2026-07-13 со страницами Archives of Nethys, перечисленными в [`aon_player_core_character_creation_sources.md`](aon_player_core_character_creation_sources.md), по workflow [`../../10_workflow/aon_elasticsearch_usage.md`](../../10_workflow/aon_elasticsearch_usage.md).

Типизированная матрица Perception, saves, attacks, defenses и class DC вынесена в [`class_proficiencies_player_core.md`](class_proficiencies_player_core.md).

## Граница

Каталог позволяет:

- выбрать класс;
- проверить безусловно доступную key ability;
- применить один class boost;
- показать base HP и стартовые rules.

Каталог не применяет proficiencies, features, spells или обязательные branch choices. Они возвращаются как декларативные descriptors до появления соответствующих подсистем.

## Стабильные идентификаторы

| Class | Id | Page | Base HP | Доступные key abilities | Spell tradition |
|---|---|---:|---:|---|---|
| Bard | `class.bard` | 94 | 8 | `Charisma` | `Occult` |
| Cleric | `class.cleric` | 108 | 8 | `Wisdom` | `Divine` |
| Druid | `class.druid` | 122 | 8 | `Wisdom` | `Primal` |
| Fighter | `class.fighter` | 136 | 10 | `Strength`, `Dexterity` | — |
| Ranger | `class.ranger` | 152 | 10 | `Strength`, `Dexterity` | — |
| Rogue | `class.rogue` | 164 | 8 | `Dexterity` | — |
| Witch | `class.witch` | 178 | 6 | `Intelligence` | определяется Patron |
| Wizard | `class.wizard` | 192 | 6 | `Intelligence` | `Arcane` |

`Source.Book` для всех записей — `Player Core`.

## Rogue key ability boundary

Правило Rogue формулируется как `Dexterity or an option from rogue's racket`. Поэтому текущий каталог:

- включает `Dexterity` в `KeyAbilityOptions`;
- добавляет обязательный descriptor `class_choice.rogue.racket`;
- добавляет dependency `class_choice_catalog.rogue_racket` для альтернативной key ability;
- не возвращает объединённый список racket-зависимых характеристик как свободный выбор.

Расширение key ability options Rogue выполняется только вместе с отдельным Racket flow.

## Типы декларативных rules

- `InitialProficiencies` — человекочитаемая сводка стартовых proficiency ranks;
- `AdditionalSkills` — формула числа дополнительных trained skills;
- `GrantedFeature` — class feature первого уровня;
- `MandatoryChoice` — обязательный branch choice;
- `Spellcasting` — наличие и tradition spellcasting;
- `ClassFeatChoice` — выбор class feat;
- `SkillFeatChoice` — выбор skill feat.

Каждый descriptor содержит:

- stable `Id`;
- `Kind`;
- fallback `Name`;
- `Summary`;
- `RequiresChoice`;
- `DeferredDependencies`.

## Общие dependency ids

- `proficiency_rules`;
- `skill_catalog`;
- `class_feature_rules`;
- `class_choice_catalog`;
- `class_feat_catalog`;
- `skill_feat_catalog`;
- `spell_catalog`;
- `deity_catalog`;
- `familiar_rules`.

Специализированные branch dependencies используют суффикс, например `class_choice_catalog.rogue_racket`.

## Нормализованные rules

### Bard

- `class.bard.initial_proficiencies` — expert Perception; trained Fortitude/Reflex, expert Will; trained Occultism, Performance, simple/martial weapons, unarmed attacks, light armor, unarmored defense, bard class DC и spell attack/DC.
- `class.bard.additional_skills` — `4 + Intelligence modifier` дополнительных trained skills.
- `class.bard.spellcasting` — Occult spellcasting, spell repertoire и composition spells.
- `class_choice.bard.muse` — обязательный Muse; влияет на class feat и spell repertoire.

### Cleric

- `class.cleric.initial_proficiencies` — trained Perception/Fortitude/Reflex, expert Will; trained Religion, simple weapons, deity favored weapon, unarmed attacks, unarmored defense, cleric class DC и spell attack/DC; armor зависит от Doctrine.
- `class.cleric.additional_skills` — `2 + Intelligence modifier` дополнительных trained skills.
- `class.cleric.spellcasting` — Divine spellcasting и Divine Font.
- `class_choice.cleric.deity` — обязательный Deity; влияет на trained skill, favored weapon и spells.
- `class_choice.cleric.doctrine` — обязательный Doctrine.

### Druid

- `class.druid.initial_proficiencies` — trained Perception/Fortitude/Reflex, expert Will; trained Nature, simple weapons, unarmed attacks, light/medium armor и unarmored defense; дополнительный skill зависит от Order.
- `class.druid.additional_skills` — `2 + Intelligence modifier` дополнительных trained skills.
- `class.druid.spellcasting` — Primal spellcasting.
- `class.druid.features` — Anathema, Shield Block, Voice of Nature и Wildsong.
- `class_choice.druid.order` — обязательный Druidic Order; влияет на class feat, order spell и trained skill.

### Fighter

- `class.fighter.initial_proficiencies` — expert Perception/Fortitude/Reflex, trained Will; expert simple/martial weapons и unarmed attacks, trained advanced weapons, all armor и unarmored defense; выбор Acrobatics или Athletics.
- `class.fighter.additional_skills` — `3 + Intelligence modifier` дополнительных trained skills.
- `class.fighter.features` — Reactive Strike и Shield Block.
- `class_choice.fighter.feat` — обязательный fighter feat первого уровня.

### Ranger

- `class.ranger.initial_proficiencies` — expert Perception/Fortitude/Reflex, trained Will; trained Nature, Survival, simple/martial weapons, unarmed attacks, light/medium armor, unarmored defense и ranger class DC.
- `class.ranger.additional_skills` — `4 + Intelligence modifier` дополнительных trained skills.
- `class.ranger.features` — Hunt Prey.
- `class_choice.ranger.hunters_edge` — обязательный Hunter's Edge.
- `class_choice.ranger.feat` — обязательный ranger feat первого уровня.

### Rogue

- `class.rogue.initial_proficiencies` — expert Perception/Reflex/Will, trained Fortitude; trained Stealth, simple/martial weapons, unarmed attacks, light armor, unarmored defense и rogue class DC; дополнительные skills зависят от Racket.
- `class.rogue.additional_skills` — `7 + Intelligence modifier` дополнительных trained skills.
- `class.rogue.features` — Sneak Attack `1d6` и Surprise Attack.
- `class_choice.rogue.racket` — обязательный Rogue's Racket; влияет на skills и возможную альтернативную key ability.
- `class_choice.rogue.feat` — обязательный rogue feat первого уровня.
- `class_choice.rogue.skill_feat` — обязательный skill feat первого уровня.

### Witch

- `class.witch.initial_proficiencies` — trained Perception/Fortitude/Reflex, expert Will; trained simple weapons, unarmed attacks, unarmored defense, witch class DC и spell attack/DC; trained skill зависит от Patron.
- `class.witch.additional_skills` — `3 + Intelligence modifier` дополнительных trained skills.
- `class.witch.spellcasting` — tradition и spellcasting зависят от Patron.
- `class.witch.familiar` — Familiar и стартовые familiar spells.
- `class_choice.witch.patron` — обязательный Patron.

### Wizard

- `class.wizard.initial_proficiencies` — trained Perception/Fortitude/Reflex, expert Will; trained Arcana, simple weapons, unarmed attacks, unarmored defense, wizard class DC и spell attack/DC.
- `class.wizard.additional_skills` — `2 + Intelligence modifier` дополнительных trained skills.
- `class.wizard.spellcasting` — Arcane spellcasting.
- `class.wizard.features` — Arcane Bond.
- `class_choice.wizard.thesis` — обязательный Arcane Thesis.
- `class_choice.wizard.school` — обязательная Arcane School.

## Правила применения

1. `ClassId` обязан существовать в каталоге.
2. Выбранная key ability обязана входить в `KeyAbilityOptions` записи.
3. Сервер применяет один boost к выбранной key ability после background package.
4. Повторный выбор class package сначала откатывает старый class boost.
5. Декларативные rules не изменяют персонажа в текущем scope.
