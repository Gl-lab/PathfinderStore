import type { CharacterClass } from '@/features/character-creation/api'
import type { AbilityCode } from '@/features/characters/api'

export function getAutomaticallySelectedKeyAbility(
  keyAbilityOptions: AbilityCode[],
): AbilityCode | null {
  return keyAbilityOptions.length === 1 ? keyAbilityOptions[0] : null
}

export function isCharacterClassChoiceComplete(
  characterClass: CharacterClass | null,
  keyAbility: AbilityCode | null,
): boolean {
  return (
    characterClass !== null &&
    keyAbility !== null &&
    characterClass.keyAbilityOptions.includes(keyAbility)
  )
}
