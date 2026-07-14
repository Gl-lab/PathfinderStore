# Priority 1 — Final Cross-Review

## Цель и scope

Проверить как единый change set roadmap commit `732cd90` и семь последовательных срезов Priority 1 до `2213bbe`: Class Skills, Hunter's Edge, Druidic Order, Bard Muse, Witch Patron, Wizard Arcane School и Arcane Thesis. Review выполняется после отдельных review/commit каждой подзадачи и до перехода к Cleric spell flow.

## План review

1. Сопоставить восемь class catalog entries с обязательными Player Core choices и убедиться, что реализованные choices больше не объявляют generic `ClassChoiceCatalog`.
2. Проверить симметричные domain/application инварианты: required choice для целевого класса, запрет для остальных, atomic invalid mutation и очистка при смене класса.
3. Сверить create request, builder/handler positional order, repository resolution, persistence и read/legacy contracts.
4. Проверить, что frontend step completeness, submit guard, class reset, review и details используют один и тот же набор choices.
5. Проверить минимальность шести migrations и отсутствие pending model changes.
6. Повторить domain/infrastructure/frontend tests, Web build, lint, production build и `git diff --check`.
7. Исправления review оформить отдельным commit, не переписывая историю завершённых slices.

## Review плана перед исправлениями

План проверен до изменения production-кода:

- scope ограничен Priority 1 и его прямыми create/read контрактами;
- нормативные catalog данные уже проверены в планах отдельных slices и повторно оцениваются на структурную согласованность, без нового массового исследования;
- существующие legacy rows должны оставаться readable, migrations не переписываются;
- исправления допустимы только при наличии воспроизводимого расхождения и получают regression-тест;
- полный прогон выполняется после review fixes, а не заменяется результатами отдельных подзадач.

## Найденные замечания

### 1. Stale Ranger ClassChoiceCatalog

Hunter's Edge catalog и endpoint реализованы в 1.2, но `CharacterClassRepository` сохранил `ClassChoiceCatalog` на rule и top-level Ranger dependencies. Это ложно сообщает клиенту, что обязательный выбор Ranger всё ещё не поддержан.

Исправление: снять stale dependency и добавить cross-class regression test, запрещающий `ClassChoiceCatalog` у всех восьми классов после завершения Priority 1.

### 2. Расхождение step и submit guards

Class step проверяет базовый key ability или полный Rogue Racket flow, но `submit()` отдельно дублирует checks и не повторяет эту ветку. В штатной навигации шаг блокирует ошибку, однако финальный guard не является самодостаточным и может разойтись с step guard при дальнейшем развитии UI.

Исправление: вынести общую `isSelectedClassChoiceComplete()` и использовать её и в `canContinue`, и непосредственно перед API request.

Других открытых замечаний по domain atomicity, catalog ownership, create/read/legacy paths, migrations и deferred-mechanics boundaries на этом этапе не найдено.

## Критерии завершения review

- ни один class/rule не содержит `ClassChoiceCatalog` после реализации всех обязательных choices;
- step и submit используют общий class-choice guard;
- отдельные и полные test/build/EF/diff checks проходят;
- worktree чист после отдельного review commit.

## Результат

Оба замечания исправлены:

- Ranger rule и top-level dependencies больше не содержат `ClassChoiceCatalog`; regression-test проверяет это для всех классов и всех class rules;
- `isSelectedClassChoiceComplete()` используется и class step, и финальным submit guard, поэтому base key ability и Rogue Racket flow не могут разойтись между двумя точками валидации.

После исправлений повторно просмотрены aggregate validation order, builder/handler argument order, converter nullable paths, все шесть Priority 1 migrations, frontend class reset и границы deferred mechanics. Новых замечаний не найдено.

Финальные проверки:

- `dotnet test Pathfinder.sln --no-restore`: passed; 144 domain и 162 infrastructure/API tests;
- frontend: 46 tests passed, lint passed, production build passed;
- EF: pending model changes отсутствуют;
- `git diff --check`: passed.

Solution build по-прежнему выводит существующие nullable/type-conflict warnings в старом коде; новые и изменённые Priority 1 файлы дополнительных compiler warnings не добавили. Открытых замечаний по Priority 1 не осталось.
