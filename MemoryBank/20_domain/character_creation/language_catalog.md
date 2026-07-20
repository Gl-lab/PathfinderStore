# Player Core Language Catalog

## Назначение

Документ фиксирует нормализованный каталог языков для character creation первого уровня и правила ссылочной целостности ancestry language packages. Источником являются Player Core и актуальные remastered-страницы Archives of Nethys.

Дата проверки источников: `2026-07-20`.

## Источники

- [Languages, Player Core page 89](https://2e.aonprd.com/Rules.aspx?ID=2095)
- [Regional Languages, Player Core page 34](https://2e.aonprd.com/Rules.aspx?ID=2069)
- [Dwarf](https://2e.aonprd.com/Ancestries.aspx?ID=59)
- [Elf](https://2e.aonprd.com/Ancestries.aspx?ID=60)
- [Gnome](https://2e.aonprd.com/Ancestries.aspx?ID=61)
- [Goblin](https://2e.aonprd.com/Ancestries.aspx?ID=62)
- [Halfling](https://2e.aonprd.com/Ancestries.aspx?ID=63)
- [Human](https://2e.aonprd.com/Ancestries.aspx?ID=64)

## Модель

Каждый `LanguageDefinition` содержит:

- стабильный lowercase `LanguageId`;
- каноническое английское название;
- краткое описание типичных носителей;
- `LanguageRarity`: `Common` или `Uncommon`;
- `LanguageCategory`: `Standard` или `Regional`;
- ссылку на книгу и страницу источника.

Каталог является immutable code-owned reference data и не копируется в состояние персонажа. Ancestry packages и будущие пользовательские choices хранят только stable ids.

## Common languages

| Id | Name |
|---|---|
| `common` | Common |
| `draconic` | Draconic |
| `dwarven` | Dwarven |
| `elven` | Elven |
| `fey` | Fey |
| `gnomish` | Gnomish |
| `goblin` | Goblin |
| `halfling` | Halfling |
| `jotun` | Jotun |
| `orcish` | Orcish |
| `sakvroth` | Sakvroth |

## Uncommon languages

Стандартные uncommon ids: `aklo`, `chthonian`, `diabolic`, `empyrean`, `kholo`, `necril`, `petran`, `pyric`, `shadowtongue`, `sussuran`, `thalassic`, `muan`, `talican`.

Regional uncommon ids: `hallit`, `kelish`, `mwangi`, `osiriani`, `shoanti`, `skald`, `tien`, `varisian`, `vudrani`.

## Ancestry language rules

- `IntelligenceModifier` даёт `max(0, Intelligence modifier)` дополнительных языков.
- `OnePlusIntelligenceModifier` даёт `1 + max(0, Intelligence modifier)` дополнительных языков; это правило Human.
- `AllowedLanguageIds` содержит ancestry-specific fixed pool.
- `AllowsCommonLanguages` разрешает common-каталог как pool; для текущего baseline это правило Human.
- `AllowsAccessLanguages` разрешает добавить язык только при наличии отдельного server-side access, например от региона или другого character choice. Этот признак не означает автоматический доступ ко всем uncommon языкам.
- Starting languages и fixed pools каждой ancestry обязаны ссылаться на существующие catalog ids.

## Граница задачи

Этот этап добавляет каталог, исправляет семантику ancestry rules и публикует `GET /api/languages`. Вычисление количества дополнительных языков, пользовательский выбор, server-side access context и persistence относятся к следующему slice приоритета 6.
