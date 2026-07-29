# ТЗ. Управление каталогом предметов и наполнение ассортимента магазинов

Эскизы UI: [ui_design_item_catalog_admin_mockups.html](../90_research/ui_design_item_catalog_admin_mockups.html) (секции 1–7). Сквозные требования UI — [ТЗ-00](ui_trading_specs/00_overview_and_shared.md). Термины — [глоссарий](../20_domain/glossary.md): «вид предмета» (`ItemDefinition`), «ревизия описания предмета» (`ItemRevision`), «конфигурация предмета» (`ItemConfiguration`); слово «вариант» синонимом ревизии не использовать.

## Проблема

Ведущий может открыть форму «Добавить из каталога», но не может подготовить для неё данные: экрана каталога нет, административной команды создания обычной кампанийной конфигурации нет, а форма показывает только опубликованные ревизии с уже существующей конфигурацией текущей кампании. Товар нельзя выставить без Swagger, миграции стартового инвентаря или прямой работы с базой. Системный администратор не имеет UI для глобального каталога вовсе. Дополнительно проверки видимости конфигураций несогласованы: покупка и рестокинг допускают конфигурации, которые форма каталога не показывает, и наоборот.

## Цель

Системный администратор ведёт глобальный каталог, ведущий — каталог своей кампании, через штатный интерфейс. Для опубликованной видимой ревизии ведущий создаёт стандартную конфигурацию кампании и сразу выставляет её как предложение магазина. Пул кандидатов генерации ассортимента (Priority 16) и список формы «Добавить из каталога» видят один и тот же набор конфигураций.

## Доступ

| Область | Кто | Как проверяется |
|---|---|---|
| Глобальный scope: создание вида, ревизий, publish/retire | Системный администратор | `IItemCatalogAdministrativeAccess.CanManageGlobalCatalogAsync` (permission `Administration`) |
| Campaign scope: вид/ревизии своей кампании, конфигурации своей кампании | Активный GameMaster активной кампании | `CanManageCampaignCatalogAsync` (активное GM-членство) |
| Чтение глобального каталога ведущим | GameMaster | только Published/Retired; глобальные Draft скрыты |
| Cross-campaign | никто | запрещено; чужая кампания неотличима от «не найдено» |

Сервер первичен: фронтенд скрывает недоступные действия, но каждая команда защищена на backend (`Forbid` → редирект + снэкбар). Ролевых claims в JWT нет — фронтенд узнаёт права через новый capabilities-endpoint.

## Единое правило видимости конфигураций

Конфигурация доступна в кампании C ⇔ `CampaignId == null` (legacy-строки до миграции `ScopeItemConfigurationsByCampaign`) **или** `CampaignId == C`; дополнительно вид — глобальный или кампании C, и для Commerce-операций ревизия Published.

Привести к правилу существующие места:

| Файл | Метод | Изменение |
|---|---|---|
| `ItemCatalog.Infrastructure/Commerce/CommerceCatalogReader.cs` | `IsPublishedConfigurationAsync`, `GetBasePriceCopperAsync` | добавить предикат по `configuration.CampaignId` (сейчас пропускают чужие кампании) |
| `Pathfinder.Web/Integration/CommerceAdministrationProjectionService.cs` | `SearchPublishedRevisionsAsync` | допускать `CampaignId == null` (сейчас legacy-конфигурации скрыты из формы) |
| `Pathfinder.Web/Integration/CompletedCharacterInventoryMigrationService.cs` | `ResolveConfigurationAsync` | shape-поиск ограничить тем же предикатом (сейчас переиспользует конфигурации чужих кампаний); при двух совпадениях предпочитать кампанийную |

`GetRestockCandidatesAsync` и `InventoryItemCatalogProjectionReader` уже соответствуют правилу — не менять. Риск: существующие offer'ы, ссылающиеся на конфигурации чужих кампаний, перестанут покупаться (чистая 400). Перед деплоем выполнить [SQL-аудит](item_catalog_cross_campaign_audit.sql) (offer'ы и экземпляры на конфигурациях чужих кампаний); найденные строки лечатся созданием одноимённой конфигурации своей кампании.

## Backend

Изменения схемы БД не требуются: нужные индексы (`ConfigurationKey` unique, фильтрованные уникальные индексы видов и ревизий) уже существуют. Никаких миграций.

### B1. Команда создания конфигурации кампании

