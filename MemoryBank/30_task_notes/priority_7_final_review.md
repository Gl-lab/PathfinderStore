# Priority 7 Final Cross-Review

## Итог

Приоритет 7 «Starting equipment и inventory boundary» завершён 21 июля 2026 года. Вместе проверены четыре последовательных vertical slices:

1. Ownership boundary между `CharacterManagement` и будущим Store/Inventory.
2. Минимальный Player Core equipment catalog v1, starting wealth `15 gp` и kits восьми классов.
3. Выбор, server-side cost/availability validation и persistence стартового набора.
4. Equipped state, weapon/armor proficiency matching и Bulk foundation.

Коммиты реализации: `dc93fe8`, `d759770`, `f77ead4`, `50f4ae7`.

## Проверка критериев

| Критерий | Результат |
|---|---|
| Только допустимое и оплаченное снаряжение | Выполнено: resolver принимает только catalog-backed kit references, проверяет option groups, rarity access и лимит `1500 cp`. |
| Стоимость, proficiency applicability и Bulk вычисляются сервером | Выполнено: клиент передаёт только choices/equipped IDs; read model возвращает server-computed totals, ranks и thresholds. |
| Catalog definitions не копируются в character state | Выполнено: aggregate хранит stable equipment IDs, purchase quantity и character-owned equipped quantity. |
| Смена класса инвалидирует несовместимый kit | Выполнено атомарно в `DraftCharacter.SetClassPackage`; покрыто доменным тестом. |
| Store не владеет character creation inventory | Выполнено: Store dependencies не добавлены, legacy пустые item endpoints удалены. |

## Cross-slice review

- Class kits и цены разрешаются только через `IEquipmentRepository`; request не содержит цену, Bulk или игровые item definitions.
- Cleric favored weapon сверяется с выбранным Deity. Unarmed `Fist`/`Claw` не создаёт item line и стоимость.
- Common equipment доступно из baseline catalog; uncommon item допускается только как подтверждённое favored weapon выбранного Deity.
- Equipped IDs должны принадлежать inventory; gear/ammunition нельзя экипировать, одновременно допустима только одна armor entry.
- Weapon proficiency выбирает максимальный применимый rank между конкретным favored weapon и общей simple/martial category. Armor сопоставляется с unarmored/light/medium defense target.
- Total Bulk считается по catalog definition и purchase quantity. Пороги первого уровня вычисляются как `5 + Strength modifier` и `10 + Strength modifier` Bulk.
- Completion повторно разрешает kit и equipped state и сравнивает их с persisted references; повреждённый или устаревший loadout не считается полным.
- Frontend показывает отдельный equipment step, бюджет, equipped choices, а details — сохранённые items, proficiency и Bulk без владения серверными формулами.
- Store, runtime shop/trade, hand/action economy, Raise a Shield, AC и Strike calculations остались за отдельными границами.

## Замечания, исправленные ревью

1. Ammunition definitions получили weapon-group metadata, чтобы будущие Strike rules не определяли совместимость по имени.
2. Weapon proficiency matching исправлен: более высокий общий category rank не перекрывается более низким specific favored-weapon rank.
3. Unarmed favored weapon исправлен: выбор не требует несуществующего purchasable equipment ID.

## Миграция

`AddCharacterStartingEquipment` добавляет nullable `SelectedClassKitId` и два non-null `jsonb` поля с empty-array defaults для option IDs и character equipment state. Миграция создана через `dotnet ef`; pending model changes отсутствуют.

## Quality gate

- `dotnet test Pathfinder.sln --no-restore`: passed; `CharacterManagement.Domain.Tests` — 232, `CharacterManagement.Infrastructure.Tests` — 271.
- Frontend Vitest — 82 tests в 29 files.
- Frontend production build — passed.
- Frontend ESLint — passed.
- Scoped `git diff --check` — passed.

Существующие repository-wide compiler warnings не изменены этим приоритетом. Vite сохраняет известное предупреждение о размере основного bundle.

## Оставшийся scope

- AC breakdown с armor, Dexterity cap и proficiency.
- Strike attack/damage для equipped weapons и unarmed attacks.
- Hand usage, Raise a Shield и runtime inventory operations.
- Расширение каталога за пределы стартовых kits и отдельный runtime Store/Inventory boundary.
- Respec/reopen завершённых персонажей и equipment changes после character creation.

Vikunja в текущем окружении недоступна. Локальная документация обновлена полностью; внешние карточки необходимо синхронизировать после восстановления подключения.
