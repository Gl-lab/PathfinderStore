# Priority 6 Final Cross-Review

## Итог

Приоритет 6 «Языки и финализация создания персонажа» завершён 20 июля 2026 года. Проверены вместе четыре последовательных vertical slices:

1. Player Core language catalog и ancestry language rules.
2. Additional language selection с server-computed count и allowed pool.
3. Единый server-side completion report.
4. Явная финализация `Draft` → `Completed` и status read model.

Коммиты реализации: `19c975c`, `f21a8fb`, `6155d94`, `fc0bcee`.

## Проверка критериев

| Критерий | Результат |
|---|---|
| Количество языков зависит от итогового Intelligence modifier и ancestry rule | Выполнено в `LanguageSelectionResolver`; create pipeline повторно вычисляет modifier из aggregate state. |
| Duplicate и unavailable choices отклоняются сервером | Выполнено; также отклоняются starting language и неизвестные IDs. |
| Финализируется только полный персонаж | Выполнено через `CharacterCompletionEvaluator` в owner-scoped finalize handler. |
| List/details различают draft и completed | Выполнено в DTO и frontend badges/details action. |
| Frontend не владеет формулой полноты | Выполнено; UI использует server-provided count/pool, `completion.isComplete` и issues. |

## Cross-slice review

- Catalog definitions остаются code-owned reference data; aggregate хранит только stable language IDs.
- Human rule исправлено до `1 + max(0, Intelligence modifier)`; для остальных baseline ancestries используется `max(0, modifier)`.
- Uncommon access не открывается булевым флагом автоматически: resolver принимает только доверенный server-side access context.
- Смена ancestry или любого ability-affecting package очищает stale language selection.
- Completion повторно использует существующие spell, feat и language resolvers, а не копирует их ограничения.
- Все восемь class flows, включая пять spellcasting loadouts, проходят completion как полные.
- Исправлен обнаруженный cross-review дефект Witch familiar spell round-trip для Patron с единственным grant.
- Existing database rows получают безопасный status `Draft`; completed timestamp nullable только для draft.
- Completed aggregate блокирует build/edit methods. Delete остаётся отдельной lifecycle operation; reopen/respec отсутствует намеренно.

## Миграции

- `AddCharacterLanguages` добавляет `SelectedAdditionalLanguageIds` как non-null `jsonb` с default empty array.
- `AddCharacterCreationStatus` добавляет `CreationStatus` с default `Draft` и nullable UTC `CompletedAtUtc`.
- `dotnet ef migrations has-pending-model-changes` сообщает об отсутствии незаписанных изменений модели.

## Quality gate

- `CharacterManagement.Domain.Tests`: 220 passed.
- `CharacterManagement.Infrastructure.Tests`: 257 passed.
- Frontend Vitest: 82 passed в 29 files.
- Frontend production build: passed.
- Frontend ESLint: passed.
- Cross-priority `git diff --check`: passed.

Существующие repository-wide compiler warnings не изменены этим приоритетом. Vite сохраняет известное предупреждение о размере основного bundle.

## Оставшийся scope

- Источники access к uncommon языкам (region, feat или другой rule source).
- Отдельный respec/reopen workflow для completed characters.
- Starting equipment/inventory boundary и последующая combat-ready card.

Vikunja остаётся источником истины по tracker status. В текущей сессии MCP Vikunja был недоступен, поэтому локальная документация обновлена полностью, а синхронизацию внешних карточек необходимо выполнить после восстановления подключения.
