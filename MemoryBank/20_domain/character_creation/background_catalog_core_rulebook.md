# Pathfinder 2e Background Catalog

Дата сбора: 2026-04-04.

Источник данных:
- [Archives of Nethys Backgrounds](https://2e.aonprd.com/Backgrounds.aspx)
- AoN Elasticsearch catalog

## Scope

Этот документ фиксирует базовый каталог backgrounds из `Core Rulebook` как стабильную baseline-выборку для character creation.

Важно:
- AoN в поисковом индексе помечает эти записи как `Core Rulebook`;
- в проектной переписке и ранних заметках этот baseline часто назывался `Player Core`;
- чтобы не смешивать разные редакции, дополнительные backgrounds из `Player Core 2`, APG, PFS и adventure guides сюда не включены.

## Краткий вывод

В baseline-каталоге AoN найдено `35` backgrounds.

Общие инварианты:
- каждый background дает `2` attribute boosts;
- один из boosts является свободным;
- каждый background дает `1` skill feat;
- каждый background дает `2` trained skills, причем один из них обычно `Lore`-типа;
- если background skill конфликтует с class-granted skill, применяется правило замены на другой skill по выбору.

## Каталог

| Name | Page | Attribute boosts | Trained skills | Skill feat |
|---|---:|---|---|---|
| Acolyte | 60 | Intelligence or Wisdom + 1 free | Religion; Scribing Lore | Student of the Canon |
| Acrobat | 60 | Strength or Dexterity + 1 free | Acrobatics; Circus Lore | Steady Balance |
| Animal Whisperer | 60 | Wisdom or Charisma + 1 free | Nature; Lore related to one terrain inhabited by animals you like | Train Animal |
| Artisan | 60 | Strength or Intelligence + 1 free | Crafting; Guild Lore | Specialty Crafting |
| Artist | 60 | Dexterity or Charisma + 1 free | Crafting; Art Lore | Specialty Crafting |
| Barkeep | 60 | Constitution or Charisma + 1 free | Diplomacy; Alcohol Lore | Hobnobber |
| Barrister | 60 | Intelligence or Charisma + 1 free | Diplomacy; Legal Lore | Group Impression |
| Bounty Hunter | 61 | Strength or Wisdom + 1 free | Survival; Legal Lore | Experienced Tracker |
| Charlatan | 61 | Intelligence or Charisma + 1 free | Deception; Underworld Lore | Charming Liar |
| Criminal | 61 | Dexterity or Intelligence + 1 free | Stealth; Underworld Lore | Experienced Smuggler |
| Detective | 61 | Intelligence or Wisdom + 1 free | Society; Underworld Lore | Streetwise |
| Emissary | 61 | Intelligence or Charisma + 1 free | Society; Lore related to one city you've visited often | Multilingual |
| Entertainer | 61 | Dexterity or Charisma + 1 free | Performance; Theater Lore | Fascinating Performance |
| Farmhand | 62 | Constitution or Wisdom + 1 free | Athletics; Farming Lore | Assurance |
| Field Medic | 62 | Constitution or Wisdom + 1 free | Medicine; Warfare Lore | Battle Medicine |
| Fortune Teller | 62 | Intelligence or Charisma + 1 free | Occultism; Fortune-Telling Lore | Oddity Identification |
| Gambler | 62 | Dexterity or Charisma + 1 free | Deception; Games Lore | Lie to Me |
| Gladiator | 62 | Strength or Charisma + 1 free | Performance; Gladiatorial Lore | Impressive Performance |
| Guard | 62 | Strength or Charisma + 1 free | Intimidation; Legal Lore or Warfare Lore | Quick Coercion |
| Herbalist | 62 | Constitution or Wisdom + 1 free | Nature; Herbalism Lore | Natural Medicine |
| Hermit | 62 | Constitution or Intelligence + 1 free | Nature or Occultism; Lore related to the terrain you lived in as a hermit | Dubious Knowledge |
| Hunter | 62 | Dexterity or Wisdom + 1 free | Survival; Tanning Lore | Survey Wildlife |
| Laborer | 62 | Strength or Constitution + 1 free | Athletics; Labor Lore | Hefty Hauler |
| Merchant | 63 | Intelligence or Charisma + 1 free | Diplomacy; Mercantile Lore | Bargain Hunter |
| Martial Disciple | 63 | Strength or Dexterity + 1 free | Acrobatics; Athletics; Warfare Lore | Cat Fall or Quick Jump |
| Miner | 63 | Strength or Wisdom + 1 free | Survival; Mining Lore | Terrain Expertise |
| Nomad | 63 | Constitution or Wisdom + 1 free | Survival; Lore related to one terrain you traveled in | Assurance |
| Noble | 63 | Intelligence or Charisma + 1 free | Society; Genealogy Lore or Heraldry Lore | Courtly Graces |
| Prisoner | 63 | Strength or Constitution + 1 free | Stealth; Underworld Lore | Experienced Smuggler |
| Sailor | 63 | Strength or Dexterity + 1 free | Athletics; Sailing Lore | Underwater Marauder |
| Scholar | 63 | Intelligence or Wisdom + 1 free | Academia Lore | Assurance |
| Scout | 64 | Dexterity or Wisdom + 1 free | Survival; Lore related to one terrain you scouted in | Forager |
| Street Urchin | 64 | Dexterity or Constitution + 1 free | Thievery; Lore for the city you lived in as a street urchin | Pickpocket |
| Tinker | 64 | Dexterity or Intelligence + 1 free | Crafting; Engineering Lore | Specialty Crafting |
| Warrior | 64 | Strength or Constitution + 1 free | Intimidation; Warfare Lore | Intimidating Glare |

## Notes on lore-style backgrounds

Several backgrounds use a flexible `Lore` slot instead of a fixed lore name.
That matters for the domain model because the actual `Lore` skill instance is generated from the background story, not selected from a fixed list.

Examples:
- `Animal Whisperer`
- `Bandit`-style terrain backgrounds
- `Emissary`
- `Hermit`
- `Nomad`
- `Scout`
- `Street Urchin`

## What this file does not cover yet

- `Player Core 2` backgrounds
- APG backgrounds
- PFS or adventure-specific backgrounds
- any cross-source deduplication or remaster migration rules

If we want the full cross-source AoN catalog later, the next step should be a separate combined index that groups entries by `primary_source` and marks which ones are legal in the current project scope.
