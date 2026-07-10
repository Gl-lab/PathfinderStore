# AoN Player Core Character Creation Sources

## Назначение

Документ фиксирует `Player Core`-данные из Archives of Nethys, которые нужны для проектирования процесса создания персонажа.

Этот документ дополняет [aon_player_core_ancestries_59_64.md](aon_player_core_ancestries_59_64.md) и отвечает на вопросы:

- какие классы входят в базовый `Player Core`-набор для выбора на старте;
- какие class-specific choices обязательны на `1st level`;
- какие инварианты есть у background;
- входят ли archetypes в базовый flow создания персонажа;
- какие `Player Core`-предметы уже нужны на старте из-за ancestry-правил.

Дата проверки источников: `2026-03-28`.

## Источники

### Общие страницы

- `Classes`: <https://2e.aonprd.com/Classes.aspx>
- `Backgrounds`: <https://2e.aonprd.com/Backgrounds.aspx>
- `Archetypes`: <https://2e.aonprd.com/Archetypes.aspx>
- `Clan Dagger`: <https://2e.aonprd.com/Weapons.aspx?ID=368&Redirected=1>

### Проверенные class pages (`Player Core`)

- `Bard`: <https://2e.aonprd.com/Classes.aspx?ID=32>
- `Cleric`: <https://2e.aonprd.com/Classes.aspx?ID=33>
- `Druid`: <https://2e.aonprd.com/Classes.aspx?ID=34>
- `Fighter`: <https://2e.aonprd.com/Classes.aspx?ID=35>
- `Ranger`: <https://2e.aonprd.com/Classes.aspx?ID=36>
- `Rogue`: <https://2e.aonprd.com/Classes.aspx?ID=37>
- `Witch`: <https://2e.aonprd.com/Classes.aspx?ID=38>
- `Wizard`: <https://2e.aonprd.com/Classes.aspx?ID=39>

### Проверенные `Player Core` background entries

- `Acrobat`: <https://2e.aonprd.com/Backgrounds.aspx?ID=407>
- `Acolyte`: <https://2e.aonprd.com/Backgrounds.aspx?ID=1>
- `Bandit`: <https://2e.aonprd.com/Backgrounds.aspx?ID=165>
- `Entertainer`: <https://2e.aonprd.com/Backgrounds.aspx?ID=13>
- `Hermit`: <https://2e.aonprd.com/Backgrounds.aspx?ID=21>
- `Noble`: <https://2e.aonprd.com/Backgrounds.aspx?ID=27>
- `Nomad`: <https://2e.aonprd.com/Backgrounds.aspx?ID=28>
- `Prisoner`: <https://2e.aonprd.com/Backgrounds.aspx?ID=29>
- `Scholar`: <https://2e.aonprd.com/Backgrounds.aspx?ID=36>
- `Street Urchin`: <https://2e.aonprd.com/Backgrounds.aspx?ID=33>

## Границы набора данных

Этот документ охватывает только то, что прямо относится к базовому `Player Core`-созданию персонажа:

- `Ancestry` уже вынесены в отдельный документ;
- здесь фиксируются `Class`, `Background`, `Archetype rules boundary` и один уже нужный стартовый предмет;
- `Heritage`, `Ancestry Feats`, `Class Feats`, стартовые spell lists, deity catalog и полный каталог `Background` поштучно ещё не извлечены целиком.

Если будущая задача требует полного мастера создания персонажа, эти каталоги нужно документировать отдельно.

## Нормализованный поток создания персонажа по Player Core

Минимальный flow, который следует из проверенных источников:

1. Выбрать `Ancestry`.
2. Выбрать `Background`.
3. Выбрать `Class`.
4. Применить ancestry boosts/flaws.
5. Применить background boosts.
6. Применить class key attribute boost.
7. Применить четыре финальных свободных boosts.
8. Зафиксировать class-specific choice на `1st level`.
9. Выдать стартовые ancestry-derived items или rules, если они есть.

Пункты `5-8` в текущем проекте ещё не реализованы как полный доменный pipeline, но `Player Core`-источник уже требует именно такую модель.

## Классы Player Core

По проверенным страницам `Player Core` в базовый набор стартовых классов входят:

- `Bard`
- `Cleric`
- `Druid`
- `Fighter`
- `Ranger`
- `Rogue`
- `Witch`
- `Wizard`

