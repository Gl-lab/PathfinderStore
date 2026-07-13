# Derived Statistics v1: Hit Points Implementation Plan

## Проблема

Текущий character creation уже хранит все источники стартовых Hit Points:

- ancestry base HP и heritage `BaseHpOverride`;
- class base HP;
- итоговый Constitution score и вычисляемый modifier.

При этом character read-модель возвращает эти части раздельно и не вычисляет maximum HP. Frontend показывает ancestry HP и class HP как справочные значения, поэтому пользователь должен складывать их самостоятельно и учитывать Constitution modifier.

## Ожидаемый результат

Сервер вычисляет maximum HP персонажа первого уровня единым доменным правилом:

```text
MaximumHitPoints = EffectiveAncestryHitPoints + ClassHitPoints + ConstitutionModifier
```

Read-модель возвращает maximum HP и объяснимый breakdown. Список персонажей показывает maximum HP, а карточка — итог и составляющие расчёта.

HP не принимаются от клиента и не сохраняются отдельной колонкой. Изменение ancestry/heritage, class или Constitution автоматически отражается в следующей read-модели без синхронизации дублирующего состояния.

## Источник правил

- [AoN: Character Creation, Step 7](https://2e.aonprd.com/Rules.aspx?ID=66) описывает стартовые HP как сумму ancestry HP, class HP и Constitution modifier.
- [AoN: Hit Points, Healing, and Dying](https://2e.aonprd.com/Rules.aspx?ID=2263) отделяет maximum HP от current и temporary HP и учитывает дополнительные источники, которые появятся позже.
- Проектная целевая модель находится в [`../20_domain/character_creation/domain_rules_target_full.md`](../20_domain/character_creation/domain_rules_target_full.md).

Для v1 персонаж считается персонажем первого уровня. Явного `Level` в текущем `DraftCharacter` нет; progression нельзя неявно моделировать в этом срезе.

## Граница задачи

### Входит

- единое доменное разрешение effective ancestry HP с учётом выбранного `BaseHpOverride`;
- типизированный доменный результат maximum HP с breakdown;
- формула первого уровня: ancestry HP + class HP + Constitution modifier;
- использование уже существующего `Characteristic.Modifier`;
- nullable derived statistics для legacy draft без Class;
- расширение `CharacterDto` без изменения create request;
- одинаковый расчёт для list и details queries;
- отображение maximum HP в dashboard;
- отображение maximum HP и breakdown в карточке персонажа;
- domain, application/integration и frontend tests;
- обновление MemoryBank и финальный code review.

### Не входит

- current HP, damage, healing и dying rules;
- temporary HP;
- Toughness, class features, feats, items и другие дополнительные HP sources;
- level-up HP и создание общей progression system;
- retraining и история изменения maximum HP;
- AC, saves, perception, speed, proficiency bonuses и initiative;
- хранение derived statistics в PostgreSQL;
- EF migration;
- изменение create request или wizard choices.

## Доменные решения

1. Maximum HP является вычисляемым значением, а не состоянием агрегата.
2. Доменный результат содержит `AncestryHitPoints`, `ClassHitPoints`, `ConstitutionModifier` и `MaximumHitPoints`.
3. Effective ancestry HP вычисляется из ancestry catalog entry и выбранных character choices. Базовое значение используется, если выбранные effects не содержат `BaseHitPointsOverride`.
4. Heritage override должен учитываться одним общим доменным методом. `AncestryDtoMapper` перестаёт самостоятельно повторять HP-логику и использует тот же результат.
5. В текущем каталоге override существует у `goblin.unbreakable` и заменяет ancestry HP `6` на `10`.
6. Если выбранные effects содержат несколько конфликтующих HP overrides, домен отклоняет неоднозначный catalog state. Молчаливый выбор первого значения запрещён. Нулевые и отрицательные overrides также отклоняются как некорректные catalog data.
7. Class catalog entry обязан соответствовать `SelectedClassId`, а ancestry entry — `AncestryType`. Несовпадение считается ошибкой сборки read-модели, а не основанием для приблизительного расчёта.
8. Constitution modifier берётся из текущего `AbilityScores.Constitution.Modifier`. Отдельная формула в application или frontend не создаётся.
9. Отрицательный Constitution modifier уменьшает maximum HP. Искусственный minimum в v1 не вводится без отдельного правила.
10. Для legacy draft без class package `DerivedStatistics` возвращается как `null`; список и карточка продолжают читаться.
11. Для draft с Class расчёт отражает текущее состояние характеристик, даже если запись была создана до появления final free boosts.

## Предлагаемая доменная модель

### Effective ancestry HP

`Ancestry` получает метод разрешения effective base HP по выбранным stable ids character package либо отдельный domain policy с тем же контрактом.

Метод:

- находит выбранные heritage и ancestry feat в переданном catalog entry;
- собирает их `BaseHitPointsOverride`;
- возвращает ancestry base HP при отсутствии override;
- возвращает единственное согласованное override;
- отклоняет конфликтующие значения.

Этот же метод использует `AncestryDtoMapper` для `EffectiveBaseHitPoints`.

### Hit Points

Доменный value object или calculator возвращает:

- `AncestryHitPoints`;
- `ClassHitPoints`;
- `ConstitutionModifier`;
- `MaximumHitPoints`.

Calculator принимает `DraftCharacter`, соответствующий `Ancestry` catalog entry и `CharacterClass` catalog entry. Клиентские DTO в домен не передаются.

## API-контракт

`CharacterDto` получает nullable блок:

```json
{
  "derivedStatistics": {
    "hitPoints": {
      "maximum": 21,
      "ancestry": 10,
      "class": 8,
      "constitutionModifier": 3
    }
  }
}
```

Контракт одинаков для:

- `GET /api/character`;
- `GET /api/character/{id}`.

Для legacy draft без полного class choice `derivedStatistics` равен `null`. Create endpoint не получает HP-полей.

## Persistence

Новые поля в `DraftCharacter` и БД не добавляются. Источниками расчёта остаются уже сохранённые ancestry/class choices и `AbilityScores`.

После реализации необходимо проверить отсутствие model changes командой EF. Новая migration будет признаком ошибочного дублирования derived state.

## Frontend flow

### Dashboard

Карточка персонажа показывает `Maximum HP`, если `derivedStatistics` присутствует. Для legacy draft без расчёта HP блок не отображается либо показывает локализованное нейтральное состояние.

### Character details

Карточка показывает:

- maximum HP как основное значение;
- ancestry contribution;
- class contribution;
- Constitution modifier со знаком.

Frontend не пересчитывает HP из других DTO и не пытается самостоятельно интерпретировать heritage effects.

## Этапы выполнения

1. Перепроверить формулу первого уровня, ancestry override, legacy boundary и отсутствие persistence; зафиксировать план.
2. Реализовать единое доменное разрешение effective ancestry HP и Hit Points calculator/value object; покрыть base, override, Constitution и конфликтующие overrides domain tests.
3. Расширить application DTO/converter, переиспользовать ancestry HP resolution и добавить query/integration tests для обычного, Unbreakable Goblin и legacy draft.
4. Подтвердить отсутствие EF model changes и migration; проверить list/details API contracts.
5. Расширить frontend types, dashboard и details view; добавить локализацию и frontend tests.
6. Обновить MemoryBank и выполнить проверки активного backend/frontend scope.
7. Провести отдельный code review, исправить замечания и повторить затронутые проверки.

После каждого этапа оставшийся план пересматривается. Current HP, дополнительные HP sources или level progression не добавляются в v1 неявно.

## Критерии готовности

- базовый ancestry HP используется при отсутствии override;
- Unbreakable Goblin использует ancestry HP `10` вместо `6`;
- class HP берётся из выбранного class catalog entry;
- Constitution modifier берётся из итогового persisted score;
- maximum HP равен сумме трёх составляющих;
- положительный, нулевой и отрицательный Constitution modifier покрыты тестами;
- конфликтующие ancestry HP overrides отклоняются;
- ancestry package и derived statistics используют одно правило effective ancestry HP;
- list и details возвращают одинаковый HP contract;
- legacy draft без Class остаётся читаемым с nullable derived statistics;
- изменение исходных данных отражается без записи отдельного HP state;
- create request не принимает HP;
- EF model не содержит новых persistence changes;
- dashboard показывает maximum HP, details — maximum и breakdown;
- backend tests, frontend tests, build и lint проходят;
- финальный code review не содержит незакрытых замечаний в scope Hit Points v1.

## План проверок

- `dotnet test CharacterManagement.Domain.Tests`;
- `dotnet test CharacterManagement.Infrastructure.Tests`;
- `dotnet build Pathfinder.Web --no-restore`;
- `dotnet ef migrations has-pending-model-changes` для `CharacterManagementDbContext`;
- frontend unit tests, lint и production build;
- `git diff --check`;
- полный `Pathfinder.sln` как диагностическая проверка с учётом известного legacy blocker в `Store.Application`.

## Статус выполнения

- план и граница задачи зафиксированы;
- этапы 1–7 завершены: domain calculator, application/read-модель, отсутствие EF changes, frontend, документация и финальный code review выполнены;
- domain tests: `53`, application/integration tests: `56`, frontend tests: `16`;
- EF подтвердил отсутствие model changes, новая migration не создавалась;
- `Pathfinder.Web` и frontend собираются, frontend lint проходит;
- на code review добавлена защита от неположительных ancestry HP overrides и отдельная проверка пересчёта HP после изменения Constitution; открытых замечаний в scope Hit Points v1 нет.

## Связанные документы

- [Near-term roadmap](character_creation_near_term_roadmap.md)
- [Final free boosts implementation](final_free_boosts_implementation.md)
- [Target full domain rules](../20_domain/character_creation/domain_rules_target_full.md)
- [Ancestry package design](../20_domain/character_creation/ancestry_package_design.md)
- [Class catalog](../20_domain/character_creation/class_catalog_player_core.md)
