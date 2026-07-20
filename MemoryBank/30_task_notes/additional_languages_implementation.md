# Additional Languages Implementation

## Статус

Завершено 20 июля 2026 года как slice 6.2 roadmap character creation.

## Результат

- Количество дополнительных языков вычисляет доменный resolver из итогового Intelligence modifier и `AdditionalLanguageRule` ancestry.
- Human получает `1 + max(0, modifier)`, остальные ancestry текущего baseline — `max(0, modifier)`.
- Допустимый pool строится из fixed ancestry IDs, common languages при наличии соответствующего разрешения и доверенного server-side access context.
- Starting languages исключены; exact count, uniqueness и availability проверяются сервером.
- Выбор хранится в `DraftCharacter.SelectedAdditionalLanguageIds`; EF использует `jsonb` и миграцию `AddCharacterLanguages`.
- API preview возвращает только вычисленные `requiredCount` и `availableLanguages`; create request хранит только stable IDs.
- Wizard запрашивает preview после финальных boosts, не воспроизводит формулу полноты и показывает выбранные языки в review.
- Details использует объединённый серверный `KnownLanguageIds`.

## Проверки

- Domain tests покрывают Human formula, access context, duplicate, starting и unavailable choices.
- Infrastructure tests покрывают preview handler, create persistence и полный existing create flow.
- Frontend unit tests покрывают exact-count validation и reconciliation stale choices.
- Выполнены backend test suites, frontend build, Vitest и ESLint.

## Оставшаяся граница

- Источники server-side access для uncommon языков пока не моделируются.
- Completion validation и явная финализация выполняются в slices 6.3 и 6.4.
