# Character Completion Validation Implementation

## Статус

Завершено 20 июля 2026 года как slice 6.3 roadmap character creation.

## Назначение

`CharacterCompletionEvaluator` является единым server-side источником решения о полноте draft. Он не изменяет персонажа и возвращает структурированный `CharacterCompletionDto` с `IsComplete` и уникальными issue codes.

## Проверяемые группы

- identity: имя и явно выбранный пол;
- ancestry package: heritage, ancestry feat и ancestry free boosts;
- background package и его skill feat;
- class package, key ability и обязательные class-specific choices;
- spell loadout для Bard, Cleric, Druid, Witch и Wizard через существующие доменные resolvers;
- обязательные selected/granted feats через общий feat resolver;
- initial и дополнительные class training grants с количеством от итогового Intelligence modifier;
- четыре final free boosts;
- exact additional language count, uniqueness и availability через `LanguageSelectionResolver`.

Один и тот же отчёт включается в list/details `CharacterDto`. Frontend получает готовые `isComplete` и issue codes и не содержит параллельной формулы полноты.

## Совместимость

Старые записи могут быть прочитаны как draft с несколькими issues. Completion evaluation не пытается автоматически дополнить или исправить legacy state. Явная команда финализации следующего slice обязана использовать этот evaluator и отклонять неполный draft.

## Проверки

- Все восемь поддерживаемых class creation flows проходят evaluator как complete, включая пять spellcasting loadouts.
- Legacy skeleton возвращает структурированные issues identity, ancestry, background, class и final boosts.
- Completion-прогон выявил и исправил round-trip Witch familiar spell: сохранённый resolved ID больше не интерпретируется повторно как пользовательский choice для Patron с единственным grant.
- Выполнены Domain и Infrastructure test suites, frontend build и Vitest.
