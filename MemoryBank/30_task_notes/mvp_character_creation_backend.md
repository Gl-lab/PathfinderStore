# MVP Character Creation Backend

## Цель

Реализовать backend MVP создания персонажа Pathfinder 2e: выдача каталогов Ancestry, Background и Class, создание персонажа, получение списка/карточки, удаление, а также серверное применение ancestry, background, class и четырёх финальных boosts.

## Что готово

- `CharacterManagement` подключён к `Pathfinder.Web`.
- Есть `IAncestryRepository` с шестью базовыми ancestry.
- Есть `IBackgroundRepository` с baseline-каталогом из 35 Background.
- Есть `ICharacterClassRepository` с восемью классами `Player Core`.
- Реализованы use cases для Ancestry, Background, Class, создания, получения и удаления персонажей.
- Есть API endpoints для Ancestry, Background, Class и characters.
- Домен применяет ancestry fixed boosts/flaws/free boosts, два background boosts, отдельный class key ability boost и отдельный обратимый пакет четырёх final free boosts.
- Выборы Background, Class и final free boosts сохраняются в БД и возвращаются в character read-модели.
- Maximum HP первого уровня вычисляется на чтении из effective ancestry HP, class HP и Constitution modifier и не дублируется в БД.
- Есть типизированный каталог 16 general skills и endpoint `GET /api/skills`.
- Background fixed/finite/open grants фактически применяют trained skill и Lore; training сохраняется и возвращается read-моделью.
- Class catalog содержит typed baseline для Perception, saves, attacks, defenses и class DC; grants вычисляются из сохранённого class id и возвращаются read-моделью.
- Есть unit и integration tests по ключевым backend-сценариям.

## Что не готово

- Background skill feat пока представлен декларативно и не применяется к персонажу.
- Class skills, higher proficiency ranks и duplicate replacement между разными sources пока не применяются.
- Class skills, spell proficiencies, features, spells и mandatory choices представлены декларативно и не применяются к персонажу.
- Текущий MVP не покрывает equipment, spells, deity, current/temporary HP и derived statistics шире maximum HP.
- Есть отдельный актуальный блок по расхождению `Secure.User` и `CharacterManagement.Account`: см. [`task_32_account_backfill.md`](task_32_account_backfill.md).

## Связанные файлы

- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md)
- [`../20_domain/character_creation/aon_player_core_ancestries_59_64.md`](../20_domain/character_creation/aon_player_core_ancestries_59_64.md)
- [`background_package_implementation.md`](background_package_implementation.md)
- [`class_package_implementation.md`](class_package_implementation.md)
- `../../CharacterManagement.Domain/Entity/DraftCharacter.cs`
- `../../CharacterManagement.Application/Builders/Implementation/CharacterBuilder.cs`
- `../../CharacterManagement.Infrastructure/Repositories/AncestryRepository.cs`
- `../../CharacterManagement.Infrastructure/Repositories/BackgroundRepository.cs`
- `../../CharacterManagement.Infrastructure/Repositories/CharacterClassRepository.cs`
- `../../CharacterManagement.Infrastructure.Tests/`
- `../../CharacterManagement.Domain.Tests/`

## Next steps

1. Продолжать обязательные class choices отдельными flows; Rogue's Racket реализован.
2. Добавить cross-source proficiency resolver вместе с первым дополнительным источником grants или level-up progression.
3. Не применять feats, class features, spells, deity и equipment без соответствующих каталогов и доменных решений.
4. При изменении character creation сверяться с `../20_domain/character_creation/domain_rules_target_full.md` и актуальными catalog documents.

