# Final Free Boosts Implementation Plan

## Проблема

Текущий character creation применяет ancestry, background и class packages, но не выдаёт четыре финальных свободных boosts первого уровня. Из-за этого стартовый pipeline характеристик не завершён, а итоговые `AbilityScores` не соответствуют полному набору выборов при создании персонажа.

Существующий `AppliedFreeBoosts` относится только к free boosts родословной. Использовать его для финального этапа нельзя: пакеты применяются в разное время, имеют разные правила выбора и должны заменяться независимо.

## Ожидаемый результат

Новый персонаж после выбора `Class` получает обязательный отдельный пакет из четырёх boosts, распределённых по четырём разным характеристикам. Сервер проверяет пакет, применяет каждый boost как `+2`, сохраняет подтверждённый выбор и возвращает его в read-модели.

Frontend добавляет отдельный шаг `Final boosts` после `Class`, не позволяет продолжить без четырёх уникальных выборов и показывает пакет на review и в карточке персонажа.

После этой задачи стартовый ability pipeline имеет полный порядок:

1. ancestry boosts/flaws и ancestry free boosts;
2. два background boosts;
3. class key ability boost;
4. четыре final free boosts.

## Источник правил

Нормативная модель проекта описана в [`../20_domain/character_creation/domain_rules_target_full.md`](../20_domain/character_creation/domain_rules_target_full.md):

- final boosts образуют отдельный одновременный пакет;
- пакет содержит четыре разных `AbilityType`;
- каждый boost первого уровня повышает score на `2`;
- итоговый modifier на первом уровне не превышает `+4`, то есть score не превышает `18`.

Partial boost не нужен в этом срезе. При корректном порядке существующих packages одна характеристика до final boosts может получить не более трёх boosts и достигнуть score `16`. Один boost из финального пакета доводит её максимум до `18`. Если агрегат находится в некорректном состоянии и выбранный boost привёл бы к score выше `18`, домен отклоняет пакет, а не применяет ещё не реализованную partial-boost механику.

## Граница задачи

### Входит

- отдельный подтверждённый пакет `AppliedFinalFreeBoosts` в `DraftCharacter`;
- ровно четыре уникальных `AbilityType`;
- проверка наличия class package перед применением final boosts;
- применение, повторное применение и независимый откат final package;
- ограничение итогового score первого уровня значением `18`;
- расширение character builder и `POST /api/character`;
- серверная валидация transport-контракта и доменных инвариантов;
- хранение пакета через EF Core и migration, созданную только через `dotnet ef`;
- возврат `FinalFreeBoosts` в character read-модели;
- отдельный шаг frontend wizard после `Class`;
- отображение выбора на review и в карточке персонажа;
- domain, application/integration и frontend tests;
- обновление MemoryBank и отдельный финальный code review.

### Не входит

- partial boosts для score `18+`;
- boosts на уровнях `5`, `10`, `15` и `20`;
- voluntary flaws и alternate ancestry boosts;
- ability modifier как отдельное новое поле read-модели;
- вычисление Hit Points и других derived statistics;
- финализация `DraftCharacter` и запрет дальнейшего редактирования packages;
- skills, proficiencies, class features, spells, deity и equipment;
- изменение правил ancestry/background/class packages.

## Доменные решения

1. `DraftCharacter` хранит `AppliedFinalFreeBoosts` отдельно от ancestry `AppliedFreeBoosts`.
2. Пакет считается применённым, только если содержит ровно четыре уникальных значения `AbilityType`.
3. Final package можно установить только после полного class package. Это закрепляет порядок источников на доменной границе, а не только в builder или UI.
4. Уникальность действует внутри final package. Характеристика может повторять выбор из ancestry, background или class package.
5. Перед заменой пакета домен откатывает только предыдущие final boosts, затем проверяет и применяет новый пакет.
6. Неуспешная замена не должна оставлять агрегат частично изменённым. Новый пакет полностью проверяется до мутации либо старое состояние восстанавливается атомарно.
7. Каждый boost применяет существующую доменную операцию повышения score на `2`.
8. После применения ни одна выбранная характеристика не может превышать score `18`.
9. Клиент передаёт только четыре `AbilityType`; рассчитанные `AbilityScores` сервер не принимает.
10. Порядок значений не имеет игрового смысла. Для стабильного persistence/read-контракта сервер возвращает подтверждённый порядок либо нормализует его единообразно; тесты не должны приписывать порядку дополнительную семантику.
11. Существующие записи без final package остаются читаемыми как пустой список. Новые запросы создания обязаны передать полный пакет.

## Предлагаемые контракты

### Domain

`DraftCharacter` получает:

- `IReadOnlyList<AbilityType> AppliedFinalFreeBoosts` с пустым значением по умолчанию;
- `HasFinalFreeBoostPackage`;
- `SetFinalFreeBoosts( IReadOnlyList<AbilityType> finalFreeBoosts )`.

Builder получает отдельный метод `SetFinalFreeBoosts`. Существующий `ApplyFreeBoosts` остаётся ancestry-specific и в рамках задачи не переименовывается, чтобы не расширять рефакторинг.

### Создание персонажа

