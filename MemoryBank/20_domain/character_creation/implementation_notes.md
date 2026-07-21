# Character Creation Implementation Notes

## Назначение

Этот документ фиксирует расхождения между доменными правилами character creation и текущей реализацией проекта.

Он не является нормативным источником правил. Нормативные правила лежат в [domain_rules_mvp.md](domain_rules_mvp.md) и [domain_rules_target_full.md](domain_rules_target_full.md).

## Текущая граница MVP

Текущий backend MVP поддерживает:

- создание персонажа с именем;
- выбор `Ancestry`;
- применение ancestry boosts/flaws;
- обязательные ancestry free boosts;
- чтение списка и карточки персонажа;
- удаление персонажа.

Вне текущего MVP:

- `Background`;
- `Class`;
- class features;
- skills;
- spells;
- deity;
- equipment;
- languages;
- ancestry feats;
- narrative fields.

## Ancestry Model

Текущая C#-модель `Ancestry` покрывает MVP-поля:

- `AncestryType`;
- `AbilityBoosts`;
- `AbilityFlaws`;
- `BaseHitPoints`;
- `Size`;
- `BaseSpeed`;
- `Darkvision`;
- `LowLightVision`.

Remastered-документы требуют более широкую модель:

- `SourceBook`;
- `SourcePage`;
- `Vision`;
- `GrantedRules`;
- `GrantedItems`;
- `Languages`;
- `AdditionalLanguageRule`.

Практический вывод: текущая модель допустима для MVP, но при расширении `Ancestry` её нужно мигрировать, а не продолжать добавлять отдельные bool-флаги и исключения.

## Vision

Текущий код хранит vision через два bool-флага:

- `Darkvision`;
- `LowLightVision`.

Для remastered baseline устойчивее единое значение:

- `VisionType.None`;
- `VisionType.LowLight`;
- `VisionType.Darkvision`.

Причины:

- меньше риск противоречивых комбинаций;
- `Halfling` в remastered baseline имеет `Keen Eyes`, а не обычный low-light vision;
- sensory-like ancestry features лучше хранить отдельно от базового vision.

## Known Data Differences

Файл `CharacterManagement.Infrastructure/Repositories/AncestryRepository.cs` частично не совпадает с [aon_player_core_ancestries_59_64.md](aon_player_core_ancestries_59_64.md):

- `Dwarf` в текущем MVP-каталоге не имеет `Darkvision`, а remastered baseline имеет.
- `Halfling` в текущем MVP-каталоге имеет `LowLightVision`, а remastered baseline фиксирует `Keen Eyes` без low-light vision.
- `Dwarf` remastered baseline даёт `Clan Dagger`, но модель не хранит granted items.
- Все шесть ancestry имеют starting languages и additional language rules, но модель их не хранит.

Это нужно считать зафиксированным gap, а не случайной ошибкой отдельного документа.

## Race Vs Ancestry

В проекте ещё встречается наследие `Race`, например в именах технических полей или старых местах модели.

Правило документации и новых задач:

- в доменных правилах использовать `Ancestry`;
- не вводить новые публичные контракты с термином `Race`;
- при изменении существующих контрактов постепенно вытеснять `Race`, если это не ломает текущий scope.

## Background And Class

Активный create pipeline `CharacterBuilder` использует catalog-backed `SetBackground`, `SetClass` и `SetStartingEquipment`. Starting equipment больше не является placeholder: стоимость, equipped state, proficiency applicability и Bulk разрешаются сервером.

В builder остаются legacy-looking placeholders `SetAlignment`, `SetDeity` и `SetAge`. Они не считаются незавершённой реализацией текущего MVP: Deity и age уже входят в специализированные package/create methods, а alignment отсутствует в remastered baseline текущего scope. Расширять эти области следует отдельными catalog/rules flows, а не свободными полями.

## Legacy Item Endpoints

Пустые legacy endpoints `GET /api/character/items` и `DELETE /api/character/items/drop` удалены до появления типизированного equipment API. Ownership catalog, starting rules и draft loadout зафиксирована за `CharacterManagement` в [equipment_inventory_boundary.md](equipment_inventory_boundary.md); runtime shop/trade inventory остаётся будущей отдельной границей.

## Что проверить перед задачами по расширению

Перед задачами по `Ancestry`, `Background`, `Class`, languages, equipment или full sheet нужно сверить:

1. Нормативное правило в [domain_rules_mvp.md](domain_rules_mvp.md) или [domain_rules_target_full.md](domain_rules_target_full.md).
2. Наличие каталога в [known_gaps.md](known_gaps.md).
3. Текущую карту `CharacterManagement` в [../../00_project/project_pathfinder_character_domain.md](../../00_project/project_pathfinder_character_domain.md).
4. Источник данных в [../../90_research/](../../90_research/), если доменный каталог ещё не promoted в `20_domain`.
