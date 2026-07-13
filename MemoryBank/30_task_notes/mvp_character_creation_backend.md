# MVP Character Creation Backend

## Цель

Реализовать backend MVP создания персонажа Pathfinder 2e: выдача каталогов ancestry и Background, создание персонажа, получение списка/карточки, удаление, а также серверное применение ancestry и background boosts.

## Что готово

- `CharacterManagement` подключён к `Pathfinder.Web`.
- Есть `IAncestryRepository` с шестью базовыми ancestry.
- Есть `IBackgroundRepository` с baseline-каталогом из 35 Background.
- Реализованы use cases для ancestry, Background, создания, получения и удаления персонажей.
- Есть API endpoints для ancestry, Background и characters.
- Домен применяет ancestry fixed boosts, flaws, free boosts и отдельный обратимый пакет из двух background boosts через `DraftCharacter` / `CharacterBuilder`.
- Выбор Background и boosts сохраняется в БД и возвращается в character read-модели.
- Есть unit и integration tests по ключевым backend-сценариям.

## Что не готово

- `Class` catalog и key ability boost ещё не реализованы.
- Четыре финальных свободных boosts ещё не входят в character creation.
- Background grants для skills, Lore и skill feats пока представлены декларативно и не применяются к персонажу.
- Текущий MVP не покрывает equipment, spells, deity и полноценные derived statistics.
- Есть отдельный актуальный блок по расхождению `Secure.User` и `CharacterManagement.Account`: см. [`task_32_account_backfill.md`](task_32_account_backfill.md).

## Связанные файлы

- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md)
- [`../20_domain/character_creation/aon_player_core_ancestries_59_64.md`](../20_domain/character_creation/aon_player_core_ancestries_59_64.md)
- [`background_package_implementation.md`](background_package_implementation.md)
- `../../CharacterManagement.Domain/Entity/DraftCharacter.cs`
- `../../CharacterManagement.Application/Builders/Implementation/CharacterBuilder.cs`
- `../../CharacterManagement.Infrastructure/Repositories/AncestryRepository.cs`
- `../../CharacterManagement.Infrastructure/Repositories/BackgroundRepository.cs`
- `../../CharacterManagement.Infrastructure.Tests/`
- `../../CharacterManagement.Domain.Tests/`

## Next steps

1. Отдельным scope нормализовать `Class` catalog для восьми классов `Player Core` и реализовать key ability boost.
2. После `Class` отдельным пакетом добавить четыре финальных свободных boosts.
3. Не применять skills, Lore, skill feats, spells, deity и equipment без соответствующих каталогов и доменных решений.
4. При изменении character creation сверяться с `../20_domain/character_creation/domain_rules_target_full.md` и актуальными catalog documents.