В `POST /api/character` добавляется обязательное поле:

```json
{
  "finalFreeBoosts": [
    "Strength",
    "Dexterity",
    "Constitution",
    "Wisdom"
  ]
}
```

На transport boundary поле моделируется как nullable `IReadOnlyList<AbilityType>?`, чтобы validator отличал отсутствующее поле от переданного пустого списка. После успешной валидации handler передаёт в домен non-null коллекцию.

Transport validator проверяет:

- поле передано;
- количество значений равно четырём;
- значения уникальны;
- все значения принадлежат `AbilityType`.

Проверки порядка packages и score cap остаются в домене. Доменная ошибка создания преобразуется в HTTP `400` существующим error flow.

### Read-модель

`CharacterDto` получает `FinalFreeBoosts` как список `AbilityType`.

- Для новых персонажей список содержит четыре значения.
- Для legacy-персонажей список пуст.
- Отдельный catalog endpoint не нужен: набор допустимых характеристик уже известен клиенту и совпадает с `AbilityType`.

### Persistence

`AppliedFinalFreeBoosts` хранится отдельной JSON-колонкой по тому же техническому подходу, который уже используется для ancestry `AppliedFreeBoosts`.

Migration должна:

- добавлять новую колонку с безопасным пустым значением для существующих строк;
- не изменять `AppliedFreeBoosts`;
- корректно обновлять model snapshot;
- создаваться командой `dotnet ef`, а не вручную.

## Frontend flow

После шага `Class` добавляется шаг `Final boosts`.

Поведение шага:

- отображаются все шесть характеристик с текущими значениями перед final package;
- пользователь выбирает четыре характеристики;
- повторный выбор той же характеристики невозможен;
- UI явно показывает `выбрано N из 4`;
- после четырёх выборов оставшиеся характеристики недоступны до снятия одного выбора;
- review показывает выбранные boosts и итоговые scores;
- create request отправляет `finalFreeBoosts`;
- карточка персонажа показывает final package отдельно от ancestry/background/class sources.

UI-ограничения не заменяют серверную валидацию.

## Этапы выполнения

1. Перепроверить нормативные правила final boosts, текущее максимальное значение score перед пакетом и persistence naming; зафиксировать спорные решения до изменения кода.
2. Реализовать доменный пакет, порядок применения, атомарную замену, cap `18` и domain tests.
3. Расширить builder, create DTO/validator/handler, character DTO/converter и application/integration tests.
4. Добавить EF mapping и migration через `dotnet ef`; проверить сохранение, загрузку и чтение legacy-персонажа.
5. Добавить frontend API type, шаг wizard, review, карточку, локализацию и frontend tests.
6. Обновить MemoryBank и выполнить проверки активного backend/frontend scope.
7. Провести отдельный code review, исправить замечания и повторить затронутые проверки.

После каждого этапа оставшийся план пересматривается. Изменение порядка packages, обнаруженная необходимость partial boosts или расширение в derived statistics оформляются явно и не добавляются в scope молча.

## Критерии готовности

- новый create request без `FinalFreeBoosts` отклоняется;
- сервер принимает ровно четыре разные характеристики и отклоняет другое количество или дубли;
- final package нельзя применить до class package;
- каждый final boost повышает выбранный score на `2`;
- одна характеристика может повторять выбор из предыдущего package;
- итоговый score первого уровня не превышает `18`;
- повторная установка того же пакета не накапливает boosts;
- смена пакета откатывает только предыдущие final boosts и не изменяет ancestry/background/class packages;
- неуспешная замена сохраняет прежнее согласованное состояние агрегата;
- пакет сохраняется и восстанавливается из PostgreSQL;
- legacy-персонаж без пакета остаётся читаемым;
- read-модель возвращает источник final boosts отдельно;
- frontend не позволяет перейти к review с неполным или дублирующимся выбором;
- review и карточка показывают final package и итоговые scores;
- domain tests, application/integration tests, frontend tests, build и lint проходят;
- финальный code review не содержит незакрытых замечаний в scope задачи.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- frontend unit tests для final-boost helper и i18n/read отображения;
- frontend lint и production build;
- `git diff --check`;
- полный `Pathfinder.sln` запускается как диагностическая проверка с учётом известного legacy blocker в `Store.Application`.

## Статус выполнения

- план и граница задачи зафиксированы;
- этапы 1–6 завершены: домен, application/API, EF migration, frontend и документация реализованы;
- migration: `20260713103506_AddFinalFreeBoosts`;
- domain tests: `43`, application/integration tests: `54`, frontend tests: `14`;
- frontend lint и production build проходят;
- финальный code review завершён: исправлены устаревший roadmap по уже существующим ability modifiers и мобильная сетка восьми шагов wizard;
- открытых замечаний в scope задачи нет.

## Связанные документы

- [Near-term roadmap](character_creation_near_term_roadmap.md)
- [Class package implementation](class_package_implementation.md)
- [Target full domain rules](../20_domain/character_creation/domain_rules_target_full.md)
- [Backend MVP](mvp_character_creation_backend.md)
- [Frontend MVP](mvp_character_creation_frontend.md)
