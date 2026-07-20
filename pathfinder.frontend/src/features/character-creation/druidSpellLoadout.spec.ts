import { describe, expect, it } from 'vitest'
import type { CharacterClass, DruidSpellOptions } from './api'
import { isDruidSpellLoadoutComplete } from './druidSpellLoadout'

const options = {
  cantrips: ['a', 'b', 'c', 'd', 'e'].map((id) => ({ id })),
  rankOneSpells: ['heal', 'fear'].map((id) => ({ id })),
} as DruidSpellOptions
const druid = { id: 'class.druid' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass

describe('Druid spell loadout', () => {
  it('accepts duplicate prepared spells but requires unique cantrips', () => {
    expect(isDruidSpellLoadoutComplete(druid, ['a', 'b', 'c', 'd', 'e'], ['heal', 'heal'], options)).toBe(true)
    expect(isDruidSpellLoadoutComplete(druid, ['a', 'a', 'c', 'd', 'e'], ['heal', 'fear'], options)).toBe(false)
  })

  it('requires empty Druid selections for another class', () => {
    expect(isDruidSpellLoadoutComplete(fighter, [], [], options)).toBe(true)
    expect(isDruidSpellLoadoutComplete(fighter, ['a'], [], options)).toBe(false)
  })
})
