# Equipment And Inventory Ownership Boundary

## Статус решения

Этот документ фиксирует ownership boundary для starting equipment в character creation. Решение действует для приоритета 7 и не включает Store в runtime приложения.

Целевое будущее состояние каталога, экземпляров, инвентаря и торговли зафиксировано отдельно в [../store/target_architecture_togaf.md](../store/target_architecture_togaf.md). До выполнения описанного там переходного этапа настоящий документ остаётся действующей границей реализации.

## Проблема

Для starting equipment нужны правила стоимости, Массы (`Bulk`), proficiency и экипировки. В solution уже есть незавершённый bounded context `Store`, однако его модель описывает товары и магазины и не является источником правил персонажа. Если character creation начнёт зависеть от Store, завершение персонажа станет зависеть от отключённой подсистемы, а PF2e item definitions смешаются с коммерческими offer и ownership records.

## Решение

### CharacterManagement владеет правилами создания персонажа

`CharacterManagement` является владельцем:

- нормализованного equipment catalog, необходимого для создания и вычисления карточки персонажа;
- stable item ids и правил предметов: category, price, Масса, weapon/armor group, traits и игровые характеристики;
- starting wealth и class kit definitions;
- выбора стартового снаряжения в draft;
- character-owned state: item reference, quantity и equipped state;
- серверной проверки стоимости, допустимости, proficiency applicability, общей Массы и нагрузки;
- invalidation несовместимого class kit при изменении class package до финализации.

Character state хранит ссылку на catalog definition по stable id и только изменяемое состояние конкретного персонажа. Полная catalog definition в aggregate и persistence не копируется. Денежный лимит создания вычисляется из правил starting wealth и не является runtime-кошельком.

### Store не участвует в character creation

Текущий `Store` не является владельцем PF2e equipment rules и не используется для:

- выдачи catalog definitions character creation;
- проверки starting wealth;
- выбора class kit;
- вычисления proficiency, Массы, нагрузки или derived combat statistics;
- хранения комплекта экипировки черновика (`draft loadout`).

`CharacterManagement` не получает ссылку на проекты `Store.*`, не вызывает Store API и не публикует обязательные для завершения персонажа запросы или события. Store остаётся отключённым в web composition root.

### Будущая runtime inventory boundary

Будущий Store/Inventory может владеть торговыми предложениями, ценами конкретного продавца, покупками, продажами, передачей предметов и account-level ownership. Передача комплекта экипировки завершённого персонажа (`completed-character loadout`) в такую подсистему требует отдельного архитектурного решения.

До этого решения:

- completed character продолжает владеть сохранённым character item state;
- catalog id является логической ссылкой, а не foreign key в Store database;
- Store не может изменять или удалять character equipment;
- интеграция не добавляется скрыто в рамках AC, attacks, damage, Массы или нагрузки.

## API boundary

Equipment endpoints для character creation должны находиться под API `CharacterManagement` и работать через его application use cases. Пустые legacy endpoints `GET /api/character/items` и `DELETE /api/character/items/drop` не являются контрактом и удаляются до появления нового типизированного API.

Runtime-команды `drop`, `trade`, `buy` и `sell` не входят в starting equipment flow.

## Инварианты для следующих slices

- сервер принимает item ids и choices, но сам разрешает definitions и вычисляет стоимость;
- неизвестный или недоступный item id отклоняется;
- character item ссылается на существующий stable catalog id;
- quantity и equipped state принадлежат персонажу, catalog definition остаётся общей;
- смена class package до финализации повторно валидирует или удаляет class-kit selection;
- завершение персонажа не зависит от доступности Store;
- проекты `CharacterManagement.*` не ссылаются на проекты `Store.*`.

## Не входит в решение

- runtime economy и кошелёк;
- магазины, ассортимент и цены продавцов;
- loot, drop, trade и передача предметов;
- расходуемые предметы и ammunition lifecycle;
- магические runes и item progression;
- перенос ownership completed inventory в отдельный bounded context.
