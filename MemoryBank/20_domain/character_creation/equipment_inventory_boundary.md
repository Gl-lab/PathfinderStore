# Equipment And Inventory Ownership Boundary

## Статус решения

Этот документ фиксирует ownership boundary для starting equipment в character creation и переход завершённого персонажа в runtime Inventory. Решение актуализировано после завершения Priority 11 и не включает Commerce в runtime приложения.

Целевое состояние каталога, экземпляров, инвентаря и торговли зафиксировано отдельно в [../store/target_architecture_togaf.md](../store/target_architecture_togaf.md). Каталог и базовый runtime Inventory реализованы; настоящий документ остаётся границей между draft selection, миграцией completed loadout и боевым чтением.

## Проблема

Для starting equipment нужны правила стоимости, Массы (`Bulk`), proficiency и экипировки. В solution уже есть незавершённый bounded context `Store`, однако его модель описывает товары и магазины и не является источником правил персонажа. Если character creation начнёт зависеть от Store, завершение персонажа станет зависеть от отключённой подсистемы, а PF2e item definitions смешаются с коммерческими offer и ownership records.

## Решение

### ItemCatalog владеет описаниями, CharacterManagement — выбором персонажа

`ItemCatalog` является владельцем stable item definitions, неизменяемых revisions, категорий, типизированных компонентов и конфигураций. `CharacterManagement` не ссылается на проекты ItemCatalog из domain/application и остаётся владельцем:

- starting wealth и class kit definitions;
- выбора стартового снаряжения в draft;
- character-owned state: item reference, quantity и equipped state;
- серверной проверки стоимости, допустимости, proficiency applicability, общей Массы и нагрузки;
- invalidation несовместимого class kit при изменении class package до финализации.

Character state хранит ссылку на catalog definition по stable key и только изменяемое состояние конкретного персонажа. Полная catalog definition в aggregate и persistence не копируется. Денежный лимит создания вычисляется из правил starting wealth и не является runtime-кошельком. Legacy `EquipmentRepository` временно сохраняет class kits и недостающие Priority 8 metadata/fallback, пока starting selection не мигрирован на новый каталог.

### Store не участвует в character creation

Текущий `Store` не является владельцем PF2e equipment rules и не используется для:

- выдачи catalog definitions character creation;
- проверки starting wealth;
- выбора class kit;
- вычисления proficiency, Массы, нагрузки или derived combat statistics;
- хранения комплекта экипировки черновика (`draft loadout`).

`CharacterManagement` не получает ссылку на проекты `Store.*`, не вызывает Store API и не публикует обязательные для завершения персонажа запросы или события. Store остаётся отключённым в web composition root.

### Runtime inventory boundary

`Inventory` владеет campaign-scoped физическими экземплярами, корневыми контейнерами, количеством, расположением, версией и неизменяемыми журналами операций и перемещений. `Commerce` в будущем будет владеть предложениями, ценами продавца, покупками и продажами; передача между владельцами относится к отдельным runtime-командам.

После завершения Priority 11:

- draft character продолжает владеть starting-equipment selection и не зависит от Inventory или Commerce;
- completed character, однозначно назначенный кампании, получает личный root container и экземпляры с точными `ItemConfigurationId`;
- миграция сохраняет quantity и equipped state и безопасно повторяется;
- completed character без кампании или с неоднозначным назначением остаётся на starting fallback до появления однозначного campaign scope;
- `CharacterManagement.Domain` и `CharacterManagement.Application` не получают project reference на Inventory или ItemCatalog.

## API boundary

Equipment endpoints для character creation должны находиться под API `CharacterManagement` и работать через его application use cases. Пустые legacy endpoints `GET /api/character/items` и `DELETE /api/character/items/drop` не являются контрактом и удаляются до появления нового типизированного API.

Runtime-команды `drop`, `trade`, `buy` и `sell` не входят в starting equipment flow.

## Allowed equipment read boundary

Боевая карточка не зависит напрямую от `IEquipmentRepository`. Application определяет порт `IAllowedEquipmentReader` и собственные безопасные типы `AllowedEquipmentLoadout`/`AllowedEquipmentItem`, содержащие только необходимые read-модели и расчётам поля.

Production adapter `RuntimeInventoryAllowedEquipmentReader` размещён в Web integration layer и:

- проверяет принадлежность character container и item instances точной кампании;
- разрешает каждый экземпляр через его неизменяемую конфигурацию и точную revision, включая впоследствии retired revision;
- сохраняет starting-catalog fallback только для ещё не мигрированного completed state;
- вычисляет proficiency applicability, стоимость, Массу и нагрузку;
- проецирует weapon/armor/shield statistics в application-owned контракт;
- не сериализует внутренние `ItemDefinition`, `ItemRevision` или `EquipmentDefinition` клиенту.

`StartingEquipmentAllowedEquipmentReader` остаётся внутренним fallback-механизмом. Runtime Inventory заменил источник production-адаптера, но не application-owned контракты и формулы боевой карточки.

## Инварианты для следующих slices

- сервер принимает item ids и choices, но сам разрешает definitions и вычисляет стоимость;
- неизвестный или недоступный item id отклоняется;
- character item ссылается на существующий stable catalog id;
- quantity и equipped state принадлежат персонажу, catalog definition остаётся общей;
- смена class package до финализации повторно валидирует или удаляет class-kit selection;
- завершение персонажа не зависит от доступности Store;
- проекты `CharacterManagement.Domain` и `CharacterManagement.Application` не ссылаются на проекты `ItemCatalog.*`, `Inventory.*` или `Store.*`.

## Не входит в решение

- runtime economy и кошелёк;
- магазины, ассортимент и цены продавцов;
- loot, drop, trade и передача предметов;
- расходуемые предметы и ammunition lifecycle;
- магические runes и item progression;
- пользовательские команды изменения runtime inventory и transfer ownership.
