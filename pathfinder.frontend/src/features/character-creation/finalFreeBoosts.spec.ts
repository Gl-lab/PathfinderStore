import { describe, expect, it } from 'vitest'
import {
  calculateAbilityScorePreview,
  isFinalFreeBoostDisabled,
  isFinalFreeBoostSelectionComplete,
} from '@/features/character-creation/finalFreeBoosts'

describe('final free boosts', () => {
  it('requires exactly four unique abilities', () => {
    expect(
      isFinalFreeBoostSelectionComplete([
        'Strength',
        'Dexterity',
        'Constitution',
        'Wisdom',
      ]),
    ).toBe(true)
    expect(isFinalFreeBoostSelectionComplete(['Strength', 'Dexterity', 'Constitution'])).toBe(
      false,
    )
    expect(
      isFinalFreeBoostSelectionComplete(['Strength', 'Strength', 'Constitution', 'Wisdom']),
    ).toBe(false)
  })

  it('disables only unselected abilities when four are selected', () => {
    const selected = ['Strength', 'Dexterity', 'Constitution', 'Wisdom'] as const

    expect(isFinalFreeBoostDisabled('Charisma', [...selected])).toBe(true)
    expect(isFinalFreeBoostDisabled('Strength', [...selected])).toBe(false)
  })

  it('calculates the score preview from boosts and flaws', () => {
    const result = calculateAbilityScorePreview(
      ['Strength', 'Strength', 'Dexterity', 'Constitution'],
      ['Charisma'],
    )

    expect(result.Strength).toBe(14)
    expect(result.Dexterity).toBe(12)
    expect(result.Constitution).toBe(12)
    expect(result.Charisma).toBe(8)
    expect(result.Wisdom).toBe(10)
  })
})
