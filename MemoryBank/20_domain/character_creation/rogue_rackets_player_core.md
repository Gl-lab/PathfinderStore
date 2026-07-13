# Rogue Rackets — Player Core

## Назначение

Документ фиксирует C#-готовый baseline четырёх Rogue rackets из `Player Core`. Данные сверены 2026-07-13 с [Archives of Nethys](https://2e.aonprd.com/Rackets.aspx).

## Каталог

| Racket | Id | Page | Alternative key ability |
|---|---|---:|---|
| Mastermind | `rogue_racket.mastermind` | 166 | `Intelligence` |
| Ruffian | `rogue_racket.ruffian` | 166 | `Strength` |
| Scoundrel | `rogue_racket.scoundrel` | 166 | `Charisma` |
| Thief | `rogue_racket.thief` | 167 | — |

Dexterity доступна Rogue независимо от racket. Alternative key ability расширяет, а не заменяет Dexterity.

## Skill grants

Независимый от racket class grant:

- `class.rogue.skill.stealth` → `skill.stealth`.

Racket grants:

| Grant id | Target/options |
|---|---|
| `rogue_racket.mastermind.skill.society` | `skill.society` |
| `rogue_racket.mastermind.skill.knowledge` | выбор одного: `skill.arcana`, `skill.nature`, `skill.occultism`, `skill.religion` |
| `rogue_racket.ruffian.skill.intimidation` | `skill.intimidation` |
| `rogue_racket.scoundrel.skill.deception` | `skill.deception` |
| `rogue_racket.scoundrel.skill.diplomacy` | `skill.diplomacy` |
| `rogue_racket.thief.skill.thievery` | `skill.thievery` |

`7 + Intelligence modifier` дополнительных Rogue skills не входит в этот flow.

## Duplicate replacement contract

Choice DTO:

```text
RogueTrainingChoice
  GrantId
  SelectedSkillId
  ReplacementSkillId
```

Правила:

1. `SelectedSkillId` обязателен только для finite-choice grant Mastermind knowledge.
2. Для fixed grant `SelectedSkillId` должен отсутствовать.
3. Если resolved target уже выдан Background или предыдущим Rogue grant, обязателен `ReplacementSkillId`.
4. Если конфликта нет, `ReplacementSkillId` запрещён.
5. Replacement должен существовать в general skill catalog и ещё не быть trained.
6. Один `GrantId` встречается в request не более одного раза.
7. Choices для чужого racket, лишние choices и неполные choices отклоняются.
8. Background training сохраняет исходный skill; заменяется более поздний class/racket grant.

## Proficiency grants

Только Ruffian добавляет typed grant:

- source: `rogue_racket.ruffian.proficiency.medium_armor`;
- target: `proficiency.defense.medium_armor`;
- rank: `Trained`.

Grant объединяется с class baseline по target id и максимальному rank.

## Декларативные effects

| Effect id | Boundary |
|---|---|
| `rogue_racket.mastermind.effect.recall_knowledge` | Recall Knowledge делает identified creature off-guard |
| `rogue_racket.ruffian.effect.brutal_sneak_attack` | расширяет допустимое оружие Sneak Attack и critical specialization |
| `rogue_racket.scoundrel.effect.feint` | расширяет Feint и даёт conditional Step |
| `rogue_racket.thief.effect.dexterity_damage` | Dexterity modifier вместо Strength для подходящего finesse melee damage |

Эти effects не исполняются до появления `ClassFeatureRules`.

## Исключённые записи

- Avenger происходит из `War of Immortals`;
- Eldritch Trickster является legacy content из `Advanced Player's Guide`.

Обе записи исключены из `Player Core` baseline.
