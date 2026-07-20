import { describe, expect, it } from 'vitest'
import type { CharacterClass, WitchPatron, WitchSpellOptions } from './api'
import { isWitchSpellLoadoutComplete } from './witchSpellLoadout'

const cantrips = Array.from({ length: 10 }, (_, index) => `cantrip-${index}`)
const spells = Array.from({ length: 5 }, (_, index) => `spell-${index}`)
const options = {
  cantrips: cantrips.map((id) => ({ id })),
  rankOneSpells: spells.map((id) => ({ id })),
} as WitchSpellOptions
const witch = { id: 'class.witch' } as CharacterClass
const patron = {
  benefits: [{ id: 'patron-spell', kind: 'FamiliarSpell' }],
  initialFocusHexOptions: [{ id: 'focus-hex' }],
} as WitchPatron

describe('Witch spell loadout', () => {
  it('validates familiar storage and allows duplicate prepared spells', () => {
    expect(isWitchSpellLoadoutComplete(
      witch, patron, null, cantrips, spells, cantrips.slice(0, 5), ['patron-spell', 'patron-spell'], 'focus-hex', options,
    )).toBe(true)
  })

  it('rejects preparation outside familiar knowledge', () => {
    expect(isWitchSpellLoadoutComplete(
      witch, patron, null, cantrips, spells, cantrips.slice(0, 5), ['unknown', 'spell-1'], 'focus-hex', options,
    )).toBe(false)
  })
})
