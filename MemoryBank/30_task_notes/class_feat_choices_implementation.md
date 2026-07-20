# Class Feat Choices — Priority 5.3

## Проблема

Единый feat catalog уже содержит class feats, но создание персонажа не материализует обязательные class feat slots. Из-за этого Fighter, Ranger и Rogue можно завершить без положенного выбора, а class choices Bard, Cleric, Druid и Wizard остаются отдельными descriptors и не входят в общий feat inventory.

## План

1. Ввести stable `FeatChoice` (`SourceId`, `FeatId`) и resolver обязательных slots первого уровня.
2. Требовать top-level class feat для Fighter, Ranger и Rogue, а также обязательный skill feat для Rogue.
3. Требовать дополнительные Wizard choices для Experimental Spellshaping и School of Unified Magical Theory.
4. Материализовать fixed grants от Bard Muse, Druidic Order, Cloistered Cleric и Improved Familiar Attunement; условный Warpriest grant оставить typed/deferred до weapon rules.
5. Валидировать class trait, level, category, специальные ограничения spellshape slot и отсутствие повторов.
6. Сохранить choices как JSON-массив stable pairs через EF migration, вернуть их в `CharacterDto.Feats`, добавить wizard UI и тесты.
7. Провести отдельное review и только после полного прогона сделать коммит.

## Критерии готовности

- Fighter и Ranger нельзя создать без одного корректного class feat; Rogue требует одновременно class feat и skill feat.
- Wizard требует 0–2 дополнительных choices в зависимости от School/Thesis; каждый slot имеет собственный stable source id.
- Feat другого класса, уровня или недопустимый для spellshape slot отклоняется сервером.
- Granted и selected class feats имеют различимый acquisition/source provenance и переживают persistence round-trip.
- Wizard показывает только применимые class feats и включает выборы в review/details.

## Границы

Механические эффекты feats не исполняются в этом slice. Условный `Deadly Simplicity` Warpriest не выдаётся без weapon catalog и проверки favored weapon. Skill feats с дополнительным parameter choice (`Assurance`, `Specialty Crafting`, `Terrain Expertise`) не предлагаются Rogue до появления соответствующей модели выбора.

## Реализовано

- `FeatChoice` хранит stable `SourceId` и `FeatId`; `DraftCharacter.SelectedClassFeatChoices` сохраняется в `jsonb` миграцией `AddClassFeatChoices`.
- `CharacterFeatResolver` выводит обязательные slots из class rules, Arcane School и Arcane Thesis, проверяет category, level, class trait, spellshape boundary и уникальность.
- Fighter и Ranger требуют один top-level class feat; Rogue требует class feat и отдельный skill feat. Experimental Spellshaping требует `Reach Spell` или `Widen Spell`; Unified Magical Theory добавляет независимый Wizard slot.
- Для выбранного Rogue skill feat проверяются поддерживаемые prerequisites на trained skill и другой feat после применения class training; parameterized feats остаются недоступны для этого slot.
- Bard Muse, Druidic Order, Cloistered Cleric и Improved Familiar Attunement теперь возвращают fixed class feats как `Granted`/`ClassChoice`.
- Write-side запрещает выбрать feat, уже выданный class choice; legacy rows с пустым новым JSON-полем остаются читаемыми.
- Реализованные catalog dependencies удалены из class/Muse/Order/School/Thesis descriptors; daily thesis choice более высоких уровней остаётся deferred.
- Wizard показывает только feats своего класса, spellshape slot дополнительно ограничен двумя применимыми feats; все class choices попадают в request и review.

## Проверки

- `dotnet test CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj --no-restore` — пройдено, 205 tests.
- `dotnet test CharacterManagement.Infrastructure.Tests/CharacterManagement.Infrastructure.Tests.csproj --no-restore` — пройдено, 243 tests.
- `npm test -- --run` — пройдено, 80 tests.
- `npm run build` — пройдено; остаётся существующее предупреждение Vite о размере chunk.
- `dotnet ef migrations has-pending-model-changes ... --no-build` — модель синхронизирована.

## Результат review

Во время отдельного review найден и исправлен конфликт selected/granted: сочетание Unified Magical Theory и Improved Familiar Attunement больше не позволяет выбрать `Familiar` повторно. Итоговый cross-review Priority 5 дополнительно обнаружил, что декларативный `Rogue SkillFeatChoice` не был материализован; slot, prerequisites и frontend filtering добавлены отдельным исправлением. После исправлений проверены completeness обязательных slots, class/spellshape/skill availability, JSON persistence, legacy-read boundary, provenance fixed grants и очистка реализованных catalog dependencies. Открытых замечаний не осталось.
