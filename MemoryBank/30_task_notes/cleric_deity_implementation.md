# Player Core Deity Catalog + Cleric Deity Choice — Implementation Plan

## Проблема

Cleric Doctrine уже сохраняется, но Deity остаётся декларативной зависимостью. Из-за этого нельзя завершить нормативный class package Cleric, применить divine skill и proficiency favored weapon, выбрать Divine Font и sanctification или вернуть deity-derived данные в read-модели.

## Baseline и граница

Источник — `Player Core`, страницы 35–39, и соответствующие remaster-страницы Archives of Nethys.

Каталог содержит 22 записи Player Core:

- 20 core deities;
- Green Faith с devotee benefits и допустимым Cleric flow;
- Atheism как faith/philosophy без devotee benefits, недопустимый для Cleric.

В task входят divine skill, favored weapon references/categories, Divine Font options, sanctification rule, primary domains и granted cleric spells. Alternate domains, edicts/anathema и intercession не входят.

## Поддержанные эффекты

- Cleric обязан выбрать одну из 21 записей с devotee benefits.
- Divine skill фактически выдаётся как trained skill.
- Если Background уже обучил divine skill, Cleric обязан выбрать replacement general skill.
- Favored weapon создаёт typed individual proficiency grant; class/doctrine/deity grants объединяются общим resolver.
- Divine Font сохраняется как `Heal` или `Harm` и проверяется по deity options.
- Required sanctification обязательна; optional sanctification может отсутствовать или быть одним из разрешённых значений.
- Domains и granted spells возвращаются typed descriptors, но не применяются до отдельных flows.
- Deadly Simplicity остаётся deferred: Deity и weapon data появятся, но ClassFeatCatalog всё ещё отсутствует.

## Persistence и legacy

Добавляются nullable `SelectedDeityId`, `SelectedDivineFont` и `SelectedDivineSanctification`. Nullable сохраняет чтение legacy Cleric rows. Новые create requests для Cleric без полного deity package отклоняются; non-Cleric не может передать deity-derived class fields.

## API и frontend

- `GET /api/classes/cleric/deities` возвращает полный каталог 22 записей и признак допустимости для Cleric.
- Create request получает deity id, font, sanctification и optional divine-skill replacement.
- Class package read-модели получает Deity package.
- Wizard показывает Deity только для Cleric, автоматически выбирает единственный font и required sanctification, запрашивает replacement только при конфликте training.
- Review/details показывают Deity, font, sanctification, favored weapons, domains и granted spells.

## Этапы

1. Зафиксировать нормативную Player Core matrix и stable ids.
2. Добавить domain model, 22-entry catalog и catalog tests.
3. Добавить divine skill resolver и deity proficiency matrix.
4. Расширить aggregate/builder и инварианты class package.
5. Добавить API, validation, create/read contracts и tests.
6. Добавить EF mapping, CLI migration, round-trip и legacy tests.
7. Обновить frontend wizard, review/details, localization и tests.
8. Обновить MemoryBank и выполнить полные проверки.
9. Провести отдельный code review, исправить замечания и повторить проверки.

После каждого этапа план пересматривается. Domain choice, spell preparation, Divine Font spell slots, edicts/anathema enforcement и Deadly Simplicity остаются отдельными задачами.

## Критерии готовности

- каталог возвращает 22 уникальные Player Core записи;
- Atheism присутствует в каталоге, но запрещён Cleric;
- новый Cleric требует Deity, Font и required sanctification;
- non-Cleric запрещает deity class fields;
- divine skill/replacement и favored weapon proficiency вычисляются без snapshot;
- domains и granted spells доступны typed read-моделью и не применяются преждевременно;
- legacy Cleric без Deity читается;
- EF/backend/frontend tests, build и lint проходят;
- финальный code review не содержит открытых замечаний.

## Статус

- normative boundary проверена;
- implementation plan зафиксирован;
- этапы 1–8 завершены;
- этапы 1–9 завершены;
- финальный code review исправил stale replacement при смене Background skill; открытых замечаний нет.
