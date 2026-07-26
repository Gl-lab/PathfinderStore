# Priority 14 Final Cross-Review

## Итог

Приоритет 14 «Инструменты ведущего и скрытые свойства» завершён 26 июля 2026 года. Шесть slices выполнены последовательно и закреплены отдельными коммитами:

1. Campaign-scoped черновики и публикация описаний — `7897637` с опорой на ранее реализованный lifecycle ItemCatalog.
2. Уникальная конфигурация и физический экземпляр — `3e3cf15`.
3. Полное серверное и видимое наблюдателю представления — `ea7da13`.
4. Знание персонажа и партии, идентификация и раскрытие — `17652ab`.
5. Скрытый binding effect и запрет передачи — `bb27822`.
6. Принудительная выдача, перемещение и исправление с причиной и аудитом — `ec28c61`.

Итоговое ревью проверило campaign isolation, отсутствие скрытого свойства в сериализованном player DTO, применение ограничения до раскрытия, раздельное знание персонажей и различимый аудит административных действий.

## Проверка критериев

| Критерий | Результат |
|---|---|
| Авторский предмет одной кампании недоступен другой | Выполнено: definition, новая configuration, instance, knowledge и observation проверяют один `CampaignId`; одинаковые catalog keys изолированы между кампаниями. |
| Игрок не получает скрытые свойства через API или breakdown | Выполнено: HTTP endpoint возвращает только `VisibleItemDto`; тест сериализации подтверждает отсутствие hidden upgrade code. |
| Сервер применяет скрытое ограничение до раскрытия | Выполнено: hidden typed effect `curse.binding` устанавливает `ItemInstance.IsTransferRestricted` при создании; обычный `MoveTo` отклоняется независимо от visible projection. |
| Разные персонажи могут знать о предмете разное | Выполнено: `ItemPropertyKnowledge` хранит reveal по экземпляру и subject `Character` или `Party`; character-specific тест подтверждает различный результат. |
| Ведущий видит полное описание только своей кампании | Выполнено: активное campaign membership разрешается сервером; GM получает resolved upgrades только в точной активной кампании. |
| Принудительное действие ведущего отличимо и записано в аудит | Выполнено: `ForcedIssuance`, `ForcedMove` и `ForcedCorrection` используют отдельные audit kinds, `IsForced`, обязательную причину и идемпотентный `OperationId`. |

## Cross-slice review

- Опубликованная revision остаётся неизменяемой основой; уникальность задаётся campaign-scoped configuration и runtime instance.
- Старые configuration rows допускают `CampaignId = null` как переходное состояние. Все новые configuration создаются только с положительным campaign id, который входит в детерминированный configuration key.
- Unique-item endpoint принимает опубликованную global revision или campaign revision точной кампании, проверяет активного ведущего, destination container и идемпотентно создаёт configuration, instance и forced issuance audit.
- Trusted resolver собирает instance, exact configuration, revision и все permanent upgrades. Visible projection фильтрует hidden upgrades по роли и сохранённому знанию.
- Reveal является явной GM-командой; target character или party проверяется внутри той же активной кампании. Запись сохраняет автора и время раскрытия.
- `curse.binding` распознаётся как типизированный скрытый effect. Ограничение хранится в authoritative instance state, поэтому отсутствие свойства в клиентском DTO не отключает серверное правило.
- Принудительная коррекция transfer restriction не маскируется под player action и допускает безопасный replay только при полном совпадении параметров.

## API

- `POST api/item-catalog-admin/campaigns/{campaignId}/unique-items` — создать уникальную configuration и instance с причиной выдачи.
- `GET api/campaigns/{campaignId}/items/{instanceKey}?observerCharacterId=...` — получить видимое представление.
- `POST api/item-catalog-admin/campaigns/{campaignId}/items/{instanceKey}/knowledge/reveal` — раскрыть hidden property персонажу или партии.
- `POST api/campaigns/{campaignId}/inventory/force-move` — принудительно переместить предмет.
- `POST api/campaigns/{campaignId}/inventory/correct-transfer-restriction` — исправить restriction с обязательной причиной.

## Миграции

Через `dotnet ef` созданы migrations схемы `item_catalog`:

1. `ScopeItemConfigurationsByCampaign`.
2. `AddItemPropertyKnowledge`.

`dotnet ef migrations has-pending-model-changes` подтверждает отсутствие расхождения модели и snapshot.

## Quality gate

- Профильные backend tests — 420 passed:
  - `ItemCatalog.Domain.Tests` — 31;
  - `ItemCatalog.Infrastructure.Tests` — 10;
  - `Inventory.Domain.Tests` — 59;
  - `Inventory.Infrastructure.Tests` — 10;
  - `CharacterManagement.Infrastructure.Tests` — 310.
- `dotnet build Pathfinder.Web/Pathfinder.Web.csproj --no-restore`: succeeded, 0 errors.
- Scoped `git diff --check` и итоговое code review выполнены перед финальным коммитом.

## Переходные ограничения

- Configuration и instance сохраняются разными EF contexts. Детерминированный configuration key, `InstanceKey` и audit `OperationId` обеспечивают безопасный retry, но общей распределённой транзакции пока нет.
- В первом typed-effect registry реализован только `curse.binding`. Новые исполняемые effects требуют явного server-side policy; произвольный код или формулы из API не поддерживаются.
- Идентификация моделируется явным reveal ведущего. Автоматическая skill-check механика и игровые броски остаются будущим Gameplay slice.
- Full resolver пока расположен в web integration composition layer. При появлении отдельного Gameplay bounded context контракт следует вынести в его application-owned port.
- Vikunja в текущем окружении недоступна. Статус зафиксирован локально; внешние карточки потребуется синхронизировать после восстановления подключения.

## Заключение

Priority 14 завершён: ведущий может создавать campaign-scoped уникальные предметы, сервер отделяет authoritative свойства от видимого представления, знания различаются по персонажу и партии, скрытое ограничение действует до раскрытия, а принудительные операции имеют обязательную причину и отдельный аудит. Следующий ограниченный этап roadmap — Priority 15 с воспроизводимой генерацией ассортимента.
