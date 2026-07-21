# Priority 8 Final Cross-Review

## Итог

Приоритет 8 «Боевая карточка персонажа v1» завершён 21 июля 2026 года. Вместе проверены пять последовательных vertical slices:

1. Application-owned read boundary разрешённого снаряжения.
2. AC с armor, Dexterity cap, proficiency и breakdown.
3. Attack/damage для Fist и экипированного оружия.
4. Class DC, spell attack и spell DC.
5. Командное current/temporary HP state относительно вычисляемого maximum HP.

Коммиты реализации: `13e7bea`, `3f1d15d`, `16edb0f`, `6fc9ed6`, `9d80066`.

## Проверка критериев

| Критерий | Результат |
|---|---|
| Значения и breakdown вычисляются сервером | Выполнено: frontend получает готовые AC, Strikes, DC и HP state и не повторяет формулы. |
| Расчёты изолированы от временного equipment catalog | Выполнено: mapper использует `IAllowedEquipmentReader` и application-owned records; текущий repository скрыт в infrastructure adapter. |
| Starting equipment полностью поддержан без Store | Выполнено: адаптер разрешает сохранённые items, equipped state, proficiency, стоимость и Массу; ссылок на `Store.*` нет. |
| Bonus layers типизированы | Выполнено: item/status/circumstance bonuses представлены отдельными `StatisticBonus` слоями в домене и API. |
| Внутренние item definitions не раскрываются | Выполнено: API проецирует безопасный subset, а не `EquipmentDefinition`. |
| Доступ отделён от формул | Выполнено: owner scope применяется repository/handler слоем; calculators не знают о пользователе. |
| Current HP ограничен maximum HP | Выполнено: read и команды clamp current в диапазон `0..maximum`, включая изменение вычисляемого maximum. |
| HP меняется командами | Выполнено: damage, heal, grant temporary и clear temporary доступны через owner-scoped command endpoint; прямое клиентское состояние не принимается. |
| Encounter mechanics не включены неявно | Выполнено: actions, conditions, Raise a Shield и spell/feat effect execution остались вне card v1. |

## Cross-slice review

- `CharacterDetailsDtoMapper` разрешает equipment один раз и передаёт application-owned loadout в starting-equipment и combat projections.
- AC использует unarmored defense либо единственную экипированную armor entry, применяет Dexterity cap и item bonus брони.
- Strike list всегда содержит Fist и добавляет экипированное оружие. Finesse выбирает лучший attack ability; ranged, thrown и propulsive damage различаются сервером.
- Attack, damage, AC, class DC и spell statistics возвращают ability/proficiency/source breakdown и раздельные bonus categories.
- Class DC использует class-specific target и выбранную key ability. Spell targets разделены по tradition; Witch получает tradition и grants от выбранного Patron.
- Maximum HP остаётся derived value. Nullable persisted current HP сохраняет legacy-семантику «полные HP до первой runtime-команды»; temporary HP имеет нулевой default.
- Damage сначала расходует temporary HP, healing не превышает maximum, а новый temporary grant заменяет старый только при большем значении.
- `POST /api/character/{id}/hit-points` проверяет владельца до изменения агрегата; draft-персонажи runtime HP-команды не принимают.

## Миграция

`AddCharacterHitPointState` добавляет nullable `CurrentHitPoints` и non-null `TemporaryHitPoints` с default `0`. Миграция и snapshot созданы через `dotnet ef`. Nullable current обеспечивает совместимость существующих завершённых персонажей без backfill вычисляемого maximum.

## Quality gate

- `dotnet test Pathfinder.sln --no-restore`: passed; `CharacterManagement.Domain.Tests` — 242, `CharacterManagement.Infrastructure.Tests` — 286.
- Frontend Vitest — 82 tests в 29 files.
- Frontend production build — passed.
- Frontend ESLint — passed.
- Scoped `git diff --check` — passed.

Существующие repository-wide compiler warnings не изменены этим приоритетом. Vite сохраняет известное предупреждение о размере основного bundle.

## Оставшийся scope

- Raise a Shield, shield HP/Hardness actions и hand/action economy.
- Encounter conditions, multiple attack penalty и временные combat modifiers.
- Специализированные ancestry unarmed attacks и исполнение combat effects feats/features.
- Общий stacking resolver для нескольких bonuses одного типа; card v1 имеет типизированные слои, но текущие источники не создают конкурирующие однотипные bonuses.
- Weapon runes, persistent upgrades, ammunition lifecycle и runtime Inventory/Store.
- Dying/wounded recovery, regeneration и spell/item execution lifecycle.
- Campaign/GM access policy: формулы готовы к другому access layer, но сам campaign scope относится к Priority 9.

Vikunja в текущем окружении недоступна. Локальная документация обновлена полностью; внешние карточки необходимо синхронизировать после восстановления подключения.
