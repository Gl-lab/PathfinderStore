# Cleric Doctrines — Player Core

## Назначение

Документ фиксирует C#-готовый baseline двух Cleric doctrines из `Player Core`. Данные сверены 2026-07-13 с [Archives of Nethys](https://2e.aonprd.com/Doctrines.aspx).

## Каталог

| Doctrine | Id | Page | Level 1 boundary |
|---|---|---:|---|
| Cloistered Cleric | `cleric_doctrine.cloistered` | 112 | Domain Initiate остаётся декларативным |
| Warpriest | `cleric_doctrine.warpriest` | 112 | три proficiency grants типизированы; Shield Block и Deadly Simplicity остаются декларативными |

Выбор doctrine обязателен для нового `class.cleric` и запрещён для остальных классов. Benefits doctrines после 1-го уровня не входят в текущий character creation flow.

## Proficiency grants

Cloistered Cleric не добавляет proficiency grants к общему Cleric baseline на 1-м уровне.

Warpriest добавляет:

| Grant id | Target | Rank |
|---|---|---|
| `cleric_doctrine.warpriest.proficiency.fortitude` | `proficiency.save.fortitude` | `Expert` |
| `cleric_doctrine.warpriest.proficiency.light_armor` | `proficiency.defense.light_armor` | `Trained` |
| `cleric_doctrine.warpriest.proficiency.medium_armor` | `proficiency.defense.medium_armor` | `Trained` |

Grants объединяются с class baseline через общий `ProficiencyResolver`: effective rank равен максимальному rank по target, а source ids сохраняются.

## Декларативные effects

| Effect id | Doctrine | Boundary | Deferred dependencies |
|---|---|---|---|
| `cleric_doctrine.cloistered.effect.domain_initiate` | Cloistered Cleric | выдаёт Domain Initiate и требует выбора domain | `ClassFeatCatalog`, `DomainCatalog`, `DeityCatalog` |
| `cleric_doctrine.warpriest.effect.shield_block` | Warpriest | выдаёт Shield Block | `GeneralFeatCatalog` |
| `cleric_doctrine.warpriest.effect.deadly_simplicity` | Warpriest | условно выдаёт Deadly Simplicity для simple или unarmed favored weapon | `ClassFeatCatalog`, `DeityCatalog`, `WeaponCatalog` |

Эти effects не применяются к агрегату до появления соответствующих нормативных каталогов и rules flows.

## Deity boundary

Doctrine flow не вводит строковую заглушку Deity. На этом этапе отсутствуют и не сохраняются:

- `SelectedDeityId`;
- divine skill и duplicate replacement;
- favored weapon reference, access и proficiency;
- Divine Font и выбор `heal`/`harm`;
- sanctification;
- domains и focus spells;
- granted spells.

`DeityCatalog` остаётся явной deferred dependency Cleric. Полноценный Deity flow начинается с отдельной нормализации каталога, после которой Deity станет обязательной частью нового Cleric package.

## Persistence и legacy

`SelectedClericDoctrineId` сохраняется как nullable stable id. Nullable нужен только для чтения уже существующих Cleric rows: новые create requests без Doctrine отклоняются. Proficiency grants и effects не сохраняются snapshot-ом и восстанавливаются из каталога по id.

## Исключённые правила

- doctrine progression на уровнях 3, 7, 11, 15 и 19;
- Deity choice и все deity-derived effects;
- фактическая выдача Domain Initiate, Shield Block и Deadly Simplicity;
- domain, feat, weapon и spell catalogs;
- универсальный framework class choices.
