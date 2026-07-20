import { describe, expect, it } from 'vitest'
import type { BardMuse, BardSpellOptions, CharacterClass } from './api'
import { getMuseGrantedSpellId, isBardSpellLoadoutComplete } from './bardSpellLoadout'

const bard = { id: 'class.bard' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass
const muse = {
  benefits: [{ id: 'spell.muse', kind: 'RepertoireSpell' }],
} as BardMuse
const options = {
  cantrips: ['a', 'b', 'c', 'd', 'e'].map((id) => ({ id })),
  rankOneSpells: ['one', 'two', 'spell.muse'].map((id) => ({ id })),
} as BardSpellOptions

describe('Bard spell loadout', () => {
  it('requires five unique cantrips and two unique non-Muse repertoire spells', () => {
    expect(isBardSpellLoadoutComplete(bard, muse, ['a', 'b', 'c', 'd', 'e'], ['one', 'two'], options)).toBe(true)
    expect(isBardSpellLoadoutComplete(bard, muse, ['a', 'a', 'c', 'd', 'e'], ['one', 'two'], options)).toBe(false)
    expect(isBardSpellLoadoutComplete(bard, muse, ['a', 'b', 'c', 'd', 'e'], ['one', 'one'], options)).toBe(false)
    expect(isBardSpellLoadoutComplete(bard, muse, ['a', 'b', 'c', 'd', 'e'], ['one', 'spell.muse'], options)).toBe(false)
  })

  it('forbids Bard choices for other classes', () => {
    expect(isBardSpellLoadoutComplete(fighter, null, [], [], options)).toBe(true)
    expect(isBardSpellLoadoutComplete(fighter, null, ['a'], [], options)).toBe(false)
  })

  it('resolves the Muse-granted repertoire entry', () => {
    expect(getMuseGrantedSpellId(muse)).toBe('spell.muse')
    expect(getMuseGrantedSpellId(null)).toBeNull()
  })
})