`POST api/item-catalog-admin/campaigns/{campaignId:int}/configurations` — новый контроллер `ItemConfigurationsAdminController`, новый сервис `ItemCatalog.Application/Configurations/ItemConfigurationAdministrationService` (первый потребитель зарегистрированного `IItemConfigurationRepository.GetByConfigurationKeyAsync`).

Тело: `{ itemDefinitionId, revisionNumber, size, materialType, materialGrade, permanentUpgrades?[] }`; улучшения — `{ code, kind, rank, visibility }` (переиспользовать `PermanentUpgradeApiRequest`). Постоянные улучшения поддерживаются полностью (домен: ≤ 16, уникальные коды).

Поток: проверка GM → вид по id (чужой campaign-вид = «не найдено») → ревизия по номеру → бизнес-ошибки для Draft («…the revision is still a draft.») и Retired («…the revision has been retired.») → `ItemConfiguration.Create` (домен считает `ConfigurationKey`, campaignId входит в хеш) → поиск по ключу: найдена — вернуть существующую с `wasCreated=false`, нет — сохранить с `wasCreated=true`.

Ответ 200 в обоих случаях: `{ itemConfigurationId, campaignId, itemRevisionId, configurationKey, size, materialType, materialGrade, permanentUpgrades[], wasCreated }`. Идемпотентность структурная — по `ConfigurationKey`; повтор команды не создаёт дубликат (гонка закрыта unique-индексом). `UniqueItemAdministrationService` и миграционный сервис на новый сервис не переводить (follow-up).

### B2. Новая ревизия существующего вида

`POST api/item-catalog-admin/definitions/{itemDefinitionId:int}/revisions` — новый метод `CreateDraftForDefinitionAsync` в `ItemCatalogAdministrationService`: авторизация по scope вида (существующий `GetAuthorizedDefinitionAsync`), затем `CreateRevision` (номер = следующий, статус Draft). Тело: `{ name, description, level, priceInCopperPieces, bulk, rules }` — `rules` в формате существующего `ItemRevisionRulesApiRequest`. Существующий `POST drafts` (find-or-create по ключу) остаётся для «создать новый вид».

### B3. Read API каталога для администрирования

`GET api/item-catalog-admin/definitions?scope=&campaignId=&status=&search=&skip=&take=` — новый `Pathfinder.Web/Integration/ItemCatalogAdministrationProjectionService` (прецедент — `CommerceAdministrationProjectionService`; облегчённая проекция без компонентов правил).

- `scope`: `All|Global|Campaign`; `status`: `Draft|Published|Retired`; `search` — по ключу и названию ревизии (lowercase contains); `take` clamp 1–200, по умолчанию 50; ответ `{ totalCount, items[] }`, item = вид + сводки ревизий (номер, имя, level, цена, bulk, категория, редкость, статус, даты).
- Видимость: `canManageGlobal` и/или `canManageCampaign(campaignId)`; оба false → 403. Глобальная ветка видна обоим, но **GM не видит глобальные Draft** (фильтрация в SQL, точный `totalCount`); кампанийная ветка — только своя кампания. Администратор без GM-членства кампанийные виды не видит (admin-право ≠ доступ к кампании).

### B4. Capabilities

`GET api/item-catalog-admin/capabilities?campaignId=` → `{ canManageGlobalCatalog, campaignId, canManageCampaignCatalog }`. Всегда 200 для авторизованного пользователя; `campaignId` опционален. Источник прав для навигации фронтенда.

### Конвенции

Ошибки: `ItemCatalogAccessDeniedException` → 403 `Forbid()`; `ItemCatalogException`/`ItemCatalogApplicationException` → 400 `string[]` (`MapError`). Время — только `TimeProvider.GetUtcNow()`. Стиль C# — по `AGENTS.md`. DI: сервис конфигураций — `ItemCatalog.Application/IoC.cs`; projection — `Startup.cs` рядом с `CommerceAdministrationProjectionService`.

## Frontend

SPA `pathfinder.frontend` (Vue 3 + Vuetify). Без `v-data-table`; empty-state — `v-empty-state` с CTA; подтверждения — паттерн кампании (`*ConfirmTitle/*ConfirmText`, warning/error, `:loading`); недоступные действия — `:disabled` + `aria-describedby`-подсказка.

### F1. Маршруты, доступ, навигация

