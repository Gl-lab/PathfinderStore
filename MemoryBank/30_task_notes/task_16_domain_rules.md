# Task 16 Status

## Vikunja

- Task: `#16`
- Title: `[Domain] Сформировать правила домена на основе Pathfinder 2e Core Rulebook`
- Checked at: `2026-07-09`

## Что уже сделано

- Сформирован набор нормативных документов в `MemoryBank/20_domain/character_creation/`.
- Зафиксированы MVP-правила создания персонажа: стартовые `AbilityScores`, boosts/flaws, free boosts, выбор `Ancestry`, API-инварианты и тестовые сценарии.
- Описана целевая полная модель создания персонажа: `Ancestry`, `Background`, `Class`, финальные boosts, derived statistics и future boundaries.
- Собраны данные по 6 базовым ancestry: `Dwarf`, `Elf`, `Gnome`, `Goblin`, `Halfling`, `Human`.
- Собран baseline-каталог backgrounds из Core Rulebook.
- Зафиксированы базовые классы Player Core и их обязательные стартовые choices.
- Собран общий skill catalog Player Core.
- В коде есть текущий ancestry-каталог и доменная логика ability boosts/flaws/free boosts.

## Где смотреть

- [`../20_domain/character_creation/README.md`](../20_domain/character_creation/README.md)
- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md)
- [`../20_domain/character_creation/domain_rules_target_full.md`](../20_domain/character_creation/domain_rules_target_full.md)
- [`../20_domain/character_creation/aon_player_core_ancestries_59_64.md`](../20_domain/character_creation/aon_player_core_ancestries_59_64.md)
- [`../20_domain/character_creation/aon_player_core_character_creation_sources.md`](../20_domain/character_creation/aon_player_core_character_creation_sources.md)
- [`../20_domain/character_creation/background_catalog_core_rulebook.md`](../20_domain/character_creation/background_catalog_core_rulebook.md)
- [`../20_domain/character_creation/skill_catalog_player_core.md`](../20_domain/character_creation/skill_catalog_player_core.md)

## Что не является частью закрытия #16

- Реализация полноценного C#-каталога `Background`.
- Реализация полноценного C#-каталога `Class`.
- Миграция текущего ancestry-каталога на все remastered-поля: `SourceBook`, `SourcePage`, `GrantedRules`, `GrantedItems`, `Languages`.
- Исправление расхождений между legacy-like MVP vision flags и remastered ancestry facts.

Эти пункты лучше вести отдельными задачами, потому что они уже относятся к реализации и расширению модели, а не к исследованию правил.

## Вывод

Исследовательская часть задачи `#16` фактически выполнена. Карточку можно закрывать как документационную, если принять, что C#-каталоги `Background` и `Class` будут отдельными задачами.
