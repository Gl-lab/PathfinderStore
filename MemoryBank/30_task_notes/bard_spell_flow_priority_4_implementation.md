# Bard Spell Flow — Priority 4.2

## Проблема

Bard уже обязан выбрать Muse, но Muse spell остаётся descriptor, а aggregate не хранит occult repertoire. Персонаж создаётся без пяти cantrips, двух выбранных rank-1 spells, двух spell slots и стартовых composition spells/focus resource, поэтому его основная классовая механика первого уровня не воспроизводится в карточке.

## Нормативный baseline

Для Bard первого уровня `Player Core` фиксируются:

- occult spontaneous spellcasting;
- 5 разных common occult cantrips по выбору;
- 2 разных common occult rank-1 spells по выбору;
- Muse добавляет ещё один rank-1 spell в repertoire, но не добавляет spell slot;
- 2 rank-1 spell slots в день;
- composition cantrip `Courageous Anthem`;
- composition focus spell `Counter Performance`;
- Focus Pool с максимумом `1`.

Источник: [Bard, Player Core](https://2e.aonprd.com/Classes.aspx?ID=32), spellcasting table и class features `Spell Repertoire`, `Composition Spells`, `Composition Cantrips`, `Muses`.

## Ожидаемый результат

Нового Bard невозможно сохранить без полного валидного стартового repertoire. Сервер проверяет tradition, rank, rarity, uniqueness и Muse-granted spell, хранит только пользовательские choices, выводит derived slots/composition package и после EF round-trip возвращает тот же loadout с provenance.

## План реализации

1. Добавить `Courageous Anthem` и `Counter Performance` в общий spell catalog как source-bound uncommon Bard spells.
2. Реализовать `BardSpellLoadoutResolver`: 5 unique common occult cantrips, 2 unique common occult rank-1 choices, запрет дублировать Muse-granted spell.
3. Расширить aggregate, builder, request validation и read DTO; derived Muse spell, 2 slots и composition package не хранить отдельно.
4. Добавить две JSONB persistence-колонки и создать migration только через `dotnet ef`.
5. Добавить frontend selection/review/details поверх server catalog без локального справочника spells.
6. Покрыть domain, application/infrastructure, EF round-trip и frontend tests.
7. Выполнить отдельный review задачи, исправить замечания, повторить quality gate и создать отдельный коммит.

## Инварианты

- Bard требует Muse и `BardSpellLoadout`; non-Bard запрещает оба списка.
- Selectable spells принадлежат common occult list первого ранга.
- Cantrips и выбранные rank-1 spells уникальны внутри своих групп.
- Muse-granted spell разрешается из server-owned Muse/catalog identity и не принимается отдельным payload.
- Выбранные rank-1 spells не могут повторять Muse-granted spell: итоговый repertoire содержит три разные записи первого ранга.
- Spell slots, composition spells и Focus Pool вычисляются сервером и не сохраняются.
- Casting, расход slots/Focus Points, Refocus, retraining и progression выше первого уровня не входят в задачу.

## План review

- атомарность `SetClassPackage` и очистка stale Bard state при смене класса;
- отсутствие доверия к client-provided metadata и derived counts;
- корректное разделение repertoire entries, slots и composition resources;
- совместимость чтения legacy Bard с пустыми новыми JSONB-колонками;
- минимальность migration и отсутствие ручных правок generated EF files;
- backend/frontend tests, builds, lint, `git diff --check` и EF pending-model check.

## Результат реализации и review

Vertical slice реализован 20 июля 2026 года.

- aggregate хранит 5 выбранных Bard cantrip ids и 2 выбранных rank-1 spell ids;
- Muse-granted spell, 2 rank-1 slots, `Courageous Anthem`, `Counter Performance` и Focus Pool `1` вычисляются из server catalogs;
- create validator, builder и domain resolver запрещают неполный loadout, wrong tradition/rank/rarity, duplicates и повтор Muse spell;
- read DTO возвращает три rank-1 repertoire entries с provenance `Selected`/`MuseGranted` и отдельный composition package;
- wizard использует общий `GET /api/spells`, очищает stale state при смене class/Muse и показывает repertoire в review/details;
- migration `AddBardSpellLoadout` добавляет только две non-null JSONB-колонки с default `[]`; legacy Bard остаётся читаемым с nullable package.

Во время отдельного review исправлены два рассогласования:

1. Реализованные Bard spell catalog/repertoire больше не объявляются deferred dependency в class/Muse descriptors; deferred остаётся только у Muse class feat.
2. Legacy Bard без Muse не пытается строить composition package через отсутствующий optional repository в старых read scenarios.

Проверки:

- 186 domain tests — пройдено;
- 217 infrastructure tests — пройдено;
- `Pathfinder.Web` build — пройдено, 0 errors;
- EF `has-pending-model-changes` — изменений после migration нет;
- 69 frontend tests, lint и production build — пройдены;
- Vite сохраняет существующее предупреждение о размере main chunk;
- `git diff --check` — пройдено.

Открытых замечаний по Bard spell flow после review не осталось.