- `/campaigns/:campaignId(\d+)/item-catalog` (`campaign-item-catalog`) и `/item-catalog` (`global-item-catalog`) — один view `ItemCatalogAdminView.vue`, режим по наличию `campaignId`.
- Campaign-режим: проверка Active-кампании и роли GameMaster как в `CampaignCommerceAdminView.vue`; не-GM → редирект + снэкбар; 403 от API → то же.
- Global-режим: `capabilities` в auth-store (lazy `loadCapabilities()`, сброс при выходе); без права — редирект.
- Вход: пункт «Каталог предметов» в drawer (`App.vue`, виден при `canManageGlobalCatalog`); кнопка во вкладке «Торговля» кампании рядом с «Управление торговлей» (виден GM).

### F2. Экран каталога (эскизы 1–2)

Файлы: `src/features/item-catalog/` — `api.ts`, чистые модули `lifecycle.ts`, `filters.ts`, `draftForm.ts`, `options.ts`, `emptyState.ts` (+ co-located `*.spec.ts`), диалоги `DraftEditorDialog.vue`, `ConfigurationDialog.vue`; view `src/views/ItemCatalogAdminView.vue`.

- Фильтры: поиск (enter/иконка → сервер), scope-переключатель (только campaign-режим), статусные чипы (клиентское уточнение, `filters.ts`).
- Список: карточка на вид — название последней ревизии, ключ моноширинно, chip scope; строки ревизий (№, имя, level, `MoneyText`, chip статуса, редкость/категория) с раскрытием (описание, даты, для Published в campaign-режиме — блок конфигураций: chips-сводки + «Добавить конфигурацию»).
- Действия: Publish (у Draft), Retire (у Published), «+ Новая ревизия» на виде (disabled + подсказка, если последняя ревизия — Draft), «+ Создать определение» в шапке. Глобальные строки в campaign-режиме read-only с подсказкой; кнопка конфигурации на глобальных Published доступна (конфигурации принадлежат кампании).
- Состояния: `isLoading/errors/actionErrors/actionKey` + `useSnackbar`; ошибка загрузки — alert с «Повторить».

### F3. Диалог вида/ревизии (эскиз 3)

Все 9 категорий; секции компонентов по карте видимости в `draftForm.ts` (Оружие → атаки (repeatable) + снаряжение; Броня/Щит/Расходник/… — свои поля; заряды и прочность — свёрнутый блок «Дополнительно» для любой категории). `buildDraftRequest` отсекает скрытые секции; валидация до отправки (i18n-ключи ошибок). Режим «новая ревизия»: ключ заблокирован, prefill из ревизии с максимальным номером. `operationId` не нужен; двойной сабмит гасится `isSaving`.

### F4. Диалог конфигурации (эскиз 4)

Селекты размер/материал/категория материала (дефолты Medium/Standard/Standard) + редактор постоянных улучшений (код, вид, ранг, видимость; ≤ 16, уникальные коды — валидация в `options.ts`). Клиентская проверка дубликата по уже загруженным конфигурациям — submit disabled + подсказка. Ответ `wasCreated=false` → info-снэкбар «уже существует», `true` → success. После создания — точечный refetch раскрытого вида.

### F5. Подтверждения (эскиз 5)

- Publish (warning): если есть текущая опубликованная ревизия — обязательный текст «текущая опубликованная ревизия №N будет автоматически отозвана; вернуть её будет нельзя» (`lifecycle.ts::publishConsequence`); иначе короткий вариант.
- Retire (error + inline warning-alert): «безвозвратно; предмет исчезнет из каталога для новых предложений; существующие конфигурации и выданные предметы не удаляются».

### F6. Пустые состояния и deep-links (эскизы 1, 6, 7)

| Где | Причина (`emptyState.ts`) | CTA |
|---|---|---|
| Экран каталога | `noDefinitions` | «Создать определение» (открывает диалог) |
| Экран каталога | `noMatches` (фильтры скрыли всё) | «Сбросить фильтры» |
| Раскрытая Published-ревизия | нет конфигураций | «Добавить конфигурацию» |
| Диалог «Добавить из каталога» | `noPublishedRevisions` | → `/campaigns/:id/item-catalog?action=create-draft` |
| Диалог «Добавить из каталога» | `noConfigurations` | → `/campaigns/:id/item-catalog?action=configure&status=Published` |

Deep-links: `action=create-draft` открывает диалог создания, `action=configure` пресетит чип Published; после обработки query очищается `router.replace`. Требование к будущему restock-UI (эскиз 7): пустое превью пополнения различает «нет кандидатов вообще» (CTA в каталог тем же deep-link) и «правило всё отфильтровало»; в текущий объём restock-UI не входит.

