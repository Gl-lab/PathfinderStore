# MVP Character Creation Backend

## Цель

Реализовать backend MVP создания персонажа Pathfinder 2e: выдача ancestry-каталога, создание персонажа, получение списка/карточки, удаление, применение ancestry boosts/flaws/free boosts на сервере.

## Что готово

- `CharacterManagement` подключён к `Pathfinder.Web`.
- Есть `IAncestryRepository` с шестью базовыми ancestry.
- Реализованы use cases для ancestry, создания, получения и удаления персонажей.
- Есть API endpoints для ancestry и characters.
- Домен применяет fixed boosts, flaws и free boosts через `DraftCharacter` / `CharacterBuilder`.
- Есть unit и integration tests по ключевым backend-сценариям.

## Что не готово

- Frontend wizard остаётся отдельной открытой задачей.
- Полные `Background` и `Class` каталоги в C# ещё не реализованы.
- Текущий MVP не покрывает skills, equipment, spells, deity и полноценные derived statistics.
- Есть отдельный актуальный блок по расхождению `Secure.User` и `CharacterManagement.Account`: см. [`task_32_account_backfill.md`](task_32_account_backfill.md).

## Связанные файлы

- [`../20_domain/character_creation/domain_rules_mvp.md`](../20_domain/character_creation/domain_rules_mvp.md)
- [`../20_domain/character_creation/aon_player_core_ancestries_59_64.md`](../20_domain/character_creation/aon_player_core_ancestries_59_64.md)
- `../../CharacterManagement.Domain/Entity/DraftCharacter.cs`
- `../../CharacterManagement.Application/Builders/Implementation/CharacterBuilder.cs`
- `../../CharacterManagement.Infrastructure/Repositories/AncestryRepository.cs`
- `../../CharacterManagement.Infrastructure.Tests/`
- `../../CharacterManagement.Domain.Tests/`

## Next steps

1. Не расширять backend MVP в сторону `Background` / `Class` без отдельного решения по scope.
2. При изменении character creation сначала сверяться с `../20_domain/character_creation/domain_rules_mvp.md`.
3. Следующий пользовательский фокус по MVP — frontend wizard.

