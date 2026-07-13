# Background Package Implementation

## Проблема

Создание персонажа останавливается после выбора `Ancestry`: пользователь не может выбрать `Background`, применить два положенных background boosts и увидеть этот выбор в карточке персонажа.

Из-за этого итоговые характеристики отражают только ancestry-этап, а дальнейшее расширение flow в сторону `Class` и финальных boosts не имеет отдельного background-пакета правил.

## Ожидаемый результат

Новый персонаж создаётся с обязательным `Background`. Сервер проверяет и применяет два background boosts после ancestry boosts, сохраняет выбор и возвращает background package в read-модели. Frontend позволяет выполнить этот выбор и показывает результат.

Skill, Lore и skill feat включаются в каталог и API как декларативные grants, но не применяются к персонажу до появления отдельной Skill subsystem.

## Источник данных

Baseline задачи — 35 backgrounds из [`../20_domain/character_creation/background_catalog_core_rulebook.md`](../20_domain/character_creation/background_catalog_core_rulebook.md).

По правилам [`../10_workflow/aon_elasticsearch_usage.md`](../10_workflow/aon_elasticsearch_usage.md) этот Core Rulebook baseline используется как текущий базовый Player Core scope. `Player Core 2`, APG, PFS и adventure-specific backgrounds не входят в задачу.

## Граница задачи

### Входит

- справочный каталог `Background` вне агрегата;
- стабильные идентификаторы background и декларативных grants;
- выбор одного boost из заданной пары;
- выбор одного свободного boost;
- применение и откат background boosts отдельно от ancestry boosts;
- хранение выбранного background и boost choices;
- `GET /api/backgrounds`;
- расширение `POST /api/character` и character read-модели;
- миграция EF Core;
- новый шаг frontend wizard и отображение в карточке;
- domain, application, integration и frontend tests.

### Не входит

- фактическое обучение skills и Lore;
- уровни proficiency;
- применение skill feat;
- разрешение конфликтов training от `Background` и будущего `Class`;
- свободный ввод Lore;
- `Class`, четыре финальных boosts и partial boosts.

## Доменные решения

1. Каталог `Background` живёт вне `DraftCharacter`, как каталог `Ancestry`.
2. `DraftCharacter` хранит только подтверждённый выбор пользователя: background id, выбранный ограниченный boost и свободный boost.
3. Два background boosts образуют один пакет и обязаны указывать разные характеристики.
4. Background boost может повторять характеристику, ранее повышенную ancestry package: ограничение уникальности действует внутри одного пакета, а не между этапами.
5. Сервер вычисляет итоговые `AbilityScores`; клиент не передаёт готовые значения.
6. Существующие персонажи без Background остаются читаемыми после миграции, но новые запросы создания обязаны содержать полный background choice.
7. Skill, Lore и skill feat возвращаются как стабильные идентификаторы и человекочитаемые fallback-названия. Переменные grants остаются декларативными requirements.

## API-контракт

### Каталог

```text
GET /api/backgrounds
```

Каждая запись содержит:

- `id`;
- `name`;
- `source`;
- `restrictedBoostOptions`;
- `freeBoostCount`;
- декларативные skill, Lore и skill feat grants.

### Создание персонажа

В `POST /api/character` добавляются:

```json
{
  "backgroundId": "background.acrobat",
  "backgroundRestrictedBoost": "Dexterity",
  "backgroundFreeBoost": "Intelligence"
}
```

### Read-модель

`CharacterDto` получает nullable `BackgroundPackage`: старые записи могут не иметь Background, новые — обязаны иметь.

## Этапы выполнения

1. Зафиксировать scope, baseline, контракты и критерии готовности.
2. Реализовать доменную модель, каталог, применение/откат boosts и unit-тесты.
3. Добавить application/API контракты и тесты создания/каталога.
4. Добавить EF persistence, миграцию и round-trip integration tests.
5. Расширить frontend wizard и карточку, добавить frontend tests.
6. Обновить MemoryBank и выполнить полные проверки.
7. Провести отдельный code review, устранить замечания и повторить проверки.

После каждого этапа план пересматривается с учётом фактического состояния реализации.

## Критерии готовности

- каталог возвращает все 35 baseline backgrounds;
- пользователь выбирает один разрешённый restricted boost и один отличный от него free boost;
- сервер отклоняет неизвестный background, запрещённый restricted boost и дублирующиеся boosts;
- background boost разрешено применять к характеристике, уже повышенной ancestry package;
- смена Background откатывает только предыдущие background boosts;
- выбор и boosts сохраняются и восстанавливаются из PostgreSQL;
- character read-модель и frontend-карточка показывают Background;
- skill, Lore и skill feat видны как декларативные grants, но не изменяют несуществующую skill-модель;
- существующий персонаж без Background остаётся читаемым;
- backend tests, frontend tests, build и lint проходят;
- финальный code review не содержит незакрытых замечаний.

## Статус выполнения

- этапы 1–7 завершены;
- реализованы домен, каталог из 35 Background, application/API, EF migration и frontend;
- backend unit/integration: `28 + 41` тестов проходят;
- `Pathfinder.Web` собирается; frontend test/build/lint проходят;
- финальный code review устранён: обязательные boost-поля сделаны nullable на транспортной границе с явной валидацией, доменные ошибки создания персонажа отображаются как HTTP 400;
- `git diff --check` проходит;
- полный solution build по-прежнему блокируется ранее отключённым legacy `Store.Application` и его отсутствующими типами; Background scope этот known issue не затрагивает;
- визуально проверена загрузка оболочки frontend; защищённый wizard не проверен end-to-end локально без `Authentication:SecurityKey`.