### F7. Интеграция с Commerce

В диалоге «Добавить из каталога» (`CampaignCommerceAdminView.vue`): два различимых empty-state из F6 вместо молчаливо пустого селекта; метки опций конфигураций локализовать (инжект форматтера в `catalogConfigurationOptions`, сейчас в UI утекают сырые коды вида `ColdIron`). Создание конфигурации внутрь диалога не встраивать — переход по CTA.

### F8. Локализация

Namespace `itemCatalogUi` (ru + en) + ключи `routes.*`, `app.navigation.itemCatalog`, `commerceUi.campaign.openItemCatalog`, `commerceAdmin.shop.catalogNo{Revisions,Configurations}*`; добавить `itemCatalogUi` в `messagesParity.spec.ts`. Enum-метки verbatim-ключами (`sizes.ColdIron` и т.п.) через хелперы в `src/i18n/domain.ts` с `te()`-fallback; категории — переиспользовать `inventoryUi.categories.*`.

## Связь с генерацией ассортимента (Priority 16)

Созданная конфигурация автоматически попадает в пул `GetRestockCandidatesAsync` — отдельная интеграция не нужна. После согласования правила видимости ручное добавление товара и генератор видят одинаковый набор конфигураций. Подтверждение restock-прогона (`ConfirmAsync`) не перепроверяет конфигурации — поведение не меняется, превью уже фильтровало корректно.

## Сценарии

1. **Глобальный happy path:** администратор из drawer открывает `/item-catalog` → создаёт вид → черновик → публикует (confirm) → GM видит ревизию в своём каталоге.
2. **Кампанийный happy path:** GM создаёт вид кампании → публикует → создаёт конфигурацию → открывает «Добавить из каталога» → конфигурация в списке → создаёт предложение → предложение в ассортименте, игрок покупает существующим торговым сценарием.
3. **Идемпотентность:** повтор команды конфигурации → тот же `itemConfigurationId`, `wasCreated=false`, info-снэкбар, дубликата нет.
4. **Замещающая публикация:** publish №2 при опубликованной №1 → confirm с текстом об авто-отзыве → №1 Retired, №2 Published (в БД максимум одна Published на вид).
5. **Границы ролей:** GM не видит глобальные Draft, не может publish/retire глобальной ревизии и чужого campaign-вида (403/«не найдено»); администратор без GM-членства не управляет конфигурациями кампании.
6. **Отказы:** конфигурация для Draft/Retired-ревизии → 400 с различимым сообщением; невалидные улучшения (дубликат кода, > 16) — валидация до запроса + серверная ошибка в alert.
7. **Пустые состояния:** новый GM без данных проходит цепочку CTA: пустой диалог Commerce → каталог → создание вида → публикация → конфигурация → возврат и создание предложения.

## Тесты

| Слой | Проект | Покрытие |
|---|---|---|
| Домен | `ItemCatalog.Domain.Tests` | при пробелах: детерминизм `ConfigurationKey` между кампаниями, независимость от порядка улучшений |
| Приложение/инфраструктура | `ItemCatalog.Infrastructure.Tests` | `ItemConfigurationAdministrationServiceTests` (создание, дедуп + `wasCreated`, Draft/Retired, отказ не-GM, чужая кампания, улучшения); дополнение `ItemCatalogAdministrationServiceTests` (новая ревизия: следующий номер, чужой GM, not found); `CommerceCatalogReaderTests` (cross-campaign reject, legacy null accept, restock-кандидаты) |
| Web/Integration | `CharacterManagement.Infrastructure.Tests` | `ItemCatalogAdministrationProjectionServiceTests` (видимость admin/GM/чужой/без ролей, глобальные Draft, поиск, статус-фильтр, paging/clamp); дополнения `CommerceAdministrationProjectionServiceTests` (legacy видимы, чужие нет) и `CompletedCharacterInventoryMigrationServiceTests` (своя конфигурация вместо чужой) |
| Frontend | Vitest, чистые модули | `lifecycle/filters/draftForm/options/emptyState.spec.ts`, parity-спек, обновлённый `admin.spec.ts`; компонентные тесты не требуются (нет прецедента) |

Regression: существующие campaign/inventory/commerce тесты, особенно `UniqueItemAdministrationServiceTests`, проходят без изменения поведения.

## Критерии приёмки

