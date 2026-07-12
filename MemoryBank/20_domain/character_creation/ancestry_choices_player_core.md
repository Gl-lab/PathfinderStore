# Ancestry Choices Player Core

## Назначение

Этот документ является нормативным нормализованным каталогом обязательных ancestry-выборов для шести базовых ancestry `Player Core`: heritage и ancestry feat 1 уровня.

Он переносит проверенные данные из [исследовательского каталога heritage](../../90_research/tempinfo/aon_player_core_ancestry_heritages.md) и [исследовательского каталога ancestry feats](../../90_research/tempinfo/aon_player_core_ancestry_feats.md). Дата исходной проверки: `2026-03-30`. Источник — Archives of Nethys, `Player Core`; ссылки на ancestry приведены в [aon_player_core_ancestries_59_64.md](aon_player_core_ancestries_59_64.md).

## Обязательные правила выбора

Для персонажа 1 уровня после выбора `Ancestry` должны быть подтверждены ровно один `Heritage` и ровно один ancestry feat уровня `1`, принадлежащие выбранному ancestry.

- `Jinxed Halfling` имеет редкость `Uncommon`, требует разрешения источника/GM и несовместим с `Halfling Luck`.
- Некоторые heritage и feats требуют последующего выбора из spell, skill, language, item, general-feat, class-feat или multiclass-dedication каталога.
- Клиент не передаёт такие выборы произвольной строкой: они должны ссылаться на соответствующий будущий каталог.

## Статусы эффектов

- `CatalogOnly` — выбор уже каталогизирован, но его механический эффект ждёт отдельной подсистемы.
- `RuleEffect` — формальное правило, действие, реакция, сопротивление или модификатор; нужен общий rule/effect engine.
- `VisionOverride` — меняет базовое зрение ancestry; может быть вычислен после введения `VisionType`.
- `BaseHpOverride` — меняет базовые ancestry HP; может быть вычислен в ancestry rules.
- `DeferredChoice` — нужен ещё один подтверждённый выбор из будущего каталога.

Ни один из этих статусов не означает, что эффект уже реализован в коде. В #43 сервер хранит и проверяет легальность выбора, не эмулируя несуществующие combat, spell или inventory правила.

## Общая нормализованная форма

Каждая запись будущего каталога содержит: `Id`, `AncestryType`, `Name`, `SourceBook`, `SourcePage`, `Level` для feat, `Rarity`, `Prerequisites`, `Restrictions`, `EffectKinds`, `DeferredDependencies`.

## Dwarf

Источник: `Player Core`, стр. `43-45`.

### Heritage

| Id | Name | EffectKinds | DeferredDependencies |
|---|---|---|---|
| `dwarf.ancient_blooded` | Ancient-Blooded Dwarf | `RuleEffect`: reaction, бонус к save против магии | rule/effect engine |
| `dwarf.death_warden` | Death Warden Dwarf | `RuleEffect`: улучшение save против void/undead | rule/effect engine, traits |
| `dwarf.forge` | Forge Dwarf | `RuleEffect`: fire resistance и защита от environmental heat | resistances, environment |
| `dwarf.rock` | Rock Dwarf | `RuleEffect`: защита от forced movement | combat movement, saves |
| `dwarf.strong_blooded` | Strong-Blooded Dwarf | `RuleEffect`: poison resistance и affliction outcome | affliction/resistance |

### Ancestry feats 1 уровня

| Id | Name | Prerequisites / restrictions | EffectKinds | DeferredDependencies |
|---|---|---|---|---|
| `dwarf.dwarven_doughtiness` | Dwarven Doughtiness | — | `RuleEffect` | conditions |
| `dwarf.dwarven_lore` | Dwarven Lore | — | `DeferredChoice` | skill, lore catalogs |
| `dwarf.dwarven_weapon_familiarity` | Dwarven Weapon Familiarity | — | `RuleEffect` | weapon, proficiency |
| `dwarf.mountain_strategy` | Mountain Strategy | — | `RuleEffect` | combat, creature traits |
| `dwarf.rock_runner` | Rock Runner | — | `RuleEffect` | movement, terrain |
| `dwarf.stonemasons_eye` | Stonemason's Eye | — | `RuleEffect` | skills, perception |
| `dwarf.unburdened_iron` | Unburdened Iron | — | `RuleEffect` | armor, encumbrance |

## Elf

Источник: `Player Core`, стр. `47-50`.

### Heritage

