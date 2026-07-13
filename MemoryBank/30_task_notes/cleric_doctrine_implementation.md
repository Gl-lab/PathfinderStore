# Cleric Doctrine и Deity Boundary — Implementation Plan

## Проблема

`Cleric` уже присутствует в class catalog, но обязательный выбор Doctrine остаётся декларативным. Поэтому стартовые proficiency Cleric всегда возвращаются как общий baseline: невозможно применить защитную подготовку Warpriest или сохранить выбранный Doctrine.

Одновременно Cleric зависит от Deity. Подменять отсутствующий deity catalog свободной строкой нельзя: deity определяет divine skill, favored weapon, Divine Font, sanctification, domains и granted spells. Эта задача должна провести явную границу между Doctrine flow, который можно реализовать на существующих подсистемах, и будущим полноценным Deity flow.

## Источник и baseline

Baseline — `Player Core`, страница 112:

- [Cleric](https://2e.aonprd.com/Classes.aspx?ID=33);
- [Cloistered Cleric](https://2e.aonprd.com/Doctrines.aspx?ID=4&Redirected=1);
- [Warpriest](https://2e.aonprd.com/Doctrines.aspx?ID=5&Redirected=1);
- [Player Core deity list](https://2e.aonprd.com/Sources.aspx?ID=216).

На 1-м уровне:

| Doctrine | Stable id | Типизированные effects | Deferred effects |
|---|---|---|---|
| Cloistered Cleric | `cleric_doctrine.cloistered` | нет дополнительных proficiency grants | Domain Initiate и выбор domain |
| Warpriest | `cleric_doctrine.warpriest` | Fortitude `Expert`, Light Armor `Trained`, Medium Armor `Trained` | Shield Block; условный Deadly Simplicity |

Последующие doctrine benefits на уровнях 3, 7, 11, 15 и 19 в эту задачу не входят.

## Ожидаемый результат

При создании Cleric пользователь обязан выбрать Cloistered Cleric или Warpriest. Выбор сохраняется, возвращается read-моделью и отображается во frontend. Effective proficiencies вычисляются из class baseline и doctrine grants через существующий multi-source resolver.

Deity остаётся явной незакрытой зависимостью. В request не добавляется свободный `deityId`, а приложение не имитирует divine skill, favored weapon, font, sanctification, domains или spells без нормативного каталога.

## Граница задачи

### Входит

- нормализация двух Doctrine из `Player Core`;
- stable ids, source и typed descriptors;
- обязательный Doctrine только для `class.cleric`;
- запрет Doctrine для остальных классов;
- Warpriest grants: Fortitude `Expert`, Light Armor `Trained`, Medium Armor `Trained`;
- декларативные descriptors для Domain Initiate, Shield Block и Deadly Simplicity;
- объединение class + doctrine proficiencies;
- обратимая замена class/doctrine package без повреждения Background, Rogue training и final boosts;
- persistence nullable doctrine id для совместимости с legacy rows;
- catalog endpoint, create/read contracts и validation;
- frontend wizard, review и character details;
- EF migration, tests, документация и финальный code review.

### Не входит

- deity catalog и выбор Deity;
- divine skill и duplicate replacement от Deity;
- favored weapon catalog, access и proficiency;
- Divine Font и выбор `heal`/`harm`;
- sanctification (`holy`/`unholy`);
- domains, Domain Initiate choice и focus spells;
- granted deity spells и Cleric spell preparation;
- фактическая выдача Shield Block и Deadly Simplicity;
- doctrine progression после 1-го уровня;
- универсальный framework всех class choices.

## Доменные решения

### Doctrine catalog

Добавить `ClericDoctrine` со следующими данными:

- `Id`, `Name`, `Source`;
- `ProficiencyGrants`;
- `Effects` с признаком typed/deferred boundary;
- `DeferredDependencies`.

Каталог живёт вне `DraftCharacter` и содержит ровно две записи `Player Core`.

Stable source ids:

- `cleric_doctrine.warpriest.proficiency.fortitude`;
- `cleric_doctrine.warpriest.proficiency.light_armor`;
- `cleric_doctrine.warpriest.proficiency.medium_armor`;
- `cleric_doctrine.cloistered.effect.domain_initiate`;
- `cleric_doctrine.warpriest.effect.shield_block`;
- `cleric_doctrine.warpriest.effect.deadly_simplicity`.

### Class package

`DraftCharacter` получает nullable `SelectedClericDoctrineId`.

Инварианты:

- `class.cleric` требует один Doctrine из каталога;
- non-Cleric запрещает Doctrine;
- смена Cleric Doctrine заменяет только doctrine selection/effects;
- смена Cleric на другой class очищает Doctrine;
- invalid request отклоняется до изменения агрегата;
- legacy Cleric без Doctrine остаётся читаемым, но новые create requests без Doctrine отклоняются.

### Proficiency resolution

Новый resolver не нужен. Существующий `ProficiencyResolver` получает class baseline и doctrine grants:

- grouping по `Target.Id`;
- effective rank — максимальный;
- source ids сохраняются в диагностическом результате;
- Cloistered Cleric использует baseline без дополнительных grants;
- Warpriest повышает Fortitude с `Trained` до `Expert` и добавляет Light/Medium Armor `Trained`.

### Deity boundary

На этом этапе:

- class catalog продолжает возвращать `DeityCatalog` как deferred dependency;
- Doctrine descriptors могут ссылаться на `DeityCatalog`, `DomainCatalog`, `FeatCatalog` и `WeaponCatalog`, но не содержат строковых заглушек;
- `SelectedDeityId` не добавляется;
- Cleric package в read-модели явно показывает unresolved deity dependency;
- следующий Deity flow начинается с отдельной нормализации каталога и не расширяет эту задачу.

Если текущего набора `CharacterClassDependencyType` недостаточно, добавить только точные декларативные типы зависимостей, не создавая фиктивные каталоги.

## Application и API

### Catalog

Добавить endpoint:

```text
GET /api/classes/cleric/doctrines
```

DTO содержит id, name, source, proficiency grants, effects и deferred dependencies.

### Create character

Добавить nullable поле:

```json
{
  "classId": "class.cleric",
  "classKeyAbility": "Wisdom",
  "clericDoctrineId": "cleric_doctrine.warpriest"
}
```

Правила validation:

- для Cleric `clericDoctrineId` обязателен;
- для non-Cleric поле запрещено;
- неизвестный id и id не из Cleric catalog отклоняются доменом;
- `deityId` в контракт не добавляется.

### Read-модель

`CharacterClassPackageDto` получает nullable Doctrine package. Effective `CharacterDto.Proficiencies` строятся из class + Rogue Racket + Cleric Doctrine sources согласно выбранному классу.

Legacy Cleric без Doctrine возвращает class baseline и nullable Doctrine package без ошибки чтения.

## Persistence

- добавить nullable `SelectedClericDoctrineId` с длиной, согласованной с другими stable ids;
- создать миграцию только через `dotnet ef`;
- не сохранять proficiency snapshot: doctrine id однозначно задаёт grants;
- не делать backfill legacy rows;
- добавить round-trip и legacy-read tests.

## Frontend

- загружать Doctrine catalog вместе с остальными creation catalogs;
- показывать selector только для Cleric;
- сбрасывать Doctrine при смене класса;
- отображать typed proficiency effects и deferred features выбранного Doctrine;
- не показывать поле Deity до появления deity catalog;
- отправлять `clericDoctrineId` только в рамках общего nullable API contract;
- показывать Doctrine в review и character details;
- добавить helper/unit tests для conditional validation и effective display.

## Этапы выполнения

1. Нормализовать Doctrine data, stable ids, typed grants и deity/deferred boundary в отдельном нормативном документе.
2. Добавить domain model, doctrine catalog и catalog tests.
3. Проверить/расширить multi-source proficiency resolution на Warpriest matrix.
4. Расширить aggregate и builder; обеспечить обязательность, запрет чужого выбора и обратимую замену package.
5. Добавить application validation, catalog endpoint и create/read DTO.
6. Добавить EF mapping, CLI migration, round-trip и legacy tests.
7. Обновить frontend wizard, review, details, локализацию и tests.
8. Обновить MemoryBank и выполнить backend/frontend checks.
9. Провести отдельный code review, исправить замечания и повторить проверки.

После каждого этапа оставшийся план пересматривается. Расширение в Deity, domains, feats, weapons или spells оформляется отдельной задачей.

## Критерии готовности

- каталог возвращает ровно Cloistered Cleric и Warpriest из `Player Core`;
- Cleric невозможно создать без Doctrine;
- non-Cleric невозможно создать с Doctrine;
- Cloistered Cleric не получает выдуманных typed grants;
- Warpriest имеет effective Fortitude `Expert`, Light Armor `Trained`, Medium Armor `Trained`;
- Domain Initiate, Shield Block и Deadly Simplicity остаются декларативными;
- deity-dependent effects не применяются без deity catalog;
- смена class/doctrine удаляет только старые doctrine effects;
- legacy Cleric без Doctrine читается;
- EF round-trip, backend tests, frontend tests, lint и production build проходят;
- финальный code review не содержит открытых замечаний.

## Следующая отдельная задача

`Player Core Deity Catalog + Cleric Deity Choice`:

- сначала определить точный baseline допустимых faith/deity записей;
- типизировать divine skill, favored weapon reference, font, sanctification, domains и granted spells;
- только затем сделать Deity обязательным выбором Cleric и применить эффекты, поддержанные готовыми подсистемами.

## Статус

- нормативные источники и архитектурная граница проверены;
- implementation plan зафиксирован;
- этап 1 завершён: normative Doctrine matrix, stable ids и deity/deferred boundary зафиксированы в `MemoryBank/20_domain/character_creation/cleric_doctrines_player_core.md`;
- этап 2 завершён: добавлены domain model, catalog, точные deferred dependency types и catalog tests;
- этап 3 завершён: общий resolver проверен на Cloistered/Warpriest matrix, новый proficiency mechanism не требуется;
- этапы 4–7 завершены: aggregate/builder, validation, API/read model, nullable EF persistence и frontend flow реализованы;
- миграция `AddClericDoctrine` создана через `dotnet ef`, без backfill;
- этап 8 завершён: MemoryBank обновлён, полные backend/frontend checks прошли;
- этап 9 завершён: code review исправил потерю source ids во frontend effective proficiency preview; открытых замечаний нет;
- после этапов 1–7 план пересмотрен: добавлен явный legacy Cleric regression test, scope остаётся без изменений.
