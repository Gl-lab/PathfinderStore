# Class Initial Proficiencies — Player Core

## Назначение

Документ фиксирует типизированный baseline стартовых class proficiencies для восьми классов `Player Core`. Данные сверены 2026-07-13 с class pages Archives of Nethys.

Skills и proficiency grants, зависящие от class choices, в основную матрицу не входят. Spell attack/DC вынесены в отдельную tradition-aware матрицу ниже; Witch получает tradition из выбранного Patron.

## Stable targets

| Category | Target id | Name |
|---|---|---|
| Perception | `proficiency.perception` | Perception |
| SavingThrow | `proficiency.save.fortitude` | Fortitude |
| SavingThrow | `proficiency.save.reflex` | Reflex |
| SavingThrow | `proficiency.save.will` | Will |
| Attack | `proficiency.attack.simple_weapons` | Simple Weapons |
| Attack | `proficiency.attack.martial_weapons` | Martial Weapons |
| Attack | `proficiency.attack.advanced_weapons` | Advanced Weapons |
| Attack | `proficiency.attack.unarmed` | Unarmed Attacks |
| Defense | `proficiency.defense.unarmored` | Unarmored Defense |
| Defense | `proficiency.defense.light_armor` | Light Armor |
| Defense | `proficiency.defense.medium_armor` | Medium Armor |
| Defense | `proficiency.defense.heavy_armor` | Heavy Armor |
| ClassDc | `proficiency.class_dc.<class>` | `<Class> Class DC` |
| SpellAttack | `proficiency.spell_attack.<tradition>` | `<Tradition> Spell Attack` |
| SpellDc | `proficiency.spell_dc.<tradition>` | `<Tradition> Spell DC` |

Не перечисленный target считается `Untrained`. Поэтому явные `Untrained in all armor` у Cleric, Witch и Wizard не создают grants.

## Нормализованная матрица

Обозначения: `T` — `Trained`, `E` — `Expert`, `—` — grant отсутствует.

| Class | Perception | Fortitude | Reflex | Will | Simple | Martial | Advanced | Unarmed | Unarmored | Light | Medium | Heavy | Class DC |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| Bard | E | T | T | E | T | T | — | T | T | T | — | — | T |
| Cleric | T | T | T | E | T | — | — | T | T | — | — | — | T |
| Druid | T | T | T | E | T | — | — | T | T | T | T | — | T |
| Fighter | E | E | E | T | E | E | T | E | T | T | T | T | T |
| Ranger | E | E | E | T | T | T | — | T | T | T | T | — | T |
| Rogue | E | T | E | E | T | T | — | T | T | T | — | — | T |
| Witch | T | T | T | E | T | — | — | T | T | — | — | — | T |
| Wizard | T | T | T | E | T | — | — | T | T | — | — | — | T |

## Spellcasting proficiency

| Class | Tradition source | Spell Attack | Spell DC |
|---|---|---|---|
| Bard | Occult | T | T |
| Cleric | Divine | T | T |
| Druid | Primal | T | T |
| Witch | Selected Patron | T | T |
| Wizard | Arcane | T | T |

Fighter, Ranger и Rogue не получают spell proficiency без отдельного источника. Для Bard, Cleric, Druid и Wizard grants входят в class baseline. Witch grants разрешаются из сохранённого Patron choice, чтобы stable target соответствовал фактической tradition.

## Исключённые grants

- Cleric deity favored weapon зависит от Deity и не добавляется в baseline.
- Cleric armor может изменяться Doctrine и не добавляется без выбора Doctrine.
- Skills, включая fighter choice, deity/order/patron/racket skills, относятся к отдельным flows.
- Item/status/circumstance modifiers для spell attack/DC не входят в стартовый proficiency catalog и подключаются отдельными rule sources.
- Все level-up increases находятся за границей стартового baseline.

## Source ids

Все grants одного класса используют source id вида:

```text
class.<class>.initial_proficiencies
```

Class DC использует class-specific target вида:

```text
proficiency.class_dc.<class>
```

## Источники

- [Bard](https://2e.aonprd.com/Classes.aspx?ID=32)
- [Cleric](https://2e.aonprd.com/Classes.aspx?ID=33)
- [Druid](https://2e.aonprd.com/Classes.aspx?ID=34)
- [Fighter](https://2e.aonprd.com/Classes.aspx?ID=35)
- [Ranger](https://2e.aonprd.com/Classes.aspx?ID=36)
- [Rogue](https://2e.aonprd.com/Classes.aspx?ID=37)
- [Witch](https://2e.aonprd.com/Classes.aspx?ID=38)
- [Wizard](https://2e.aonprd.com/Classes.aspx?ID=39)
