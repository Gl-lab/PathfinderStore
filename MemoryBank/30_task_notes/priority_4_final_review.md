# Priority 4 — Final Cross-Review

## Цель и scope

Проверить как единый change set пять последовательных slice Priority 4: общий Player Core spell catalog и spell flows Bard, Druid, Witch и Wizard. Review выполнен после отдельных plan/implementation/review/commit каждой задачи и ограничен стартовым loadout первого уровня.

## Проверенный change set

- `529e327` — tradition-aware Player Core spell catalog;
- `37adbec` — Bard spell flow;
- `b132f56` — Druid spell flow;
- `7a10880` — Witch spell flow;
- `361b9ce` — Wizard spell flow.

## План review

1. Сверить tradition, rank, rarity, uniqueness/duplicates и количество choices/slots для всех четырёх классов.
2. Проверить derived grants: Muse spell, composition spells, Order spell, Patron spell/hex cantrip, initial school spell и focus resources.
3. Проверить prepared/repertoire/spellbook provenance и отдельные class-specific slots.
4. Проверить очистку choices при смене Class, Muse, Order, Patron и School.
5. Проверить create/persistence/read round-trip, nullable legacy rows и последовательность четырёх migrations.
6. Проверить, что реализованные spell rules больше не обозначены deferred, а runtime casting, progression, feats, familiar/item mechanics и advanced school spells остались за явной границей.
7. Повторить domain/infrastructure tests, builds, EF model check, frontend test/lint/build и `git diff --check`.

## Найденные замечания

### 1. Смена Arcane School очищала совместимый базовый spellbook

Первоначальная Wizard UI-реализация сбрасывала весь spellbook и prepared loadout при любой смене School. Common Arcane base choices не зависят от формальной школы и не должны теряться.

Исправление: добавлен чистый reconciliation helper. Он сохраняет 10 базовых cantrips, совместимые base rank-1 spells и prepared choices; при переходе Unified → formal удаляет только лишний шестой base spell и зависимый slot, а curriculum choices фильтрует по новой школе. Поведение закреплено frontend test.

### 2. Смена традиции Witch очищала совместимый focus hex

`Patron's Puppet` и `Phase Familiar` доступны при каждой Patron tradition, но UI очищал выбранный focus hex вместе со spell storage при смене традиции.

Исправление: смена tradition по-прежнему очищает несовместимые familiar/prepared spells, но сохраняет универсальный focus hex. При снятии Patron выбор очищается полностью.

### 3. Обзор проекта описывал только Cleric spell flow

`project_overview.md` не отражал завершённые Bard, Druid, Witch и Wizard flows и общий `/api/spells` catalog.

Исправление: текущий character creation focus и список API синхронизированы с завершённым Priority 4.

Других открытых замечаний по server-owned validation, slot provenance, duplicate semantics, migrations, legacy read и границам runtime mechanics не найдено.

## Результат

Priority 4 завершён. Все пять spellcasting-классов текущего Player Core baseline имеют обязательный валидируемый стартовый loadout: Cleric из Priority 2 и Bard, Druid, Witch, Wizard из Priority 4. Общий каталог предоставляет common options по tradition/rank/kind, а class resolvers владеют granted spells, preparation/repertoire/spellbook rules, class-specific slots и focus resources.

Runtime casting, расходование slots/focus points, rest lifecycle, heightening/progression и исполнение spell effects намеренно остаются отдельной подсистемой.

## Финальные проверки

- Domain tests: `194/194` passed;
- Infrastructure/API tests: `223/223` passed;
- `Pathfinder.Web` build: passed без ошибок;
- `CharacterManagement.Infrastructure` build: passed без ошибок;
- EF: `No changes have been made to the model since the last migration`;
- frontend: `76/76` tests passed, ESLint passed, production build passed;
- `git diff --check`: passed.

Production build сохраняет существующее предупреждение Vite о размере основного chunk больше 500 kB. Оно не блокирует сборку и не создано исправлениями финального review. Открытых замечаний по Priority 4 после исправлений не осталось.
