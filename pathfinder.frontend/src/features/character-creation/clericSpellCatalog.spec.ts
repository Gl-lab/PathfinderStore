import { describe, expect, it } from 'vitest'
import type { SpellDefinition } from './api'
import { groupClericSpellCatalog } from './clericSpellCatalog'

function spell(id: string, kind: SpellDefinition['kind']): SpellDefinition {
  return { id, kind } as SpellDefinition
}

describe('Cleric spell catalog', () => {
  it('groups server definitions without deriving spell eligibility', () => {
    const cantrip = spell('spell.guidance', 'Cantrip')
    const preparedSpell = spell('spell.heal', 'Spell')
    const focusSpell = spell('spell.fire_ray', 'Focus')

    expect(groupClericSpellCatalog([focusSpell, cantrip, preparedSpell])).toEqual({
      Cantrip: [cantrip],
      Spell: [preparedSpell],
      Focus: [focusSpell],
    })
  })
})
