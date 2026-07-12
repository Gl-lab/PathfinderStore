# Character Creation Domain

## Назначение

Этот набор документов фиксирует доменные правила создания персонажа для проекта Pathfinder 2e Web App.

Документы используются как нормативная основа для:

- проектирования доменной модели `CharacterManagement`;
- реализации API и frontend-визарда;
- написания unit и integration tests;
- дальнейшего расширения MVP до полного процесса создания персонажа.

Источник правил: материалы из `../../90_research/tempinfo/`, проверенные AoN-конспекты и адаптация под текущую архитектуру и состояние кода проекта.

## Как читать

- Если задача касается текущего backend/frontend MVP, начинать с [domain_rules_mvp.md](domain_rules_mvp.md).
- Если задача проектирует следующий этап character creation, читать [domain_rules_target_full.md](domain_rules_target_full.md), затем нужные каталоги.
- Если нужно понять, чего не хватает в данных, читать [known_gaps.md](known_gaps.md).
- Если нужно понять расхождения между правилами и текущим кодом, читать [implementation_notes.md](implementation_notes.md).

## Normative Rules

- [domain_rules_mvp.md](domain_rules_mvp.md) — обязательные правила для текущего MVP.
- [domain_rules_target_full.md](domain_rules_target_full.md) — целевая полная модель процесса создания персонажа по мотивам PF2e.

Принцип:

- если правило описано в `domain_rules_mvp.md`, оно обязательно для текущей разработки;
- если правило есть только в `domain_rules_target_full.md`, оно пока не обязательно для MVP, но должно учитываться при проектировании расширяемой модели;
- при конфликте между текущим кодом и доменными правилами конфликт должен быть зафиксирован в [implementation_notes.md](implementation_notes.md).

## Catalogs

- [aon_player_core_ancestries_59_64.md](aon_player_core_ancestries_59_64.md) — проверенный remastered-источник по 6 базовым ancestry из Archives of Nethys (`Player Core`).
- [ancestry_choices_player_core.md](ancestry_choices_player_core.md) — нормализованный каталог heritage и ancestry feats 1 уровня для этих ancestry, включая зависимости эффектов от будущих подсистем.
- [ancestry_package_design.md](ancestry_package_design.md) — утверждённое проектирование catalog, aggregate, persistence и API для полного ancestry-пакета задачи #43.
- [aon_player_core_character_creation_sources.md](aon_player_core_character_creation_sources.md) — `Player Core`-источник по классам, background rules, archetype boundary и ancestry-derived стартовым предметам.
- [background_catalog_core_rulebook.md](background_catalog_core_rulebook.md) — baseline-каталог backgrounds из Core Rulebook.
- [skill_catalog_player_core.md](skill_catalog_player_core.md) — общий каталог skills Player Core.
- [catalog_inventory_status.md](catalog_inventory_status.md) — карта готовности каталогов.

## Gaps And Implementation

- [known_gaps.md](known_gaps.md) — отсутствующие или частично готовые доменные данные и каталоги.
- [implementation_notes.md](implementation_notes.md) — расхождения текущего кода с доменными документами и remastered-источниками.

## Базовый словарь проекта

- `Character` / `DraftCharacter` — создаваемый персонаж пользователя.
- `AbilityScores` — шесть базовых значений характеристик: `Strength`, `Dexterity`, `Constitution`, `Intelligence`, `Wisdom`, `Charisma`.
- `Characteristic.Modifier` — вычисляемый модификатор характеристики по формуле `(Value - 10) / 2`.
- `Ancestry` — игровая сущность PF2e, которая в текущем проекте определяет стартовые повышения, понижения, размер, скорость, базовые HP и зрение.
- `Background` — источник стартовых boosts и дополнительных эффектов, пока не входит в MVP.
- `Class` — источник key ability boost, class HP, proficiencies и class features, пока не входит в MVP.
- `Core Rulebook` / `Player Core` — в AoN-данных базовый `Player Core`-слой часто индексируется как `Core Rulebook`; в документации проекта эти два ярлыка считаются одним baseline-источником, если явно не указано иное.

## Нормализация терминов

В каноническом тексте PF2e используется термин `народ`. В проекте следует использовать термин `Ancestry`.

Отдельно:
- `RaceId` в текущей модели является техническим наследием существующего кода
- на уровне доменных правил первичной сущностью выбора народа является именно `Ancestry`
- дальнейшая разработка должна стремиться к тому, чтобы `Race` не дублировал и не подменял `Ancestry`

## Нормализация характеристик

В тексте источника стартовые значения описаны как модификаторы `+0`, а повышения и понижения изменяют модификатор на `+1` и `-1`.

В проекте каноническим представлением должно быть:
- базовое значение каждой характеристики на старте равно `10`
- повышение характеристики изменяет значение на `+2`
- понижение характеристики изменяет значение на `-2`
- модификатор всегда вычисляется из итогового значения, а не хранится отдельно

Это правило обязательно для всех уровней модели, API и тестов.
