import { describe, expect, it } from 'vitest'
import type { ArcaneThesis, CharacterClass } from './api'
import {
  formatArcaneThesisMilestones,
  isArcaneThesisChoiceComplete,
} from './arcaneThesis'

const wizard = { id: 'class.wizard' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass
const thesis = { id: 'arcane_thesis.spell_substitution' } as ArcaneThesis

describe('Arcane Thesis choice', () => {
  it('requires a Thesis only for Wizard', () => {
    expect(isArcaneThesisChoiceComplete(wizard, null)).toBe(false)
    expect(isArcaneThesisChoiceComplete(wizard, thesis)).toBe(true)
    expect(isArcaneThesisChoiceComplete(fighter, null)).toBe(true)
    expect(isArcaneThesisChoiceComplete(fighter, thesis)).toBe(false)
  })

  it('formats milestone levels in catalog order', () => {
    expect(formatArcaneThesisMilestones([1, 6, 12, 18])).toBe('1, 6, 12, 18')
  })
})
