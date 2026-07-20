import type { AbilityCode } from '@/features/characters/api'
import type { Background } from '@/features/character-creation/api'
import type { BackgroundTrainingChoice } from '@/features/character-creation/api'

export function isBackgroundChoiceComplete(
  background: Background | null,
  restrictedBoost: AbilityCode | null,
  freeBoost: AbilityCode | null,
): boolean {
  return (
    background !== null &&
    restrictedBoost !== null &&
    freeBoost !== null &&
    background.restrictedBoostOptions.includes(restrictedBoost) &&
    restrictedBoost !== freeBoost
  )
}

export function createBackgroundTrainingChoices(
  background: Background | null,
): BackgroundTrainingChoice[] {
  if (!background) return []

  return background.grants
    .filter(
      (grant) =>
        grant.requiresChoice &&
        (grant.kind === 'SkillTraining' ||
          grant.kind === 'LoreTraining' ||
          grant.kind === 'SkillFeat'),
    )
    .map((grant) => ({
      grantId: grant.id,
      targetId: null,
      customLoreTopic: null,
    }))
}

export function isBackgroundTrainingComplete(
  background: Background | null,
  choices: BackgroundTrainingChoice[],
): boolean {
  if (!background) return false

  const requiredGrants = background.grants.filter(
    (grant) =>
      grant.requiresChoice &&
      (grant.kind === 'SkillTraining' ||
        grant.kind === 'LoreTraining' ||
        grant.kind === 'SkillFeat'),
  )
  return requiredGrants.every((grant) => {
    const choice = choices.find((item) => item.grantId === grant.id)
    if (!choice) return false
    if (grant.allowsCustomLore)
      return choice.targetId === null && Boolean(choice.customLoreTopic?.trim())

    return (
      choice.customLoreTopic === null &&
      choice.targetId !== null &&
      grant.options.some((option) => option.id === choice.targetId)
    )
  })
}

export function getBackgroundTrainingLabels(
  background: Background | null,
  choices: BackgroundTrainingChoice[],
): string[] {
  if (!background) return []

  return background.grants
    .filter(
      (grant) =>
        grant.kind === 'SkillTraining' ||
        grant.kind === 'LoreTraining' ||
        grant.kind === 'SkillFeat',
    )
    .map((grant) => {
      if (!grant.requiresChoice) return grant.name
      const choice = choices.find((item) => item.grantId === grant.id)
      if (grant.allowsCustomLore) {
        const topic = choice?.customLoreTopic?.trim()
        if (!topic) return grant.name
        return topic.toLocaleLowerCase().endsWith(' lore') ? topic : `${topic} Lore`
      }

      return grant.options.find((option) => option.id === choice?.targetId)?.name ?? grant.name
    })
}

export function getBackgroundFreeBoostOptions(
  abilityCodes: AbilityCode[],
  restrictedBoost: AbilityCode | null,
): AbilityCode[] {
  return abilityCodes.filter((abilityCode) => abilityCode !== restrictedBoost)
}