Это важно для проекта:

- не следует автоматически подтягивать классы из `Player Core 2`, `Battlecry!`, `Dark Archives` и других книг в базовый wizard создания персонажа;
- если UI должен работать строго по `Player Core`, набор стартовых классов должен быть ограничен этими восемью.

## Нормализованная модель класса

Для старта класса в проекте уже недостаточно только:

- `ClassType`
- `KeyAbility`
- `BaseHitPoints`

Минимально полезная модель для `Player Core`-создания должна включать ещё:

- `SourceBook`
- `SourcePage`
- `KeyAbilityOptions`
- `InitialPerception`
- `InitialSavingThrows`
- `InitialSkillTraining`
- `AdditionalSkillCountRule`
- `InitialWeaponProficiencies`
- `InitialArmorProficiencies`
- `SpellTradition` если класс кастует
- `FirstLevelMandatoryChoices`
- `FirstLevelGrantedFeatures`

## Нормализованные данные по классам

### Bard

- `SourceBook`: `Player Core`
- `SourcePage`: `94`
- `KeyAbilityOptions`: `Charisma`
- `BaseHitPoints`: `8`
- `SpellTradition`: `Occult`
- `AdditionalSkillCountRule`: `4 + Intelligence modifier`
- `FirstLevelMandatoryChoices`: `Muse`
- `FirstLevelGrantedFeatures`: `Bard Spellcasting`, `Spell Repertoire`, `Composition Spells`
- `CreationNotes`: muse на `1st level` даёт конкретный `1st-level bard feat` и добавляет spell в repertoire

### Cleric

- `SourceBook`: `Player Core`
- `SourcePage`: `108`
- `KeyAbilityOptions`: `Wisdom`
- `BaseHitPoints`: `8`
- `SpellTradition`: `Divine`
- `AdditionalSkillCountRule`: `2 + Intelligence modifier`
- `FirstLevelMandatoryChoices`: `Deity`, `Doctrine`
- `FirstLevelGrantedFeatures`: `Cleric Spellcasting`, `Divine Font`
- `CreationNotes`: deity даёт trained skill, favored weapon и добавляет spells; doctrine на `1st level` выбирается между `Cloistered Cleric` и `Warpriest`

### Druid

- `SourceBook`: `Player Core`
- `SourcePage`: `122`
- `KeyAbilityOptions`: `Wisdom`
- `BaseHitPoints`: `8`
- `SpellTradition`: `Primal`
- `AdditionalSkillCountRule`: `2 + Intelligence modifier`
- `FirstLevelMandatoryChoices`: `Druidic Order`
- `FirstLevelGrantedFeatures`: `Druid Spellcasting`, `Anathema`, `Shield Block`, `Voice of Nature`, `Wildsong`
- `CreationNotes`: order даёт class feat, order spell и дополнительный trained skill

### Fighter

- `SourceBook`: `Player Core`
- `SourcePage`: `136`
- `KeyAbilityOptions`: `Strength`, `Dexterity`
- `BaseHitPoints`: `10`
- `SpellTradition`: `None`
- `AdditionalSkillCountRule`: `3 + Intelligence modifier`
- `FirstLevelMandatoryChoices`: нет отдельного class branch choice кроме key ability route
- `FirstLevelGrantedFeatures`: `Reactive Strike`, `Fighter Feat`, `Shield Block`
- `CreationNotes`: если проект будет моделировать старт глубже, fighter сразу получает `fighter feat` на `1st level`

### Ranger

- `SourceBook`: `Player Core`
- `SourcePage`: `152`
- `KeyAbilityOptions`: `Strength`, `Dexterity`
- `BaseHitPoints`: `10`
- `SpellTradition`: `None`
- `AdditionalSkillCountRule`: `4 + Intelligence modifier`
- `FirstLevelMandatoryChoices`: `Hunter's Edge`
- `FirstLevelGrantedFeatures`: `Hunt Prey`, `Ranger Feat`
- `CreationNotes`: `Hunter's Edge` обязателен уже на `1st level`

### Rogue