| Id | Name | EffectKinds | DeferredDependencies |
|---|---|---|---|
| `elf.ancient` | Ancient Elf | `DeferredChoice`: multiclass dedication другого class | class, class-feat, archetype catalogs |
| `elf.arctic` | Arctic Elf | `RuleEffect`: cold resistance и environmental cold | resistances, environment |
| `elf.cavern` | Cavern Elf | `VisionOverride`: darkvision | `VisionType` |
| `elf.seer` | Seer Elf | `DeferredChoice`, `RuleEffect`: `detect magic` и bonuses | spell, skill/rule engine |
| `elf.whisper` | Whisper Elf | `RuleEffect`: Seek и hearing effects | perception, conditions |
| `elf.woodland` | Woodland Elf | `RuleEffect`: movement и cover в forest | movement, terrain |

### Ancestry feats 1 уровня

| Id | Name | Prerequisites / restrictions | EffectKinds | DeferredDependencies |
|---|---|---|---|---|
| `elf.ancestral_longevity` | Ancestral Longevity | — | `DeferredChoice` | skills, daily rule engine |
| `elf.elven_lore` | Elven Lore | — | `DeferredChoice` | skill, lore catalogs |
| `elf.elven_weapon_familiarity` | Elven Weapon Familiarity | — | `RuleEffect` | weapon, proficiency |
| `elf.forlorn` | Forlorn | — | `RuleEffect` | emotion/mental effects |
| `elf.nimble_elf` | Nimble Elf | — | `RuleEffect` | speed calculation |
| `elf.otherworldly_magic` | Otherworldly Magic | — | `DeferredChoice` | spell catalog |
| `elf.unwavering_mien` | Unwavering Mien | — | `RuleEffect` | saves, emotion |

## Gnome

Источник: `Player Core`, стр. `51-54`.

### Heritage

| Id | Name | EffectKinds | DeferredDependencies |
|---|---|---|---|
| `gnome.chameleon` | Chameleon Gnome | `RuleEffect` | actions, stealth |
| `gnome.fey_touched` | Fey-Touched Gnome | `DeferredChoice`, `RuleEffect` | spell catalog, spellcasting |
| `gnome.sensate` | Sensate Gnome | `RuleEffect` | senses, perception |
| `gnome.umbral` | Umbral Gnome | `VisionOverride`: darkvision | `VisionType` |
| `gnome.wellspring` | Wellspring Gnome | `DeferredChoice`, `RuleEffect` | spell tradition, spell catalog |

### Ancestry feats 1 уровня

| Id | Name | Prerequisites / restrictions | EffectKinds | DeferredDependencies |
|---|---|---|---|---|
| `gnome.animal_accomplice` | Animal Accomplice | — | `RuleEffect` | animal companion/familiar |
| `gnome.animal_elocutionist` | Animal Elocutionist | — | `RuleEffect` | communicate-with-animals rules |
| `gnome.fey_fellowship` | Fey Fellowship | — | `RuleEffect` | diplomacy, creature traits |
| `gnome.first_world_magic` | First World Magic | — | `DeferredChoice` | spell catalog |
| `gnome.gnome_obsession` | Gnome Obsession | — | `DeferredChoice` | skill, lore catalogs |
| `gnome.gnome_weapon_familiarity` | Gnome Weapon Familiarity | — | `RuleEffect` | weapon, proficiency |
| `gnome.razzle_dazzle` | Razzle-Dazzle | — | `RuleEffect` | performance, conditions |

## Goblin

Источник: `Player Core`, стр. `55-57`.

### Heritage

| Id | Name | EffectKinds | DeferredDependencies |
|---|---|---|---|
| `goblin.charhide` | Charhide Goblin | `RuleEffect` | resistances, persistent damage |
| `goblin.irongut` | Irongut Goblin | `RuleEffect` | survival, affliction/conditions |
| `goblin.razortooth` | Razortooth Goblin | `RuleEffect` | unarmed attack, weapons |
| `goblin.snow` | Snow Goblin | `RuleEffect` | resistances, environment |
| `goblin.unbreakable` | Unbreakable Goblin | `BaseHpOverride`, `RuleEffect` | HP calculation, falling |

### Ancestry feats 1 уровня

