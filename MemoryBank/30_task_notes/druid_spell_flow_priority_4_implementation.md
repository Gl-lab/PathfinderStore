# Druid Spell Flow — Priority 4.3

## Проблема

Druid выбирает Order, но не имеет сохранённого primal prepared loadout, а Order focus spell остаётся descriptor без Focus Pool. Созданный персонаж не воспроизводит пять cantrips, два подготовленных слота и основную магию Order первого уровня.

## Нормативный baseline и ожидаемый результат

По [Druid, Player Core](https://2e.aonprd.com/Classes.aspx?ID=34) Druid первого уровня каждое утро готовит 5 разных primal cantrips и 2 rank-1 slots из common primal list. Один spell можно подготовить в нескольких slots. Выбранный Order даёт один order focus spell и стартовый Focus Pool `1`.

Нового Druid невозможно сохранить без полного валидного loadout. Сервер проверяет tradition/rank/rarity/counts, хранит только prepared ids, выводит Order spell и Focus Pool из catalog identity и возвращает всё после EF round-trip.

## План

1. Добавить четыре Player Core Order focus spells в общий каталог.
2. Реализовать Druid prepared resolver и Order focus resolver.
3. Расширить aggregate, builder, request, validator и read DTO.
4. Добавить две JSONB-колонки миграцией `dotnet ef`.
5. Подключить Primal options к общему spell step frontend, review и details.
6. Добавить domain/infrastructure/frontend tests, выполнить отдельный review и quality gate.

## Инварианты и границы

- 5 cantrips уникальны; 2 prepared rank-1 slots могут содержать одинаковый spell.
- Selectable entries — только common Primal rank-1 definitions.
- Order spell не передаётся клиентом и не занимает prepared slot.
- Focus Pool maximum `1` вычисляется из Order.
- casting, расход slots/Focus Points, Refocus, Learn a Spell, anathema enforcement и progression не входят в задачу.

## Выполнено

- общий Player Core catalog дополнен `Heal Animal`, `Cornucopia`, `Tempest Surge` и `Untamed Shift`;
- создание Druid требует 5 уникальных common Primal cantrips и 2 подготовленных rank-1 slots;
- одинаковый rank-1 spell разрешён в обоих slots;
- aggregate сохраняет выбранные ids, а read model восстанавливает полные определения spells;
- выбранный Druidic Order определяет focus spell и Focus Pool `1` без входного значения от клиента;
- добавлена EF-миграция `AddDruidSpellLoadout` с безопасным default `[]` для существующих записей;
- frontend поддерживает выбор, review и details на русском и английском;
- `SpellCatalog` больше не отмечен deferred для реализованной Druid spellcasting и Order focus spell.

## Review и проверка

Отдельное ревью среза проверило границы классов, атомарность class package, допустимость повторной подготовки spell, legacy-read без сохранённого loadout, EF round-trip и соответствие deferred-зависимостей фактической реализации. Блокирующих замечаний не осталось.

Quality gate:

- Domain tests: `191 passed`;
- Infrastructure tests: `218 passed`;
- Frontend tests: `71 passed`;
- frontend lint: успешно;
- frontend production build: успешно, остаётся существующее предупреждение Vite о chunk больше 500 kB;
- Web build: успешно;
- EF migrations check: pending model changes отсутствуют.
