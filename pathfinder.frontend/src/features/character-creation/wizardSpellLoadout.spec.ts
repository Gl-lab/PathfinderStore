import { describe, expect, it } from 'vitest'
import type { ArcaneSchool, CharacterClass, SpellDefinition } from './api'
import {
  isWizardSpellLoadoutComplete,
  reconcileWizardSpellLoadoutForSchool,
} from './wizardSpellLoadout'

const wizard = { id: 'class.wizard' } as CharacterClass
const formalSchool = {
  id: 'arcane_school.battle_magic',
  hasCurriculum: true,
  curriculumSpells: [
    { id: 'spell.shield', rank: 0 },
    { id: 'spell.telekinetic_projectile', rank: 0 },
    { id: 'spell.breathe_fire', rank: 1 },
    { id: 'spell.force_barrage', rank: 1 },
    { id: 'spell.mystic_armor', rank: 1 },
  ],
} as ArcaneSchool
const options = {
  cantrips: Array.from({ length: 10 }, (_, index) => ({ id: `cantrip.${index}` }) as SpellDefinition),
  rankOneSpells: Array.from({ length: 6 }, (_, index) => ({ id: `spell.${index}` }) as SpellDefinition),
}

describe('Wizard spell loadout', () => {
  it('accepts a complete formal-school loadout', () => {
    expect(isWizardSpellLoadoutComplete(
      wizard,
      formalSchool,
      options.cantrips.map((spell) => spell.id),
      options.rankOneSpells.slice(0, 5).map((spell) => spell.id),
      'spell.shield',
      ['spell.breathe_fire', 'spell.force_barrage'],
      options.cantrips.slice(0, 5).map((spell) => spell.id),
      ['spell.0', 'spell.0'],
      'spell.telekinetic_projectile',
      'spell.mystic_armor',
      options,
    )).toBe(true)
  })

  it('requires six base spells and no curriculum for Unified Magical Theory', () => {
    const unified = { ...formalSchool, hasCurriculum: false, curriculumSpells: [] } as ArcaneSchool
    expect(isWizardSpellLoadoutComplete(
      wizard,
      unified,
      options.cantrips.map((spell) => spell.id),
      options.rankOneSpells.map((spell) => spell.id),
      null,
      [],
      options.cantrips.slice(0, 5).map((spell) => spell.id),
      ['spell.0', 'spell.1'],
      null,
      null,
      options,
    )).toBe(true)
  })

  it('preserves compatible base choices and clears only incompatible school choices', () => {
    const result = reconcileWizardSpellLoadoutForSchool(formalSchool, {
      spellbookCantripIds: options.cantrips.map((spell) => spell.id),
      spellbookSpellIds: options.rankOneSpells.map((spell) => spell.id),
      curriculumCantripId: 'spell.old_cantrip',
      curriculumSpellIds: ['spell.force_barrage', 'spell.old_spell'],
      preparedCantripIds: options.cantrips.slice(0, 5).map((spell) => spell.id),
      preparedSpellIds: ['spell.0', 'spell.5'],
      preparedCurriculumCantripId: 'spell.telekinetic_projectile',
      preparedCurriculumSpellId: 'spell.old_spell',
    })

    expect(result.spellbookCantripIds).toHaveLength(10)
    expect(result.spellbookSpellIds).toHaveLength(5)
    expect(result.preparedSpellIds).toEqual(['spell.0', null])
    expect(result.curriculumSpellIds).toEqual(['spell.force_barrage'])
    expect(result.preparedCurriculumCantripId).toBe('spell.telekinetic_projectile')
    expect(result.preparedCurriculumSpellId).toBeNull()
  })
})