- `SourceBook`: `Player Core`
- `SourcePage`: `164`
- `KeyAbilityOptions`: `Dexterity`, `Other from Rogue's Racket`
- `BaseHitPoints`: `8`
- `SpellTradition`: `None`
- `AdditionalSkillCountRule`: `7 + Intelligence modifier`
- `FirstLevelMandatoryChoices`: `Rogue's Racket`
- `FirstLevelGrantedFeatures`: `Sneak Attack 1d6`, `Surprise Attack`, `Rogue Feat`, `Skill Feat`
- `CreationNotes`: racket влияет и на skill training, и на доступный key attribute branch

### Witch

- `SourceBook`: `Player Core`
- `SourcePage`: `178`
- `KeyAbilityOptions`: `Intelligence`
- `BaseHitPoints`: `6`
- `SpellTradition`: `Depends on Patron`
- `AdditionalSkillCountRule`: `3 + Intelligence modifier`
- `FirstLevelMandatoryChoices`: `Patron`
- `FirstLevelGrantedFeatures`: `Familiar`, `Witch Spellcasting`, `Hexes`
- `CreationNotes`: tradition и часть стартовых правил зависят от patron; familiar начинает со списком известных cantrips/spells

### Wizard

- `SourceBook`: `Player Core`
- `SourcePage`: `192`
- `KeyAbilityOptions`: `Intelligence`
- `BaseHitPoints`: `6`
- `SpellTradition`: `Arcane`
- `AdditionalSkillCountRule`: `2 + Intelligence modifier`
- `FirstLevelMandatoryChoices`: `Arcane Thesis`, `Arcane School`
- `FirstLevelGrantedFeatures`: `Wizard Spellcasting`, `Arcane Bond`
- `CreationNotes`: school и thesis являются обязательными branch choices уже на старте

## Общие правила background

Страница `Backgrounds` на AoN фиксирует следующие инварианты `Player Core`:

- background выбирается на `1st level`;
- выбор постоянный и не меняется на поздних уровнях;
- каждый background даёт `2 ability boosts`;
- каждый background даёт `1 skill feat`;
- каждый background даёт `trained` в двух навыках;
- один из этих навыков обязательно является `Lore`-навыком;
- если background даёт навык, который класс тоже даёт как `trained` на `1st level`, персонаж вместо этого получает `trained` в другом навыке по выбору.

Следствие для проекта:

- `Background` должен быть полноценным справочным каталогом, а не просто набором flavor-полей;
- доменная модель должна уметь обрабатывать конфликт background skill vs class skill;
- background нельзя сводить только к boosts.

## Нормализованная модель background

Минимальная модель:

- `BackgroundType`
- `Name`
- `SourceBook`
- `SourcePage`
- `RequiredBoostOptionA`
- `RequiredBoostOptionB`
- `FreeBoostCount`
- `GrantedSkillTrainings`
- `GrantedLoreRule`
- `GrantedSkillFeat`
- `ConditionalSkillReplacementRule`

`ConditionalSkillReplacementRule` обязателен, так как прямо задан на странице `Backgrounds`.

## Проверенные примеры Player Core backgrounds

Ниже только те background entries, которые были отдельно проверены в этой сессии.

### Acrobat

- boost pair: `Strength or Dexterity` + `1 free`
- trained skill: `Acrobatics`
- lore: `Circus Lore`
- skill feat: `Steady Balance`

### Acolyte

- boost pair: `Intelligence or Wisdom` + `1 free`
- trained skill: `Religion`
- lore: `Scribing Lore`
- skill feat: `Student of the Canon`

### Bandit

- boost pair: `Dexterity or Charisma` + `1 free`
- trained skill: `Intimidation`
- lore: terrain-related lore
- skill feat: `Group Coercion`

### Entertainer

- boost pair: `Dexterity or Charisma` + `1 free`
- trained skill: `Performance`
- lore: `Theater Lore`
- skill feat: `Fascinating Performance`

### Hermit

- boost pair: `Constitution or Intelligence` + `1 free`
- trained skill: `Nature` or `Occultism`
- lore: terrain-related lore
- skill feat: `Dubious Knowledge`

### Noble

- boost pair: `Intelligence or Charisma` + `1 free`
- trained skill: `Society`
- lore: `Genealogy Lore` or `Heraldry Lore`
- skill feat: `Courtly Graces`

### Nomad

