# Priority 10 Final Cross-Review

## Итог

Приоритет 10 «Версионируемый каталог предметов v2» завершён 22 июля 2026 года. Реализованы шесть последовательных vertical slices:

1. Stable item definition и неизменяемые revision snapshots.
2. Основная категория и типизированные компоненты правил.
3. Global/campaign scopes и изоляция ключей.
4. Неизменяемая конфигурация из ревизии, размера, материала и постоянных улучшений.
5. Lifecycle `Draft -> Published -> Retired`, административный API и проверка полномочий.
6. Переходный адаптер `IAllowedEquipmentReader` поверх опубликованного ItemCatalog без переноса character-owned state.

Коммиты реализации: `ca9e6f9`, `06d34a5`, `96d2033`, `1c1c074`, `c595511`, `8254737`.

## Проверка критериев

| Критерий | Результат |
|---|---|
| Опубликованная ревизия не изменяется на месте | Выполнено: публичных mutators нет; следующая версия создаётся отдельным snapshot. Lifecycle и timestamps защищены доменными инвариантами и DB check constraint. |
| Щит является композицией, а не наследником брони | Выполнено: shield revision требует `ShieldComponent`, attack, equipment и durability; `ArmorComponent` не используется как базовый тип щита. |
| Базовый кубик урона принадлежит ревизии | Выполнено: die count/size/type находятся в `AttackComponent` revision. |
| Размер, материал и постоянные улучшения принадлежат конфигурации | Выполнено: `ItemConfiguration` неизменяемо связывает точную revision с size, material/grade и типизированными upgrades. |
| Глобальными описаниями управляет системный администратор | Выполнено: Web adapter проверяет `Permissions_Administration`; клиент не передаёт доверенный флаг администратора. |
| Ведущий управляет только собственной кампанией | Выполнено: требуется активная кампания и активное membership `GameMaster` точного `CampaignId`. |
| Административный ввод не принимает исполняемый код или непроверяемые формулы | Выполнено: API принимает только типизированные enums, числа, строки и component records; script/formula payload отсутствует. |

## Cross-slice review

- `ItemDefinition.Key` стабилен, нормализуется и уникален отдельно для global и каждой campaign scope.
- Ревизии получают последовательные номера и сохраняют snapshot базовых полей и компонентов; одна коллекция rules не может быть повторно присвоена другой ревизии.
- Категория используется для навигации, а поведение складывается из независимых attack, armor, shield, equipment, consumption, charge и durability components.
- DB schema `item_catalog` фиксирует scope check, partial unique indexes ключей и единственную published revision на definition.
- Lifecycle check constraint согласует `Status`, `PublishedAtUtc` и `RetiredAtUtc`; публикация новой revision автоматически выводит предыдущую из обращения.
- `ItemConfiguration` имеет детерминированный composition key и уникальный индекс; permanent upgrades валидируются до изменения агрегата, включая visibility.
- Административные create/publish/retire сценарии проходят через application service и `IUnitOfWork`; scope авторизуется по сохранённому definition, а не по клиентскому параметру lifecycle-команды.
- `CharacterManagement.Domain` и `CharacterManagement.Application` не зависят от ItemCatalog. Web integration adapter реализует существующий application-owned `IAllowedEquipmentReader`.
- Для campaign card опубликованная revision точной кампании перекрывает global; другая campaign scope невидима. Draft и retired revisions не влияют на боевую карточку.
- Quantity и equipped state остаются character-owned. Priority 10 не создаёт runtime item instances и не меняет ownership.

## Миграции

Созданы пять последовательных миграций ItemCatalog:

1. `InitialItemCatalog`.
2. `AddTypedRuleComponents`.
3. `ScopeItemDefinitions`.
4. `AddItemConfigurations`.
5. `AddItemRevisionLifecycle`.

`dotnet ef migrations has-pending-model-changes` подтверждает отсутствие расхождения модели и snapshot. Миграции в локальную PostgreSQL не применялись: доступный connection string отверг credentials пользователя `postgres`; проверка выполнялась на модели, migration files и in-memory persistence tests.

## Quality gate

- `dotnet test Pathfinder.sln --no-restore`: passed, 611 backend tests:
  - `CharacterManagement.Domain.Tests` — 242;
  - `CharacterManagement.Infrastructure.Tests` — 302;
  - `CampaignManagement.Domain.Tests` — 17;
  - `CampaignManagement.Infrastructure.Tests` — 11;
  - `ItemCatalog.Domain.Tests` — 31;
  - `ItemCatalog.Infrastructure.Tests` — 8.
- Frontend Vitest — 85 tests в 30 files.
- Frontend ESLint — passed.
- Frontend production build — passed; сохраняется известное предупреждение Vite о chunk больше 500 kB.
- Scoped `git diff --check` — passed.
- Serena diagnostics для ключевых новых domain/application/integration файлов — без ошибок; один существующий `IDE0270` в campaign handler не относится к изменению поведения.

Существующие repository-wide compiler warnings не изменены этим приоритетом. EF Core также выводит предупреждения о sentinel для enum defaults, но доменные фабрики всегда задают валидные non-zero значения.

## Переходные ограничения

- Class kits и создание starting selection пока используют legacy `EquipmentRepository`; ItemCatalog adapter накладывает published descriptions только на уже известные stable equipment keys.
- Недостающие в ItemCatalog v2 metadata Priority 8 — weapon/armor proficiency groups, traits, rarity и units per purchase — временно берутся из starting-catalog fallback.
- Personal character endpoint без campaign context использует только global descriptions; campaign override применяется через campaign-scoped card endpoint.
- Heavy armor и weapon attacks с несколькими damage dice пока не выражаются контрактом combat card v1 и отклоняются адаптером.
- Административный backend API реализован без отдельного frontend UI и без универсального effect execution engine.
- Runtime identity, ownership, containers, stacks, mutable charges/durability state и migration completed-character equipment относятся к Priority 11.

Vikunja в текущем окружении недоступна. Локальная документация обновлена полностью; внешние карточки необходимо синхронизировать после восстановления подключения.
