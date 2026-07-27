# Priority 17 — итоговый cross-review

**Дата:** 27 июля 2026 года
**Статус:** завершён

## Проверенный объём

Priority 17 выполнен последовательными срезами:

1. Runtime-заряды, расходование и восстановление.
2. Расходуемые предметы, боеприпасы и уменьшение стопки.
3. Твёрдость, текущая прочность, порог поломки, уничтожение и ремонт.
4. Прикрепляемые руны и перенос между совместимыми экземплярами.
5. Campaign-scoped server commands с `expectedVersion` и `operationId`.
6. Trusted integration port для будущих `Gameplay`/`Encounter`.

Реализация зафиксирована коммитами:

- `ec37224` — runtime item charges;
- `f739e91` — consumables, ammunition и depletion;
- `cbe0439` — durability lifecycle;
- `7783504` — attachable rune transfer;
- `e6d568c` — авторизованные lifecycle commands и HTTP API;
- `68c9056` — trusted gameplay lifecycle port;
- `c3530b3` — исправление attachment/access по результатам cross-review.

## Результат проверки критериев

- Текущие заряды и прочность ограничены доменными инвариантами и DB constraints.
- `expectedVersion` разрешает только одного победителя при конкурентном расходовании последнего заряда или единицы стопки.
- Повтор с тем же `operationId` и теми же аргументами идемпотентен; конфликтное повторное использование отклоняется.
- Расходование уникального экземпляра завершает его lifecycle, а stack consumption использует сохранённый размер расхода.
- Hardness применяется до уменьшения HP; broken threshold, уничтожение и запрет обычного ремонта уничтоженного экземпляра вычисляются сервером.
- Руна прикрепляется и переносится только между совместимыми campaign-scoped экземплярами; multi-item change сохраняется одной транзакцией.
- Прикреплённая руна не перемещается и не резервируется отдельно, а пользовательский доступ разрешается через текущего носителя.
- Публичные команды авторизуют ведущего или управляющего персонажем; trusted port не опубликован через HTTP.
- `CharacterManagement` и frontend не изменяют lifecycle state напрямую и не ссылаются на trusted port.

## Замечания cross-review и исправления

Cross-review выявил, что прикреплённая руна сохраняла исходный контейнер как техническое местоположение. Обычные move/reserve могли рассматривать её как самостоятельный предмет, а после передачи носителя lifecycle authorization продолжала смотреть на прежнего владельца.

Исправлено:

- обычные move, reserve и force move отклоняются для прикреплённой руны;
- lifecycle authorization прикреплённой руны использует текущий контейнер носителя;
- добавлены domain и integration regression tests для независимого перемещения и смены владельца носителя.

Неразрешённых блокирующих замечаний в объёме Priority 17 не осталось.

## Quality gate

- `dotnet test Pathfinder.sln --no-restore --verbosity minimal` — успешно для всей solution.
- `Inventory.Domain.Tests` — 84 теста успешно.
- `Inventory.Infrastructure.Tests` — 23 теста успешно.
- `CampaignManagement.Infrastructure.Tests` — 11 тестов успешно.
- `CharacterManagement.Infrastructure.Tests` — 320 тестов успешно.
- `dotnet build Pathfinder.Web/Pathfinder.Web.csproj --no-restore` — успешно.
- Все четыре EF migration созданы через `dotnet ef`; snapshot согласован.
- `git diff --check` — ошибок форматирования нет.

Полная solution сохраняет ранее существовавшие nullable и duplicate-type warnings вне объёма Priority 17; новых ошибок сборки или тестов нет.

## Вне охвата

Полный encounter engine, runtime execution spell/feat effects, crafting, loot и торговый UI не входят в Priority 17. Trusted port предоставляет только Inventory-owned mutation boundary для их будущей реализации.
