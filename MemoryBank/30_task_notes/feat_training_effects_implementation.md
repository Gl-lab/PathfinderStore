# Feat Training Effects — Priority 5.4

## Проблема

Feat inventory показывает выбранные и granted feats, но даже полностью определённые training effects не влияют на эффективные Skills/Lore. Одновременно часть ancestry training feats требует replacement choice при конфликте, а weapon familiarity и Martial Performance невозможно корректно применить без weapon catalog.

## План

1. Добавить отдельный pure resolver эффективного feat training, не смешивая его с persistence исходных background/class choices.
2. Подключить фиксированные grants `Dwarven Lore`, `Elven Lore`, `Goblin Lore`, `Halfling Lore`, `Prairie Rider` и `Bardic Lore`.
3. Добавлять только не конфликтующие Skill/Lore grants; конфликтные replacement grants возвращать отдельными typed deferred descriptors, а не молча терять.
4. Передавать effective training в training DTO и skill/lore modifiers, сохраняя provenance feat source.
5. Оставить `Natural Skill`, `Gnome Obsession`, временную training и weapon proficiency effects deferred до соответствующих choice/weapon subsystems.
6. Покрыть resolver, mapper/modifiers и UI отображение deferred replacement тестами; провести review и отдельный коммит.

## Критерии готовности

- фиксированный не конфликтующий feat grant отображается trained и влияет на modifier;
- каждый grant сохраняет source feat id;
- существующая training не дублируется, а необходимый replacement явно виден в API/UI;
- ни один weapon/combat effect не считается применённым без weapon catalog.

## Реализовано

- `FeatTrainingResolver` рассчитывает effective training поверх сохранённых background/class choices, не изменяя aggregate и не дублируя persistence.
- Подключены Dwarven Lore (`Crafting`, `Religion`, `Dwarf Lore`), Elven Lore (`Arcana`, `Nature`, `Elf Lore`), Goblin Lore (`Nature`, `Stealth`, `Goblin Lore`), Halfling Lore (`Acrobatics`, `Stealth`, `Halfling Lore`), Prairie Rider (`Nature`) и Bardic Lore (`Bardic Lore`).
- Каждый применённый `TrainedSkill`/`TrainedLore` использует feat id как provenance source.
- Effective training передаётся и в `CharacterDto.Training`, и в skill/lore modifier mapper; поэтому trained rank и proficiency bonus изменяются согласованно.
- Конфликт существующего training не теряется: `DeferredFeatTrainingGrant` возвращает feat, target и `ReplacementChoiceRequired`, карточка показывает предупреждение.
- У фиксированных Lore feats static dependency теперь отражает только ещё не реализованную proficiency progression; Prairie Rider сохраняет deferred mount mechanics.
- Проверка Player Core effect исправила каталог `Martial Performance`: feat не выдаёт weapon proficiency, а продлевает composition после Strike, поэтому его зависимости — `Spellcasting` и `CombatRules`.

## Оставлено deferred

- `Natural Skill` и `Gnome Obsession` требуют новых постоянных choices;
- `Ancestral Longevity` является временным daily training;
- ancestry weapon familiarity и `Unconventional Weaponry` требуют weapon catalog и weapon-specific proficiency rules;
- progression Additional Lore/Bardic Lore выше первого уровня требует общей advancement subsystem.

## Проверки

- `dotnet test CharacterManagement.Domain.Tests/CharacterManagement.Domain.Tests.csproj --no-restore` — пройдено, 204 tests.
- `dotnet test CharacterManagement.Infrastructure.Tests/CharacterManagement.Infrastructure.Tests.csproj --no-restore` — пройдено, 242 tests.
- `npm test -- --run` — пройдено, 79 tests.
- `npm run build` — пройдено; остаётся существующее предупреждение Vite о размере chunk.

## Результат review

Отдельное review проверило полный список feat effects с training/proficiency dependencies, source provenance, дедупликацию, modifier integration и отображение unresolved replacement. Найдена и исправлена ошибочная классификация `Martial Performance`; иных прямых proficiency grants, которые можно корректно применить без weapon catalog, в baseline нет. Открытых замечаний не осталось.
