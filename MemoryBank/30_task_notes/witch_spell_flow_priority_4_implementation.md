# Witch Spell Flow — Priority 4.4

## Проблема

Witch уже выбирает Patron и определённый им familiar spell, но aggregate не хранит стартовые spells familiar, подготовленный loadout или выбранный focus hex. Patron hex cantrip и Focus Pool остаются descriptors, поэтому основная spellcasting-механика первого уровня не воспроизводится после сохранения.

## Нормативный baseline и ожидаемый результат

По [Witch, Player Core](https://2e.aonprd.com/Classes.aspx?ID=38) Patron определяет tradition, дополнительный familiar spell и hex cantrip. Familiar первого уровня знает 10 cantrips, 5 rank-1 spells и 1 дополнительный Patron spell. Witch подготавливает 5 cantrips и 2 rank-1 slots из известных familiar spells. Также Witch выбирает `Patron's Puppet` или `Phase Familiar` и начинает с Focus Pool `1`.

Нового Witch невозможно сохранить без полного валидного familiar storage и prepared loadout. Сервер проверяет tradition, rank, rarity, counts, subsets и Patron grants, хранит только пользовательские choices, а derived Patron spell, hex cantrip и Focus Pool восстанавливает из catalog identity.

## План

1. Добавить семь Patron hex cantrips и два стартовых focus hex в общий catalog.
2. Реализовать resolver familiar storage, prepared loadout и hex package.
3. Расширить aggregate, builder, request validation и read DTO.
4. Добавить JSONB/scalar persistence через migration `dotnet ef`.
5. Подключить tradition-aware options к frontend selection, review и details.
6. Добавить domain/infrastructure/frontend tests, выполнить отдельный review и quality gate.

## Инварианты и границы

- familiar знает 10 уникальных common cantrips и 5 уникальных common rank-1 spells tradition выбранного Patron;
- Patron spell добавляется сверх пяти familiar spells и не может дублироваться в пользовательской пятёрке;
- Witch подготавливает 5 уникальных известных cantrips и 2 slots из известных rank-1 spells; один spell можно подготовить в обоих slots;
- Patron hex cantrip не занимает prepared cantrip slot;
- выбранный focus hex обязан быть `Patron's Puppet` или `Phase Familiar`; Focus Pool maximum равен `1`;
- familiar creature customization, daily familiar abilities, casting, расход slots/Focus Points, Refocus, Learn a Spell и progression не входят в задачу.

## Выполнено

- общий catalog дополнен семью Patron hex cantrips, `Patron's Puppet` и `Phase Familiar`;
- создание Witch требует familiar storage `10/5`, prepared loadout `5/2` и стартовый focus hex;
- tradition выводится из Patron, а selectable options приходят из общего `GET /api/spells`;
- Patron spell хранится как существующий Patron choice, добавляется сверх пользовательской пятёрки и доступен для подготовки;
- Patron hex cantrip и Focus Pool `1` вычисляются сервером и возвращаются отдельным typed package;
- aggregate сохраняет четыре списка ids и выбранный focus hex; read model восстанавливает определения spells после EF round-trip;
- migration `AddWitchSpellLoadout` добавляет четыре JSONB-колонки с default `[]` и nullable scalar для legacy rows;
- frontend поддерживает familiar storage, preparation, Patron/focus grants, review и details на русском и английском;
- при смене Patron той же tradition сохраняются совместимые familiar choices, а несовместимый старый Patron spell удаляется из prepared slots;
- `SpellCatalog` больше не отмечен deferred для Witch spellcasting, Patron familiar spell и Patron hex cantrip; familiar creature mechanics и lesson effects остаются deferred.

## Review и проверка

Отдельное ревью проверило subsets familiar knowledge, допустимые duplicates только в prepared slots, Patron spell сверх базовой пятёрки, ограничение стартового focus hex двумя вариантами, смену Patron, legacy-read, EF defaults и deferred metadata. Исправлены сохранение совместимых choices при смене Patron одной tradition и повторная серверная проверка identity focus hex в read resolver. Блокирующих замечаний не осталось.

Quality gate:

- Domain tests: `194 passed`;
- Infrastructure tests: `220 passed`;
- Frontend tests: `73 passed`;
- frontend lint и production build: успешно; остаётся существующее предупреждение Vite о chunk больше 500 kB;
- Web build: успешно;
- EF pending model changes: отсутствуют;
- `git diff --check`: успешно.
