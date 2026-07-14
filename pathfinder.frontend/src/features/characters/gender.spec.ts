import { describe, expect, it } from 'vitest'
import {
  isLegacyGenderSelectionRequired,
  isSelectableCharacterGender,
} from '@/features/characters/gender'

describe('character gender', () => {
  it.each(['Male', 'Female'] as const)('accepts selectable gender %s', (gender) => {
    expect(isSelectableCharacterGender(gender)).toBe(true)
  })

  it.each(['NotSpecified', null] as const)('rejects creation gender %s', (gender) => {
    expect(isSelectableCharacterGender(gender)).toBe(false)
  })

  it('requires legacy selection only for NotSpecified', () => {
    expect(isLegacyGenderSelectionRequired('NotSpecified')).toBe(true)
    expect(isLegacyGenderSelectionRequired('Male')).toBe(false)
    expect(isLegacyGenderSelectionRequired('Female')).toBe(false)
  })
})
