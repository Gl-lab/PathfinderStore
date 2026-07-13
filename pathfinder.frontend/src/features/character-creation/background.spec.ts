import { describe, expect, it } from 'vitest'
import type { AbilityCode } from '@/features/characters/api'
import type { Background } from '@/features/character-creation/api'
import {
  getBackgroundFreeBoostOptions,
  isBackgroundChoiceComplete,
} from '@/features/character-creation/background'

const acrobat: Background = {
  id: 'background.acrobat',
  name: 'Acrobat',
  restrictedBoostOptions: ['Strength', 'Dexterity'],
  freeBoostCount: 1,
  grants: [],
}

describe('background choice', () => {
  it('accepts one restricted and one different free boost', () => {
    expect(isBackgroundChoiceComplete(acrobat, 'Dexterity', 'Intelligence')).toBe(true)
  })

  it('rejects a restricted boost outside the background options', () => {
    expect(isBackgroundChoiceComplete(acrobat, 'Wisdom', 'Intelligence')).toBe(false)
  })

  it('rejects duplicate boosts from the same background package', () => {
    expect(isBackgroundChoiceComplete(acrobat, 'Dexterity', 'Dexterity')).toBe(false)
  })

  it('removes the restricted boost from free boost options', () => {
    const abilities: AbilityCode[] = ['Strength', 'Dexterity', 'Intelligence']

    expect(getBackgroundFreeBoostOptions(abilities, 'Dexterity')).toEqual([
      'Strength',
      'Intelligence',
    ])
  })
})
