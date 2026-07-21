# Equipment And Starting Wealth Catalog V1

## Scope

Этот нормативный catalog покрывает starting equipment восьми классов `Player Core`, поддерживаемых character creation: Bard, Cleric, Druid, Fighter, Ranger, Rogue, Witch и Wizard.

Catalog v1 включает:

- все base items и статические options этих class kits;
- все покупаемые favored weapons core deities;
- поля armor, weapon и shield, необходимые для будущих AC, strike и proficiency calculations;
- цену и Bulk всех definitions.

Произвольный полный ассортимент `Player Core`, consumables, magic items, runes и runtime economy не входят в v1.

## Sources

- `Player Core`, pages 25, 267–280 и 287–292;
- [AoN Character Creation: Buy Equipment](https://2e.aonprd.com/Rules.aspx?ID=66);
- [AoN Class Kits](https://2e.aonprd.com/ClassKits.aspx?ID=17) и отдельные kit pages `ID=18`–`24`;
- AoN Elasticsearch records с `primary_source = Player Core` для item definitions.

Некоторые базовые weapon records (`Spear`, `Staff`, `Starknife`, `Glaive`) в текущем AoN index всё ещё имеют legacy `Core Rulebook` primary source, хотя Player Core class-kit pages ссылаются на них. В catalog v1 они нормализованы к соответствующим Player Core tables и pages.

## Money And Purchase Units

- персонаж первого уровня получает `15 gp = 150 sp = 1500 cp`;
- сервер хранит и вычисляет starting price в целых copper pieces;
- итоговая стоимость не может превышать `1500 cp`;
- остаток валюты равен `1500 cp - total equipment price`;
- `Arrows`, `Bolts` и `Sling Bullets` продаются batches по 10 штук; `UnitsPerPurchase = 10`;
- ammunition definition хранит совместимую weapon group: `Bow`, `Crossbow` или `Sling`;
- quantity class-kit line означает число покупаемых catalog batches, а не число отдельных ammunition units;
- item с price `0` допустим, но всё равно должен присутствовать в catalog.

Common level-0 equipment из catalog доступен при character creation. Uncommon item требует typed access source. В v1 единственный uncommon purchasable definition — `equipment.spiked_chain`; доступ Cleric выводится из favored weapon выбранного deity, а не из клиентского флага.

## Bulk Representation

Bulk хранится как `BulkTenths`:

- `0` — negligible Bulk;
- `1` — light Bulk (`L`);
- `10` — `1 Bulk`.

Catalog хранит Bulk одного purchase batch. Правила суммирования light Bulk, worn/container adjustments и carrying capacity реализуются отдельным slice 7.4.

## Armor Catalog

| Stable id | Name | Price cp | Bulk tenths | Category | Group | AC | Dex cap | Check | Speed ft | Strength |
|---|---|---:|---:|---|---|---:|---:|---:|---:|---:|
| `equipment.explorers_clothing` | Explorer's Clothing | 10 | 1 | Unarmored | Cloth | 0 | 5 | 0 | 0 | 0 |
| `equipment.leather_armor` | Leather Armor | 200 | 10 | Light | Leather | 1 | 4 | -1 | 0 | 0 |
| `equipment.studded_leather_armor` | Studded Leather Armor | 300 | 10 | Light | Leather | 2 | 3 | -1 | 0 | 1 |
| `equipment.hide_armor` | Hide Armor | 200 | 20 | Medium | Leather | 3 | 2 | -2 | -5 | 2 |
| `equipment.scale_mail` | Scale Mail | 400 | 20 | Medium | Composite | 3 | 2 | -2 | -5 | 2 |
| `equipment.chain_mail` | Chain Mail | 600 | 20 | Medium | Chain | 4 | 1 | -2 | -5 | 3 |

Armor traits v1: Explorer's Clothing — `Comfort`; Chain Mail — `Flexible`, `Noisy`.

## Weapon Catalog

| Stable id | Name | Price cp | Bulk | Category | Group | Type | Damage | Hands | Range |
|---|---|---:|---:|---|---|---|---|---|---:|
| `equipment.dagger` | Dagger | 20 | 1 | Simple | Knife | Melee | d4 P | 1 | — |
| `equipment.rapier` | Rapier | 200 | 10 | Martial | Sword | Melee | d6 P | 1 | — |
| `equipment.sling` | Sling | 0 | 1 | Simple | Sling | Ranged | d6 B | 1 | 50 |
| `equipment.javelin` | Javelin | 10 | 1 | Simple | Dart | Ranged | d6 P | 1 | 30 |
| `equipment.spear` | Spear | 10 | 10 | Simple | Spear | Melee | d6 P | 1 | — |
| `equipment.greatsword` | Greatsword | 200 | 20 | Martial | Sword | Melee | d12 S | 2 | — |
| `equipment.longbow` | Longbow | 600 | 20 | Martial | Bow | Ranged | d8 P | 1+ | 100 |
| `equipment.longsword` | Longsword | 100 | 10 | Martial | Sword | Melee | d8 S | 1 | — |
| `equipment.shortsword` | Shortsword | 90 | 1 | Martial | Sword | Melee | d6 P | 1 | — |
| `equipment.sickle` | Sickle | 20 | 1 | Simple | Knife | Melee | d4 S | 1 | — |
| `equipment.staff` | Staff | 0 | 10 | Simple | Club | Melee | d4 B | 1 | — |
| `equipment.crossbow` | Crossbow | 300 | 10 | Simple | Crossbow | Ranged | d8 P | 2 | 120 |
| `equipment.mace` | Mace | 100 | 10 | Simple | Club | Melee | d6 B | 1 | — |
| `equipment.whip` | Whip | 10 | 10 | Martial | Flail | Melee | d4 S | 1 | — |
| `equipment.starknife` | Starknife | 200 | 1 | Martial | Knife | Melee | d4 P | 1 | — |
| `equipment.trident` | Trident | 100 | 10 | Martial | Spear | Melee | d8 P | 1 | — |
| `equipment.falchion` | Falchion | 300 | 20 | Martial | Sword | Melee | d10 S | 2 | — |
| `equipment.greataxe` | Greataxe | 200 | 20 | Martial | Axe | Melee | d12 S | 2 | — |
| `equipment.scimitar` | Scimitar | 100 | 10 | Martial | Sword | Melee | d6 S | 1 | — |
| `equipment.glaive` | Glaive | 100 | 20 | Martial | Polearm | Melee | d8 S | 2 | — |
| `equipment.warhammer` | Warhammer | 100 | 10 | Martial | Hammer | Melee | d8 B | 1 | — |
| `equipment.scythe` | Scythe | 200 | 20 | Martial | Polearm | Melee | d10 S | 2 | — |
| `equipment.spiked_chain` | Spiked Chain | 300 | 10 | Martial | Flail | Melee | d8 S | 2 | — |

Weapon traits сохраняются типизированным списком строк catalog definition. Parameter values остаются частью normalized trait label, например `Thrown 20 ft.`, `Deadly d8` и `Versatile Piercing`.

`Fist` и `Claw` являются unarmed attacks, а не покупаемым equipment, поэтому не входят в этот catalog.

## Ammunition, Gear And Shield

| Stable id | Name | Price cp | Bulk | Units per purchase |
|---|---|---:|---:|---:|
| `equipment.arrows` | Arrows | 10 | 1 | 10 |
| `equipment.bolts` | Bolts | 10 | 1 | 10 |
| `equipment.sling_bullets` | Sling Bullets | 1 | 1 | 10 |
| `equipment.adventurers_pack` | Adventurer's Pack | 150 | 10 | 1 |
| `equipment.climbing_kit` | Climbing Kit | 50 | 10 | 1 |
| `equipment.grappling_hook` | Grappling Hook | 10 | 1 | 1 |
| `equipment.healers_toolkit` | Healer's Toolkit | 500 | 10 | 1 |
| `equipment.cookware` | Cookware | 100 | 20 | 1 |
| `equipment.musical_instrument_handheld` | Musical Instrument (Handheld) | 80 | 10 | 1 |
| `equipment.primal_symbol` | Primal Symbol | 0 | 0 | 1 |
| `equipment.religious_symbol_wooden` | Religious Symbol (Wooden) | 10 | 1 | 1 |
| `equipment.thieves_toolkit` | Thieves' Toolkit | 300 | 1 | 1 |
| `equipment.writing_set` | Writing Set | 100 | 1 | 1 |
| `equipment.steel_shield` | Steel Shield | 200 | 10 | 1 |

Steel Shield дополнительно определяет AC bonus `2`, Hardness `5` и HP `20`. Shield AC применяется только через будущую Raise a Shield/runtime boundary, а не автоматически из факта владения.

## Class Kits

Все base totals вычисляются из item references. `Money left` не хранится в kit definition.

| Class | Base price cp | Base items | Options |
|---|---:|---|---|
| Bard | 752 | studded leather, dagger, rapier, sling, 20 bullets, adventurer's pack, handheld instrument | — |
| Cleric | 170 | explorer's clothing, adventurer's pack, wooden religious symbol | deity favored weapon; chain mail; healer's toolkit |
| Druid | 400 | hide armor, 4 javelins, spear, adventurer's pack, primal symbol | healer's toolkit |
| Fighter | 580 | scale mail, dagger, adventurer's pack, grappling hook | at most one: greatsword; longbow + 20 arrows; longsword + steel shield |
| Ranger | 370 | leather armor, dagger, adventurer's pack | at most one: longbow + 20 arrows; longsword + steel shield; 2 shortswords |
| Rogue | 620 | leather armor, dagger, rapier, adventurer's pack, climbing kit | thieves' toolkit |
| Witch | 182 | explorer's clothing, sickle, sling, 20 bullets, staff, adventurer's pack | cookware; healer's toolkit |
| Wizard | 260 | explorer's clothing, staff, adventurer's pack, writing set | crossbow + 20 bolts |

AoN Rogue kit page omits `climbing kit` from rendered Gear while retaining base price `620 cp` and Bulk `4 Bulk, 1 light`. Player Core equipment table includes it; catalog v1 keeps the item because it reconciles both published totals.

Cleric favored weapon is represented by typed dependency `DeityFavoredWeapon`. Конкретный item id выводится сервером из выбранного deity. Непокупаемые unarmed favored weapons не добавляют item line или cost.

## API Contract

- `GET /api/equipment` — catalog definitions, sorted by name;
- `GET /api/equipment/class-kits` — восемь class kits, starting wealth и typed option groups.

Оба endpoint требуют authentication. Catalog API не принимает цену, Bulk или игровые характеристики от клиента.
