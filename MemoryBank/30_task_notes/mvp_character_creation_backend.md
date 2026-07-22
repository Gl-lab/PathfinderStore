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
- Общий proficiency resolver объединяет grants из class baseline и class choice sources по максимальному rank с сохранением source ids.
- Rogue's Racket, Cleric Doctrine и Deity реализованы как обязательные class choices: каталоги, validation, persistence, API/read-модель и поддержанные typed grants.
- Player Core Deity catalog содержит 22 записи; 21 доступна Cleric. Divine skill, replacement training, favored weapon proficiency, Divine Font и sanctification применяются фактически, domains/granted spells возвращаются декларативно.
- Есть unit и integration tests по ключевым backend-сценариям.
- Реализованы Player Core starting equipment catalog, `15 gp`, class kits, server-side selection/cost validation, persistence stable item references, equipped state, proficiency matching, общая Масса и пороги нагрузки.
- Application-owned allowed-equipment read boundary изолирует боевые расчёты от временного starting-equipment каталога и не раскрывает клиенту внутренние definitions.
- Карточка серверно вычисляет AC, Fist/equipped-weapon Strikes, class DC и spell attack/DC с ability, proficiency и типизированными bonus layers.
- Current и temporary HP сохраняются отдельно от вычисляемого maximum HP и изменяются owner-scoped командами damage/heal/grant/clear.

## Что не готово

- Background skill feat пока представлен декларативно и не применяется к персонажу.
- Общий выбор дополнительных Class skills и level-up proficiency progression пока не реализованы.
- Большинство runtime effects features/spells представлены декларативно.
- Текущий MVP создаёт starting loadout и мигрирует completed-character equipment в базовый runtime Inventory после назначения кампании, но пока не покрывает transfer/loot/Commerce commands, hand/action economy, Raise a Shield, encounter conditions, специализированные ancestry strikes и spell execution lifecycle.
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

1. Следующий Cleric flow — domain choice либо spell preparation/Divine Font slots как отдельная задача.
2. Продолжать остальные обязательные class choices отдельными flows.
3. Не расширять feats, class features, spells и runtime inventory без соответствующих каталогов и доменных решений.
4. При изменении character creation сверяться с `../20_domain/character_creation/domain_rules_target_full.md` и актуальными catalog documents.

