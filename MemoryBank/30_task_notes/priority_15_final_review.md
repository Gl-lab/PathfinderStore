# Приоритет 15 — итоговый cross-review

Дата завершения: 26 июля 2026 года.

## Результат

Приоритет 15 завершён. Пользовательский MVP-1 связывает ранее реализованные `CampaignManagement`, `Inventory`, `ItemCatalog` и `Commerce` в один campaign-scoped поток:

1. пользователь открывает отдельную страницу кампании с deep-link вкладками;
2. переходит к инвентарю назначенного персонажа;
3. видит серверные экземпляры, точные catalog revisions, версии, экипировку, provenance, Bulk и кошелёк;
4. отправляет подтверждаемый подарок и принимает входящий;
5. просматривает партийное хранилище, вносит и забирает предметы;
6. получает обновление ожидающих подарков и обменов не реже одного раза в 45 секунд.

## Выполненные slices и коммиты

| Slice | Результат | Коммит |
|---|---|---|
| 1 | Read-проекция инвентаря персонажа | `3d8f3e6` |
| 2 | Read-проекции подарков, обменов и партийного хранилища | `0026e32` |
| 3 | Read-проекции кошелька, поселений и магазинов | `2a9de94` |
| 4 | Страница кампании, lazy routes и query-вкладки | `5fb76f7` |
| 5 | Экран инвентаря и общие UI-компоненты | `b684d84` |
| 6 | Подтверждаемое дарение | `7563e55` |
| 7 | Партийное хранилище | `37299a9` |
| 8 | Polling, badge, ru/en и regression quality gate | `05c2913` |

Каждый slice был отдельно спланирован, реализован, проверен и закоммичен.

## Добавленные read API

- `GET /api/campaigns/{campaignId}/inventory/characters/{characterId}`;
- `GET /api/campaigns/{campaignId}/inventory/gifts`;
- `GET /api/campaigns/{campaignId}/inventory/exchanges`;
- `GET /api/campaigns/{campaignId}/inventory/party-storage`;
- `GET /api/commerce/campaigns/{campaignId}/wallets/{characterId}`;
- `GET /api/commerce/campaigns/{campaignId}/settlements`.

Проекции применяют server-side campaign/character access policy. Item DTO возвращают только разрешённое resolved-представление каталога и не раскрывают hidden properties.

## Итоговое ревью

Проверено:

- campaign isolation и owner/GM read-only доступ;
- отсутствие клиентского пересчёта доменного Bulk;
- передача `ExpectedItemVersion` во всех новых UI-командах;
- refetch и сохранение контекста формы при конфликте версии;
- повторное использование `GiftKey`/`OperationId` после сетевой ошибки;
- блокировка двойного submit;
- визуальная блокировка предмета в ожидающем исходящем подарке;
- отсутствие фиктивных действий reject/revoke и настройки storage policy;
- аудит deposit/withdraw в read-модели хранилища;
- единый формат денег без сырых `NNN cp` в пользовательских шаблонах;
- lazy routes, global snackbar host, loading/empty/error/retry states;
- строгий паритет новых namespace `inventoryUi`, `commerceUi`, `tradeUi` для ru/en.

Новых блокирующих замечаний по совокупному diff не найдено. Serena diagnostics для новых C# projection services чисты.

## Проверки

Frontend:

- `npm test`: 37 test files, 105 tests passed;
- `npm run lint`: passed, 0 warnings;
- `npm run build`: passed.

Backend, конфигурация `Release`:

- `CharacterManagement.Infrastructure.Tests`: 320 passed;
- `Inventory.Infrastructure.Tests`: 10 passed;
- `Inventory.Domain.Tests`: 59 passed;
- `Commerce.Infrastructure.Tests`: 10 passed;
- `CampaignManagement.Infrastructure.Tests`: 11 passed;
- `dotnet build Pathfinder.sln -c Release --no-restore`: passed, 0 errors.

В solution build остаётся существующее предупреждение frontend build-скрипта о `NODE_TLS_REJECT_UNAUTHORIZED=0`; оно не добавлено приоритетом 15.

## Границы и последующие этапы

- Магазин игрока, стол обмена и административный торговый UI остаются MVP-2/MVP-3.
- Команд reject/revoke подарка и настройки storage policy в backend нет; UI их не имитирует.
- Polling охватывает доступные read-проекции ожидающих подарков и обменов. Commerce purchase reservations будут добавлены в polling после появления пользовательской read-проекции в MVP-2.
- Встроенная браузерная сессия без авторизации подтвердила auth-redirect, но не позволила выполнить визуальный прогон защищённого экрана с реальными campaign data. Статические проверки, Vue type-check, production build и интеграционные тесты выполнены полностью.
