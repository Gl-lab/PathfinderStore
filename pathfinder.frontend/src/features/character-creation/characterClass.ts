import type { CharacterClass } from '@/features/character-creation/api'
import type { AbilityCode } from '@/features/characters/api'

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
