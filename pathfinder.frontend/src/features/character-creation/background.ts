import type { AbilityCode } from '@/features/characters/api'
import type { Background } from '@/features/character-creation/api'

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

export function getBackgroundFreeBoostOptions(
  abilityCodes: AbilityCode[],
  restrictedBoost: AbilityCode | null,
): AbilityCode[] {
  return abilityCodes.filter((abilityCode) => abilityCode !== restrictedBoost)
}