- boost pair: `Constitution or Wisdom` + `1 free`
- trained skill: `Survival`
- lore: terrain-related lore
- skill feat: `Assurance ( Survival )`

### Prisoner

- boost pair: `Strength or Constitution` + `1 free`
- trained skill: `Stealth`
- lore: `Underworld Lore`
- skill feat: `Experienced Smuggler`

### Scholar

- boost pair: `Intelligence or Wisdom` + `1 free`
- trained skill: one of `Arcana`, `Nature`, `Occultism`, `Religion`
- lore: `Academia Lore`
- skill feat: `Assurance` in chosen skill

### Street Urchin

- boost pair: `Dexterity or Constitution` + `1 free`
- trained skill: `Thievery`
- lore: city-related lore
- skill feat: `Pickpocket`

## Archetypes: граница применения

Страница `Archetypes` сама ссылается на `Player Core pg. 215` как на правила archetypes, но проверенный каталог показывает важную границу:

- общие правила archetypes входят в `Player Core`;
- большинство конкретных non-multiclass archetype entries на странице берутся из `Player Core 2` и других книг;
- multiclass archetypes на странице перечислены как отдельная группа.

Практический вывод для проекта:

- archetypes не должны входить в базовый wizard создания персонажа `1st level` по текущему `Player Core`-scope;
- archetype support следует проектировать как отдельную подсистему после MVP;
- для текущего `Player Core`-создания archetypes нужно явно считать `out of scope`, если только в проекте не будет отдельного решения поддержать их через optional build mode.

Это вывод по источнику и текущему скоупу проекта. Он не запрещает будущую поддержку archetypes, но запрещает смешивать её с базовым flow без отдельного доменного шага.

## Стартовые ancestry-derived предметы: Clan Dagger

Документ по ancestry уже фиксирует, что `Dwarf` получает `Clan Dagger` бесплатно при создании.

По странице оружия:

- `SourceBook`: `Player Core`
- `SourcePage`: `277`
- `Name`: `Clan Dagger`
- `Rarity`: `Uncommon`
- `Traits`: `Agile`, `Dwarf`, `Parry`, `Versatile B`
- `Price`: `2 gp`
- `Damage`: `1d4 piercing`
- `Bulk`: `L`
- `Hands`: `1`
- `Type`: `Melee`
- `Category`: `Simple`
- `Group`: `Knife`

Следствие для проекта:

- если ancestry выдаёт стартовый предмет автоматически, проекту нужен каталог хотя бы минимальных item snapshots;
- ancestry-derived grants не должны храниться только в текстовом описании ancestry.

## Доменные выводы

### 1. Класс в PF2e создаёт отдельную ветку обязательных решений

Даже в базовом `Player Core` почти каждый класс требует дополнительный выбор на старте:

- `Bard` -> `Muse`
- `Cleric` -> `Deity`, `Doctrine`
- `Druid` -> `Druidic Order`
- `Ranger` -> `Hunter's Edge`
- `Rogue` -> `Rogue's Racket`
- `Witch` -> `Patron`
- `Wizard` -> `Arcane Thesis`, `Arcane School`

Следствие:
- будущий wizard создания персонажа не может ограничиться полями `ClassId` и `KeyAbility`.

### 2. Background должен применяться после ancestry и до class-specific finalization

Поскольку background даёт boosts и skill training, а класс тоже даёт training на `1st level`, порядок вычисления должен быть воспроизводимым и одинаковым на бэке и фронте.

### 3. Для полной Player Core-совместимости проекту нужен слой справочных каталогов

Минимально:

- `AncestryCatalog`
- `BackgroundCatalog`
- `ClassCatalog`
- `ItemCatalog` для ancestry-granted items

### 4. Archetypes нельзя случайно смешать с базовым creation flow

Иначе доменная модель быстро смешает:

- обязательный процесс `1st-level` создания;
- optional progression system;
- пост-стартовые build choices.

## Рекомендация для будущих задач

Следующий логичный шаг после ancestry:

1. Задокументировать `Player Core` heritages и `1st-level ancestry feats`.
2. Задокументировать полный каталог `Player Core` classes как DTO/source catalog.
3. Задокументировать полный каталог `Player Core` backgrounds поштучно.
4. Спроектировать единый `CharacterCreationPipeline`, где ancestry, background и class применяются как отдельные пакеты правил.
