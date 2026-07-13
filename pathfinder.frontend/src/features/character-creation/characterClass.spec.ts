import { describe, expect, it } from 'vitest'
import type { CharacterClass } from '@/features/character-creation/api'
import { isCharacterClassChoiceComplete } from '@/features/character-creation/characterClass'

const fighter: CharacterClass = {
  id: 'class.fighter',
  name: 'Fighter',
  baseHitPoints: 10,
  keyAbilityOptions: ['Strength', 'Dexterity'],
  spellTradition: null,
  rules: [],
  deferredDependencies: [],
}

describe('class choice', () => {
  it('accepts an allowed key ability', () => {
    expect(isCharacterClassChoiceComplete(fighter, 'Strength')).toBe(true)
  })

  it('rejects a key ability outside the class options', () => {
    expect(isCharacterClassChoiceComplete(fighter, 'Wisdom')).toBe(false)
  })

  it('requires both class and key ability', () => {
    expect(isCharacterClassChoiceComplete(null, 'Strength')).toBe(false)
    expect(isCharacterClassChoiceComplete(fighter, null)).toBe(false)
  })
})
