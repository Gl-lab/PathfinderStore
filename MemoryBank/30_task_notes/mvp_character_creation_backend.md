# MVP Character Creation Backend

## Цель

Реализовать backend MVP создания персонажа Pathfinder 2e: выдача каталогов Ancestry, Background и Class, создание персонажа, получение списка/карточки, удаление, а также серверное применение ancestry, background и class boosts.

## Что готово

- `CharacterManagement` подключён к `Pathfinder.Web`.
- Есть `IAncestryRepository` с шестью базовыми ancestry.
- Есть `IBackgroundRepository` с baseline-каталогом из 35 Background.
- Есть `ICharacterClassRepository` с восемью классами `Player Core`.
- Реализованы use cases для Ancestry, Background, Class, создания, получения и удаления персонажей.
- Есть API endpoints для Ancestry, Background, Class и characters.
- Домен применяет ancestry fixed boosts/flaws/free boosts, два background boosts и отдельный обратимый class key ability boost.
- Выборы Background и Class сохраняются в БД и возвращаются в character read-модели.
- Есть unit и integration tests по ключевым backend-сценариям.

## Что не готово

- Четыре финальных свободных boosts ещё не входят в character creation.
- Background grants для skills, Lore и skill feats пока представлены декларативно и не применяются к персонажу.
- Class proficiencies, features, spells и mandatory choices представлены декларативно и не применяются к персонажу.
- Текущий MVP не покрывает equipment, spells, deity и полноценные derived statistics.
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

1. Отдельным пакетом добавить четыре финальных свободных boosts.
2. Не применять skills, Lore, feats, class features, spells, deity и equipment без соответствующих каталогов и доменных решений.
3. При изменении character creation сверяться с `../20_domain/character_creation/domain_rules_target_full.md` и актуальными catalog documents.

