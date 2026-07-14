import { describe, expect, it } from 'vitest'
import type { BardMuse, CharacterClass } from './api'
import { isBardMuseChoiceComplete } from './bardMuse'

const bard = { id: 'class.bard' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass
const enigma = { id: 'bard_muse.enigma' } as BardMuse

describe('isBardMuseChoiceComplete', () => {
  it('requires a Muse only for Bard', () => {
    expect(isBardMuseChoiceComplete(bard, null)).toBe(false)
    expect(isBardMuseChoiceComplete(bard, enigma)).toBe(true)
    expect(isBardMuseChoiceComplete(fighter, null)).toBe(true)
    expect(isBardMuseChoiceComplete(fighter, enigma)).toBe(false)
  })
})
