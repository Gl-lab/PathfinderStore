# ТЗ UI торговли и инвентаря — обзор и сквозные требования

Пакет ТЗ к дизайн-проекту [ui_design_inventory_commerce_trading.md](../../90_research/ui_design_inventory_commerce_trading.md), мокапы — [ui_design_inventory_commerce_trading_mockups.html](../../90_research/ui_design_inventory_commerce_trading_mockups.html).

Состав пакета:

| № | ТЗ | Этап |
|---|----|------|
| 01 | [Страница кампании с вкладками](01_campaign_page_tabs.md) | MVP-1 (пререквизит) |
| 02 | [Инвентарь персонажа](02_character_inventory.md) | MVP-1 |
| 03 | [Дарение предмета](03_gift_dialog.md) | MVP-1 |
| 04 | [Партийное хранилище](04_party_storage_tab.md) | MVP-1 |
| 05 | [Магазин игрока](05_player_shop.md) | MVP-2 |
| 06 | [Стол обмена](06_exchange_table.md) | MVP-3 |
| 07 | [Панель торговли ведущего](07_commerce_admin.md) | MVP-3 |

**Блокер всех экранов:** на бэкенде нет read-проекций (только POST-команды). Каждое ТЗ перечисляет требуемые GET в разделе «Зависимости API». Реализация read-проекций — отдельные backend-задачи, UI-задачи стартуют после них (или параллельно на моках из типов `api.ts`).

---

## Сквозные требования (входят в Definition of Done каждого экрана)

### Код и структура

1. Новые фичи: `src/features/inventory/` и `src/features/commerce/` — typed `api.ts` (axios-обёртки + DTO-интерфейсы), чистая логика отдельными модулями с co-located `*.spec.ts`.
2. Переиспользуемые компоненты — в новой папке `src/components/` (первая общая папка проекта).
3. Роуты — lazy (`component: () => import(...)`), `meta.title` + `meta.requiresAuth` по существующей конвенции.
4. Состояние — локальное для view; глобальные store только `usePendingOperationsStore` (см. ниже). 

### Общие компоненты (реализуются в задаче ТЗ-02, используются всеми)

| Компонент | Контракт |
|---|---|
| `MoneyText` | props: `copper: number`, `compact?: boolean`. Формат: `12 зм 4 см 3 мм` (en: `12 gp 4 sp 3 cp`), нулевые разряды опускаются, `0` → `0 мм`. tabular-nums. |
| `MoneyInput` | v-model: copper (number). Три поля зм/см/мм, валидация неотрицательности. |
| `ItemListRow` | props: иконка категории, name, quantity, bulkTenths, subtitle; slots: `append` (действия). |
| `BulkMeter` | props: `totalTenths, encumberedTenths, maximumTenths`. Насечка на пороге, цвет error при перегрузе. |
| `OperationStatusChip` | props: `status` из union `Pending | Reserved | Completed | Cancelled | Expired`; цвета: bronze/teal/ok/default/err. |
| `CountdownChip` | props: `expiresAtUtc: string`. Тик 1 с, по истечении — emit `expired` и статус «истёк». Отсчёт чисто визуальный, авторитетен сервер. |
| `AppSnackbarHost` | глобальный хост в `App.vue` + composable `useSnackbar()` (`success/error/info`, очередь). |

Категория → иконка (MDI): Weapon `mdi-sword`, Armor `mdi-shield-half-full`, Shield `mdi-shield`, Gear `mdi-sack`, Ammunition `mdi-arrow-projectile-multiple`, Consumable `mdi-flask-round-bottom`.

### Деньги

- Хранение и API — только медь (`number`, копия backend `long *Copper`).
- Отображение — всегда через `MoneyText`/`MoneyInput`. 1 зм = 10 см = 100 мм. Платина не отображается.
- Запрещено выводить сырые `NNN cp` (существующее место в шаге Starting Equipment чинится в ТЗ-02 п. «Попутно»).

### Идемпотентность и optimistic concurrency

- `OperationId`/`GiftKey`/`ExchangeKey`/ключи резервов — `crypto.randomUUID()`, генерируются **при открытии формы** и переиспользуются при retry той же формы. Новая попытка после явного отказа сервера по бизнес-правилу — новый ключ.
- Каждая команда с `ExpectedItemVersion` при 400-ответе о конфликте версии: показать tonal-alert «Предмет изменился, список обновлён», сделать refetch проекции, форму не закрывать (кроме случаев, оговорённых в ТЗ экрана).
- Маппинг ошибок — существующий `getApiErrorMessages`; новые бизнес-коды добавляются в словарь локализации, не хардкодятся в компонентах.

### Обновление данных (без WebSocket)

- Composable `usePendingOperations(campaignId)` + Pinia store: опрос входящих подарков/обменов/активных резервов каждые 45 с, только пока открыт роут внутри кампании; ручной refresh кнопкой. Экспортирует счётчики для бейджей (вкладка кампании, полоса «Входящие»).
- После любой успешной команды — точечный refetch затронутых проекций (не полный опрос).

### Доступ

- Все экраны — `meta.requiresAuth`.
- Campaign-scoped доступ проверяет сервер; UI дополнительно скрывает недоступное: панель ведущего видна только роли GameMaster кампании, действия с чужими персонажами не отображаются.

### i18n

- Новые namespace: `inventoryUi.*`, `commerceUi.*`, `tradeUi.*` в `i18n/messages.ts`, **обязательно ru + en**, ru — первичный.
- Статусы операций и виды offer'ов локализуются через lookup в `i18n/domain.ts` (по образцу существующих доменных кодов).

### Тесты

- Чистая логика (форматтер денег, расчёт bulk, countdown, редьюсер конфликтов версий, сборка линий обмена) — Vitest, co-located.
- Компонентные тесты — не требуются в этом пакете (нет прецедента в проекте).

### Критерии приёмки, общие для каждого экрана

- [ ] Скелетон/`v-progress-linear` при загрузке, empty-state с подсказкой, inline `v-alert` с retry при ошибке загрузки (существующий паттерн `isLoading`/`errorMessages`).
- [ ] Все суммы — через `MoneyText`, все статусы — через `OperationStatusChip`.
- [ ] Конфликт версии обрабатывается по сквозному правилу (alert + refetch, без потери контекста формы).
- [ ] ru/en переключаются без пропусков ключей (проверка: переключить локаль на экране).
- [ ] Соответствие теме `characterLedger`: flat-карточки `elevation=0` с рамкой `surface-variant`, eyebrow + serif-заголовок, никаких `v-data-table`.
