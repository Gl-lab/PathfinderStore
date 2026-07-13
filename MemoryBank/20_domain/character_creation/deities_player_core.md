# Deities and Faiths — Player Core

## Назначение

Документ фиксирует baseline раздела Deities из `Player Core`, страницы 35–39. Stable id строится как `deity.<snake_case_name>`.

## Состав каталога

Core deities: Abadar, Asmodeus, Calistria, Cayden Cailean, Desna, Erastil, Gorum, Gozreh, Iomedae, Irori, Lamashtu, Nethys, Norgorber, Pharasma, Rovagug, Sarenrae, Shelyn, Torag, Urgathoa и Zon-Kuthon.

Faiths and philosophies:

- `deity.green_faith` имеет devotee benefits и допустим для Cleric;
- `deity.atheism` не имеет devotee benefits и недопустим для Cleric.

## Типизированные поля

Каждая допустимая Cleric запись определяет:

- divine skill (`skill.*`);
- один или несколько favored weapons (`weapon.*`) и category;
- Divine Font options (`Heal`, `Harm`);
- allowed и, при наличии, required sanctification (`Holy`, `Unholy`);
- primary domains (`domain.*`);
- granted cleric spells как `rank + spell.*`.

## Sanctification

Required:

- Holy: Iomedae;
- Unholy: Asmodeus, Rovagug, Urgathoa.

Gozreh и Pharasma не дают sanctification. Остальные записи разрешают optional Holy, Unholy или оба варианта согласно каталогу.

## Supported effects

- divine skill training с replacement при конфликте Background;
- individual favored weapon proficiency;
- сохранение выбранных Font и sanctification.

Domains и granted spells пока являются typed references. Alternate domains, domain selection, focus spells, spell preparation и font slots исключены.
