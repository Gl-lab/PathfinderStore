import { describe, expect, it } from 'vitest'
import type { CharacterClass, ClericAvailableSpell, ClericSpellOptions } from './api'
import { isClericSpellLoadoutComplete } from './clericSpellLoadout'

const cleric = { id: 'class.cleric' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass
const option = (id: string): ClericAvailableSpell => ({ spell: { id } }) as ClericAvailableSpell
const options: ClericSpellOptions = {
  cantrips: ['a', 'b', 'c', 'd', 'e'].map(option),
  rankOneSpells: ['heal', 'sure-strike'].map(option),
}

describe('Cleric spell loadout', () => {
  it('requires five unique server-provided cantrips and two prepared slots', () => {
    expect(isClericSpellLoadoutComplete(cleric, ['a', 'b', 'c', 'd', 'e'], ['heal', 'heal'], options)).toBe(true)
    expect(isClericSpellLoadoutComplete(cleric, ['a', 'a', 'c', 'd', 'e'], ['heal', 'heal'], options)).toBe(false)
    expect(isClericSpellLoadoutComplete(cleric, ['a', 'b', 'c', 'd', 'x'], ['heal', 'heal'], options)).toBe(false)
    expect(isClericSpellLoadoutComplete(cleric, ['a', 'b', 'c', 'd', 'e'], ['heal', null], options)).toBe(false)
  })

  it('allows repeated prepared spells and forbids loadout for another class', () => {
    expect(isClericSpellLoadoutComplete(cleric, ['a', 'b', 'c', 'd', 'e'], ['heal', 'heal'], options)).toBe(true)
    expect(isClericSpellLoadoutComplete(fighter, [], [], options)).toBe(true)
    expect(isClericSpellLoadoutComplete(fighter, ['a'], [], options)).toBe(false)
  })
})
