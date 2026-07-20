# Player Core Spell Catalog — Priority 4.1

## Проблема

Cleric spell flow использовал единый `SpellRepository`, но фактическое наполнение каталога было ограничено divine options, deity-granted rank-1 spells и Cleric focus spells. Bard, Druid, Witch и Wizard нельзя было строить поверх общего server-owned источника: часть common occult, primal и arcane spells отсутствовала, а общего способа получить допустимые options по tradition не было.

## Ожидаемый результат

Backend содержит единый tradition-aware каталог всех common Player Core cantrips и spells первого ранга для Arcane, Divine, Occult и Primal traditions. Клиент получает selectable options через общий API, а uncommon и focus spells доступны только через отдельное правило источника.

## План реализации

1. Дополнить существующий `SpellRepository` недостающими Player Core cantrips и spells первого ранга, сохранив stable ids и полные metadata.
2. Добавить domain resolver common options по `SpellTradition`, rank и `SpellKind`.
3. Добавить общий application query и `GET /api/spells` без изменения совместимого Cleric API.
4. Зафиксировать полноту каталога и access boundary тестами.
5. Провести review diff, targeted backend tests и сборку web-проекта до коммита.

## Реализованная модель

- `SpellDefinition` остаётся единым server-owned определением spell identity, rank, kind, traditions, traits, rarity и source.
- Каталог содержит 122 определения первого ранга:
  - 25 common cantrips;
  - 57 common spells;
  - 1 uncommon spell `Detect Poison`;
  - 39 ранее реализованных Cleric focus spells.
- `SpellCatalogResolver.ResolveCommonOptions` возвращает только common spells указанной tradition, rank и kind.
- `SpellKind.Focus` намеренно запрещён в общем resolver: focus spell требует class feature, Order, Patron, School или другого явного источника.
- `GET /api/spells?tradition=...&rank=1&kind=...` предоставляет общий каталог будущим class flows.
- Существующие `GET /api/classes/cleric/spells` и deity-aware available options сохранены без изменения контракта.

## Нормативные количества common options первого уровня

| Tradition | Cantrips | Rank-1 spells |
|---|---:|---:|
| Arcane | 19 | 42 |
| Divine | 16 | 23 |
| Occult | 16 | 33 |
| Primal | 15 | 31 |

Пересечения traditions являются нормальными: одно определение spell участвует в нескольких списках без копирования.

## Границы задачи

В задачу не входят:

- repertoire, preparation, spellbook и persistence class loadout;
- автоматический доступ к uncommon spells;
- focus pool и class-granted focus spell packages;
- casting, расходование slots, rest lifecycle и spell effects;
- ranks выше первого и другие книги.

## Проверки

- `CharacterManagement.Domain.Tests`: resolver фильтрует rarity/tradition/rank/kind, сортирует options и запрещает общий focus enumeration.
- `CharacterManagement.Infrastructure.Tests`: каталог уникален, содержит ожидаемые totals и точные counts четырёх traditions; Cleric access tests остаются зелёными.
- `Pathfinder.Web`: общий endpoint и MediatR handler компилируются в полном web graph.
- `dotnet test CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj --no-restore` — пройдено, 180 tests.
- `dotnet test CharacterManagement.Infrastructure.Tests/CharacterManagement.Infrastructure.Tests.csproj --no-restore` — пройдено, 217 tests.
- `dotnet build Pathfinder.Web/Pathfinder.Web.csproj --no-restore` — пройдено, 0 errors.

## Результат review

Общий selectable resolver не смешивает common tradition access с source-bound uncommon/focus access. Cleric получает прежние 16 divine cantrips и 23 common divine rank-1 spells плюс deity grants, а расширение repository не меняет сохранённый loadout или EF schema. Открытых замечаний по первой задаче Priority 4 не осталось.
