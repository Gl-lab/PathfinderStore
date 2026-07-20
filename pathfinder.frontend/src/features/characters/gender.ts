import type { CharacterGender } from '@/features/characters/api'

export type SelectableCharacterGender = Exclude<CharacterGender, 'NotSpecified'>

export function isSelectableCharacterGender(
  gender: CharacterGender | null,
): gender is SelectableCharacterGender {
  return gender === 'Male' || gender === 'Female'
}

export function isLegacyGenderSelectionRequired(gender: CharacterGender): boolean {
  return gender === 'NotSpecified'
}
