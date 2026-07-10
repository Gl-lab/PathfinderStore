# Pathfinder 2e Skill Catalog from Player Core

Дата сбора: 2026-04-04.

Источник:
- [Skills index](https://2e.aonprd.com/Skills.aspx)
- [Player Core skills chapter](https://2e.aonprd.com/Rules.aspx?ID=1391)
- Отдельные skill pages на Archives of Nethys

## Краткий вывод

В `Player Core` есть 16 общих skills, а `Lore` выступает как отдельное семейство поднавыков.

Общий список, который показывает AoN:
- Acrobatics
- Arcana
- Athletics
- Crafting
- Deception
- Diplomacy
- Intimidation
- Lore
- Medicine
- Nature
- Occultism
- Performance
- Religion
- Society
- Stealth
- Survival
- Thievery

## Общая рамка

По `Skills` chapter на AoN:
- skills представляют training и experience персонажа;
- каждый skill привязан к одной из характеристик;
- expertise в skill приходит из background, class и других источников;
- в chapter отдельно описаны `Identify Magic`, `Learn a Spell` и связанные usage rules.

## Каталог

| Skill | Key ability | Source page | Кратко | Основные trained uses |
|---|---|---:|---|---|
| Acrobatics | Dex | 233 | Координация, ловкость, баланс и акробатика | `Balance`, `Tumble Through`, `Maneuver in Flight`; также помогает `Escape`, `Arrest a Fall`, `Grab an Edge` |
| Arcana | Int | 234 | Знание arcane magic, теорий и связанных существ | `Decipher Writing`, `Identify Magic`, `Learn a Spell`; также `Borrow an Arcane Spell` |
| Athletics | Str | 235 | Физическая сила и контроль движения | `Climb`, `High Jump`, `Long Jump`, `Swim`, `Grapple`, `Shove`, `Disarm`, `Reposition`, `Trip` |
| Crafting | Int | 236 | Создание и починка предметов | `Craft`, `Earn Income by crafting`; также works with consumables/ammunition batches |
| Deception | Cha | 237 | Обман, маскировка, ложь, отвлечение внимания | `Create a Diversion`, `Lie`, `Feint`, `Create a Disguise` |
| Diplomacy | Cha | 239 | Переговоры, дружеское общение и влияние на людей | `Gather Information` и связанные social interactions |
| Intimidation | Cha | 240 | Давление, угрозы и запугивание | `Coerce` и другие intimidation-based social actions |
| Lore | varies by subtype | n/a | Семейство узких знаний по теме или области | `Recall Knowledge` по подкатегории Lore; конкретные uses зависят от subcategory |
| Medicine | Wis | 241 | Лечение ран, болезней и ядов | `Administer First Aid`, `Treat Disease`, `Treat Poison`; также `Treat Wounds` |
| Nature | Wis | 242 | Природа, животные, фауна и primal magic | `Command an Animal`, `Identify Magic`, `Learn a Spell` |
| Occultism | Int | 243 | Оккультные знания, философии и сверхъестественное | `Decipher Writing`, `Identify Magic`, `Learn a Spell` |
| Performance | Cha | 243 | Исполнение, сценические и художественные выступления | зависит от формы performance; AoN подчеркивает, что audience и format matter |
| Religion | Wis | 244 | Боги, догма, вера, divine magic | `Decipher Writing`, `Identify Magic`, `Learn a Spell` |
| Society | Int | 244 | История, общество, институты и settlement lore | `Recall Knowledge` о local history, law, structure, humanoid cultures |
| Stealth | Dex | 244 | Скрытность, скрытие предметов и уход из поля зрения | `Conceal an Object` и действия, связанные с hiding/sneaking |
| Survival | Wis | 246 | Выживание в дикой местности | `Sense Direction`, `Subsist in the wild`; связан с tracking и hiding trail |
| Thievery | Dex | 246 | Воровские и ловкие манипуляции | `Palm an Object` и другие thief-like actions |

## Что важно для проекта

1. `Skill list` теперь можно считать найденным как отдельный каталог общего уровня.
2. Для класса и background это критично, потому что многие стартовые правила ссылаются на конкретные skills.
3. `Lore` лучше хранить отдельно от остальных 16 skills:
   - это не один фиксированный skill;
   - это семейство узких подкатегорий;
   - в `Background` и `Class` правилах он чаще выступает как conditional or generated training.

## Практический вывод

Для дальнейшей систематизации стоит разделить данные на два уровня:

- `general_skills`:
  - 16 общих skills из `Player Core`;
- `lore_subskills`:
  - отдельные `Lore`-подкатегории, которые будут приходить из `Background`, `Class`, feats и других источников.

Такой разрез лучше подходит для будущих DTO и UI, чем попытка хранить `Lore` как один-единственный skill.
