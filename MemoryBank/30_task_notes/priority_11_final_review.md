# Priority 11 Final Cross-Review

## Итог

Приоритет 11 «Экземпляры и runtime-инвентарь» завершён 22 июля 2026 года. Семь slices выполнены последовательно и закреплены отдельными коммитами:

1. Item instance, campaign scope и точная конфигурация — `c035a90`.
2. Личные, партийные, магазинные и мировые root containers — `d3a4c73`.
3. Стопки, разделение и объединение — `d85c77d`.
4. Текущее расположение и журнал перемещений — `8d72f1d`.
5. Version, operation id и идемпотентность — `a3ceddd`.
6. Миграция completed-character equipment — `eaadba1`.
7. Переключение combat adapter на Inventory — `27d9b29`.

## Проверка критериев

| Критерий | Результат |
|---|---|
| Экземпляр находится ровно в одном месте | Выполнено: `ItemInstance` имеет обязательный `CurrentContainerId`, а каждое изменение расположения проходит через доменную операцию. |
| Обычная команда не переносит экземпляр между кампаниями | Выполнено: instance и container содержат обязательный `CampaignId`; persistence дополнительно фиксирует same-campaign composite foreign key. |
| Полная catalog definition не копируется в экземпляр | Выполнено: экземпляр хранит только точный `ItemConfigurationId` и изменяемое runtime state. |
| Повтор операции не меняет состояние дважды | Выполнено: operation journal хранит `OperationId`; повтор split, merge или move возвращает сохранённый результат либо отклоняет конфликтующее использование идентификатора. |
| Starting equipment мигрировано без потери выбора и экипировки | Выполнено: quantity и equipped state переносятся в детерминированные экземпляры; повторный запуск безопасен. |
| Character creation не зависит от Commerce | Выполнено: draft selection остаётся в `CharacterManagement`; Inventory и Commerce не входят в domain/application dependencies character creation. |

## Cross-slice review

- `ItemInstance` имеет постоянный identity, campaign scope, точную неизменяемую конфигурацию, custom name, quantity, stackability, current container и optimistic version.
- Root container явно моделирует владельца `Character`, `Party`, `Shop` или `World`; ownership не дублируется на каждом экземпляре.
- Split проверяет уникальность нового identity и количество; merge требует совместимых экземпляров и защищён от переполнения.
- Move изменяет materialized current location и одновременно добавляет неизменяемую запись в movement journal.
- Operation journal различает тип операции и payload, поэтому один `OperationId` нельзя повторно применить с другим смыслом.
- EF schema `inventory` хранит контейнеры, экземпляры и журналы; concurrency token предотвращает потерю параллельных изменений.
- Миграция обрабатывает только completed characters с ровно одной кампанией. Перед записью Inventory она разрешает все catalog configurations и создаёт недостающий global starting catalog детерминированно.
- Детерминированные container/instance ids и проверка уже существующего состояния делают startup migration повторяемой и не допускают тихого расхождения данных.
- `RuntimeInventoryAllowedEquipmentReader` проверяет character container, campaign scope и видимость definition, затем читает exact revision через конфигурацию. Последующий перевод revision в `Retired` не меняет свойства уже существующего экземпляра.
- `CharacterManagement.Domain` и `CharacterManagement.Application` сохраняют application-owned `IAllowedEquipmentReader` и не зависят от `Inventory.*` или `ItemCatalog.*`.

## Миграции

Созданы две EF migrations:

1. Inventory `AddInventoryRuntime` — schema, containers, instances, movements и operations.
2. CharacterManagement `AddRuntimeInventoryReferences` — признак завершённой миграции и ссылки на runtime equipment instances.

`dotnet ef migrations has-pending-model-changes` для Inventory и CharacterManagement подтверждает отсутствие расхождения модели и snapshots.

## Quality gate

- `dotnet test Pathfinder.sln --no-restore`: passed, 670 backend tests:
  - `CharacterManagement.Domain.Tests` — 245;
  - `CharacterManagement.Infrastructure.Tests` — 305;
  - `CampaignManagement.Domain.Tests` — 17;
  - `CampaignManagement.Infrastructure.Tests` — 11;
  - `ItemCatalog.Domain.Tests` — 31;
  - `ItemCatalog.Infrastructure.Tests` — 8;
  - `Inventory.Domain.Tests` — 52;
  - `Inventory.Infrastructure.Tests` — 1.
- `Pathfinder.Web` собирается в составе solution-level прогона.
- Scoped `git diff --check` и проверка документации выполняются перед финальным коммитом.

## Переходные ограничения

- Миграция выполняется startup hosted service. Completed character без кампании или с назначениями в несколько кампаний намеренно пропускается: обязательный `CampaignId` нельзя угадать безопасно.
- Для ещё не мигрированного completed state combat reader использует starting fallback. После однозначного назначения и следующего запуска migration данные переходят в Inventory.
- Priority 11 создаёт доменную и persistence основу, но не публикует пользовательские Inventory commands/API. Подарок, обмен и общее хранилище относятся к Priority 12.
- Root containers реализованы; вложенные контейнеры, loot/drop, расходование, charges, durability mutations и attachments относятся к следующим приоритетам.
- Автоматическое добавление произвольных post-creation catalog items и универсальный resolved-item contract пока отсутствуют; текущий adapter покрывает мигрированный starting equipment.
- Vikunja в текущем окружении недоступна. Статус зафиксирован локально; внешние карточки потребуется синхронизировать после восстановления подключения.

## Заключение

Priority 11 завершён: физическая идентичность, campaign ownership, location, stacks, audit, idempotency, migration и combat read boundary образуют согласованный базовый runtime Inventory. Следующий ограниченный этап — Priority 12 с атомарными transfer workflows и party storage policy.
