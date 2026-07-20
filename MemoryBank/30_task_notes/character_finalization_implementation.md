# Character Finalization Implementation

## Статус

Завершено 20 июля 2026 года как slice 6.4 roadmap character creation.

## Результат

- `DraftCharacter` хранит `CharacterCreationStatus` (`Draft` или `Completed`) и UTC timestamp завершения.
- `POST /api/character/{id}/finalize` работает только в scope текущего владельца и перед переходом вызывает единый `CharacterCompletionEvaluator`.
- Неполный draft остаётся `Draft`; ответ содержит ошибку с группами незавершённых правил.
- Повторная финализация completed-персонажа идемпотентна и не меняет исходный timestamp.
- После завершения все build/edit methods aggregate отклоняют изменения. Delete остаётся отдельной lifecycle-командой.
- List/details read-модель возвращает status, timestamp и completion report.
- Dashboard и details явно различают draft/completed; кнопка финализации доступна только для полного draft.
- EF migration `AddCharacterCreationStatus` добавляет status с безопасным default `Draft` для существующих строк и nullable `CompletedAtUtc`.

## Инварианты

- `Draft` не имеет completion timestamp.
- `Completed` обязан иметь UTC completion timestamp.
- Status transition однонаправленный; reopen/respec не реализован.
- Completion нельзя определить во frontend: UI использует только server-provided `completion.isComplete` и status.

## Respec boundary

Любое изменение ancestry, background, class, boosts, training, feats, spells или languages завершённого персонажа требует будущего отдельного respec workflow. Он должен явно определить допустимые изменения, стоимость/аудит и повторную проверку полноты; автоматическое возвращение в draft в текущий scope не входит.

## Проверки

- successful и повторная finalization;
- rejection incomplete draft без изменения status;
- persistence status/timestamp;
- запрет build mutation после completion;
- UTC/status consistency;
- frontend build и lint.