| Id | Name | Prerequisites / restrictions | EffectKinds | DeferredDependencies |
|---|---|---|---|---|
| `goblin.burn_it` | Burn It! | — | `RuleEffect` | fire/spell damage |
| `goblin.goblin_lore` | Goblin Lore | — | `DeferredChoice` | skill, lore catalogs |
| `goblin.goblin_scuttle` | Goblin Scuttle | — | `RuleEffect` | reaction, movement |
| `goblin.goblin_song` | Goblin Song | — | `RuleEffect` | performance, conditions |
| `goblin.goblin_weapon_familiarity` | Goblin Weapon Familiarity | — | `RuleEffect` | weapon, proficiency |
| `goblin.rough_rider` | Rough Rider | — | `RuleEffect` | general feat, animal companion |
| `goblin.very_sneaky` | Very Sneaky | — | `RuleEffect` | stealth, conditions |

## Halfling

Источник: `Player Core`, стр. `59-62`.

### Heritage

| Id | Name | EffectKinds | DeferredDependencies |
|---|---|---|---|
| `halfling.gutsy` | Gutsy Halfling | `RuleEffect` | emotion, saves |
| `halfling.hillock` | Hillock Halfling | `RuleEffect` | healing/rest |
| `halfling.jinxed` | Jinxed Halfling | `RuleEffect`, `DeferredChoice` | uncommon availability, action, class/spell DC, feat restriction |
| `halfling.nomadic` | Nomadic Halfling | `DeferredChoice` | language catalog |
| `halfling.twilight` | Twilight Halfling | `VisionOverride`: low-light vision | `VisionType` |
| `halfling.wildwood` | Wildwood Halfling | `RuleEffect` | movement, terrain |

### Ancestry feats 1 уровня

| Id | Name | Prerequisites / restrictions | EffectKinds | DeferredDependencies |
|---|---|---|---|---|
| `halfling.distracting_shadows` | Distracting Shadows | — | `RuleEffect` | stealth, cover, size |
| `halfling.halfling_lore` | Halfling Lore | — | `DeferredChoice` | skill, lore catalogs |
| `halfling.halfling_luck` | Halfling Luck | incompatible with `halfling.jinxed` | `RuleEffect` | reroll rule engine |
| `halfling.halfling_weapon_familiarity` | Halfling Weapon Familiarity | — | `RuleEffect` | weapon, proficiency |
| `halfling.prairie_rider` | Prairie Rider | — | `RuleEffect` | skill, animal companion |
| `halfling.sure_feet` | Sure Feet | — | `RuleEffect` | movement, conditions |

## Human

Источник: `Player Core`, стр. `63-65`.

### Heritage

| Id | Name | EffectKinds | DeferredDependencies |
|---|---|---|---|
| `human.skilled` | Skilled Human | `DeferredChoice`, `RuleEffect` | skill/proficiency progression |
| `human.versatile` | Versatile Human | `DeferredChoice` | general feat catalog and prerequisites |

### Ancestry feats 1 уровня

| Id | Name | Prerequisites / restrictions | EffectKinds | DeferredDependencies |
|---|---|---|---|---|
| `human.adapted_cantrip` | Adapted Cantrip | spellcasting class feature | `DeferredChoice` | class, spell catalog |
| `human.cooperative_nature` | Cooperative Nature | — | `RuleEffect` | Aid/check rules |
| `human.general_training` | General Training | general-feat prerequisites | `DeferredChoice` | general feat catalog |
| `human.natural_ambition` | Natural Ambition | class feat availability | `DeferredChoice` | class, class-feat catalog |
| `human.natural_skill` | Natural Skill | — | `DeferredChoice` | skill catalog |
| `human.unconventional_weaponry` | Unconventional Weaponry | valid uncommon weapon | `DeferredChoice` | weapon/proficiency catalog |

## Граница реализации задачи #43

Задача #43 использует этот каталог как источник истины для принадлежности heritage и feat к ancestry, обязательности выбора на 1 уровне, уровня feat, редкости, явных несовместимостей и выдачи в API декларативных effects и запросов дополнительных choices.

Не объявлять реализованными до появления соответствующих подсистем: spells и innate spells; class feats, multiclass dedication и general feats; skills/Lore/proficiency; weapons, armor, unarmed attacks и inventory; languages; combat, conditions, saves, resistances, terrain и action economy.

`Clan Dagger` dwarf остаётся базовым ancestry-granted item из [aon_player_core_ancestries_59_64.md](aon_player_core_ancestries_59_64.md). Его выдача ждёт item/inventory catalog; ancestry-каталог хранит декларацию grant, а не создаёт предмет вручную в UI.
