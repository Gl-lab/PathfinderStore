# Player Core Feat Catalog — Priority 5.1

## Проблема

Ancestry feats, background skill feats и class feats использовали разные формы идентификаторов и descriptors. Единого server-owned каталога с category, level, traits, prerequisites и явной границей отложенных механических эффектов не было, поэтому следующие slices не могли одинаково проверять выбранные и granted feats.

## Ожидаемый результат

Backend предоставляет единый read-only каталог feats первого уровня для текущего character creation baseline. Существующие ancestry и background ссылки разрешаются в этом каталоге, все восемь поддерживаемых классов имеют полный набор Player Core class feats первого уровня, а не реализованные rule effects представлены типизированными зависимостями.

## План реализации

1. Ввести единое определение `FeatDefinition` со stable id, category, level, traits, rarity, prerequisites, source и typed deferred dependencies.
2. Агрегировать существующие ancestry feat и background skill feat источники без копирования их identity.
3. Добавить class feat definitions первого уровня для Bard, Cleric, Druid, Fighter, Ranger, Rogue, Witch и Wizard.
4. Добавить resolver common options и `GET /api/feats` с фильтрами category, level и trait.
5. Зафиксировать целостность каталога domain/infrastructure tests и провести review до отдельного коммита.

## Реализованная модель

- `FeatCategory` различает `Ancestry`, `Skill` и `Class`.
- `FeatDefinition` хранит `Id`, `Name`, `Category`, `Level`, `Traits`, `Rarity`, `Prerequisites`, `Summary`, `DeferredDependencies` и `Source`.
- Ancestry definitions проецируются из `AncestryRepository`; существующие ids вида `human.cooperative_nature` остаются стабильными.
- Background skill feats проецируются из `BackgroundRepository`, включая оба варианта Martial Disciple; повторяющиеся grants дедуплицируются по stable id.
- Class catalog содержит 47 уникальных definitions и воспроизводит нормативное число вариантов каждого из восьми Player Core классов первого уровня. Общие feats (`Animal Companion`, `Counterspell`, `Reach Spell`, `Widen Spell`) представлены одной записью с несколькими class traits.
- Полный baseline содержит 118 уникальных definitions: 40 ancestry, 31 background skill и 47 class feats.
- `FeatCatalogResolver` возвращает только common feats заданных category, level и trait, отсортированные по имени.
- `GET /api/feats?category=...&level=1&requiredTrait=...` предоставляет каталог будущим feat flows.

## Границы задачи

В задачу не входят:

- сохранение общего feat inventory персонажа;
- автоматическое подключение background grants и ancestry choices к карточке;
- выбор class feats в wizard;
- исполнение actions, combat, spells, equipment, familiar или companion effects;
- general feat catalog, higher-level feats и источники вне текущего baseline.

## Проверки

- `CharacterManagement.Domain.Tests`: фильтрация category/level/trait, common rarity и deterministic ordering.
- `CharacterManagement.Infrastructure.Tests`: uniqueness, полнота существующих ancestry/background ссылок, варианты всех восьми классов, prerequisites и typed dependencies.
- `Pathfinder.Web`: controller, MediatR handler, DTO mapping и DI registration компилируются в полном web graph.
- `dotnet test CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj --no-restore` — пройдено.
- `dotnet test CharacterManagement.Infrastructure.Tests/CharacterManagement.Infrastructure.Tests.csproj --no-restore` — пройдено, 242 tests.
- `dotnet build Pathfinder.Web/Pathfinder.Web.csproj --no-restore` — пройдено, 0 errors.

## Результат review

Проверены полнота class trait counts относительно research-каталога, разрешимость всех существующих ancestry/background feat ids, уникальность 118 definitions, common-only boundary общего resolver и отсутствие persistence/EF изменений. Shared class feats не дублируются, а class-specific prerequisites сохраняют явный контекст в описании. Открытых замечаний по первой задаче Priority 5 не осталось.
