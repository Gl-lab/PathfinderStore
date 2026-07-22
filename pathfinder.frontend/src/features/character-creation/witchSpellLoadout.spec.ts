import { describe, expect, it } from 'vitest'
import type { CharacterClass, WitchPatron, WitchSpellOptions } from './api'
import { getWitchSpellLoadoutProgress, isWitchSpellLoadoutComplete } from './witchSpellLoadout'

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
  it('reports progress for every required Witch spell selection', () => {
    const progress = getWitchSpellLoadoutProgress(
      witch,
      patron,
      null,
      cantrips.slice(0, 1),
      spells.slice(0, 3),
      cantrips.slice(0, 1),
      ['spell-0', 'spell-1'],
      'focus-hex',
      options,
    )

    expect(progress).toEqual({
      patronReady: true,
      familiarCantrips: { selected: 1, required: 10, isComplete: false },
      familiarSpells: { selected: 3, required: 5, isComplete: false },
      preparedCantrips: { selected: 1, required: 5, isComplete: false },
      preparedSpellSlots: { selected: 2, required: 2, isComplete: true },
      focusHexSelected: true,
      isComplete: false,
    })
    expect(
      [
        progress.familiarCantrips,
        progress.familiarSpells,
        progress.preparedCantrips,
        progress.preparedSpellSlots,
      ].filter((selection) => !selection.isComplete),
    ).toHaveLength(3)
  })

  it('validates familiar storage and allows duplicate prepared spells', () => {
    expect(
      isWitchSpellLoadoutComplete(
        witch,
        patron,
        null,
        cantrips,
        spells,
        cantrips.slice(0, 5),
        ['patron-spell', 'patron-spell'],
        'focus-hex',
        options,
      ),
    ).toBe(true)
  })

  it('rejects preparation outside familiar knowledge', () => {
    expect(
      isWitchSpellLoadoutComplete(
        witch,
        patron,
        null,
        cantrips,
        spells,
        cantrips.slice(0, 5),
        ['unknown', 'spell-1'],
        'focus-hex',
        options,
      ),
    ).toBe(false)
  })
})