- [ ] Администратор через UI создаёт глобальный вид, черновик ревизии и публикует его.
- [ ] GM через UI создаёт вид своей кампании и не может управлять глобальным или чужим campaign scope.
- [ ] Для опубликованной глобальной или кампанийной ревизии GM создаёт конфигурацию своей кампании; повтор команды не создаёт дубликат (`wasCreated=false`).
- [ ] Draft и Retired ревизии нельзя использовать для конфигурации и нового предложения магазина; сообщения ошибок различимы.
- [ ] Созданная конфигурация сразу видна в «Добавить из каталога»; созданное предложение отображается в ассортименте и покупается существующим торговым сценарием; legacy-конфигурации (`CampaignId == null`) видны, чужих кампаний — нет.
- [ ] Пустой каталог и отсутствие конфигураций дают различимые empty-state с рабочими CTA и deep-links (сценарий 7 проходит целиком).
- [ ] Публикация с замещением показывает предупреждение об авто-отзыве; retire оформлен как безвозвратное действие.
- [ ] ru/en локализация полная, parity-тест зелёный; сырые enum-коды в UI не отображаются.
- [ ] Backend и frontend автотесты из раздела «Тесты» зелёные; изменений EF-модели нет.
- [ ] SQL-аудит cross-campaign ссылок приложен к PR.

## Вне охвата

- UI управления правилами и прогонами пополнения (restock) — только требование к его будущему пустому состоянию (эскиз 7).
- Рефакторинг `UniqueItemAdministrationService` и `CompletedCharacterInventoryMigrationService` на новый сервис конфигураций (кроме предиката видимости) — follow-up.
- Редактирование/удаление черновиков, `operationId`-идемпотентность ItemCatalog-команд (структурной достаточно), глобальные (`CampaignId == null`) конфигурации через новую команду.
- Ремедиация данных по результатам SQL-аудита (отдельная операция при необходимости).

## Состояние

Реализовано полностью (backend B1–B4, согласование правил видимости, frontend F1–F8) 29 июля 2026 года.

- Backend: `ItemConfigurationAdministrationService` + `ItemConfigurationsAdminController`; `CreateDraftForDefinitionAsync` + `POST definitions/{id}/revisions`; `ItemCatalogAdministrationProjectionService` + `GET definitions` + `GET capabilities`; предикаты `CommerceCatalogReader`, `SearchPublishedRevisionsAsync` и shape-поиска миграции приведены к единому правилу видимости.
- Frontend: маршруты `campaign-item-catalog`/`global-item-catalog` на общий `ItemCatalogAdminView`, `DraftEditorDialog`, `ConfigurationDialog`, confirm-диалоги publish/retire, empty-state с CTA и deep-links в диалоге «Добавить из каталога», capabilities в auth-store, namespace `itemCatalogUi` (ru/en, parity), локализованные метки конфигураций в опциях Commerce.
- Существующие конфигурации ревизии экран получает через `GET /api/item-catalog/revisions` (проекция Commerce), поэтому клиентская проверка дубликата — подсказка, а не блокировка: улучшения существующих конфигураций проекция не возвращает, авторитетен сервер (`wasCreated=false`).
- По итогам код-ревью дополнительно: empty-state различает «пусто» и «ничего не найдено» при серверном поиске (экран и Commerce-диалог); scope-переключатель All/Global/Campaign в campaign-режиме; `Description` в `ItemRevisionSummaryDto` (prefill и раскрытая ревизия показывают описание; компоненты правил в новую ревизию не переносятся — в диалоге есть info-подсказка); гонка идемпотентности конфигурации закрыта re-query по `ConfigurationKey` после неудачного Commit; невалидные scope/status фильтры → 400; политика `take` целиком в projection-сервисе (≤0 → 50, максимум 200); ordering предпочтения кампанийной конфигурации в миграции переписан null-безопасно (`CampaignId != null`); сообщение о черновике глобальной ревизии не раскрывается GM («не найдено»); deep-link не вызывает повторную загрузку и стирает только свои query-параметры; ошибки загрузки capabilities показываются как ошибка с retry, а не как отказ в доступе.
- Проверки: `dotnet test` — 829 зелёных; `npm test` — 157 зелёных; `npm run build` и `lint` чистые; `dotnet ef migrations has-pending-model-changes` — изменений нет.
- SQL-аудит: [item_catalog_cross_campaign_audit.sql](item_catalog_cross_campaign_audit.sql) — выполнить перед деплоем; ремедиация по результатам вне охвата. Restock-UI — вне охвата.