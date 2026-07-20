import { describe, expect, it } from 'vitest'
import {
  getAvailableClassFeatOptions,
  getRequiredClassFeatChoiceSlots,
  isClassFeatChoiceComplete,
} from './classFeatChoices'
import type { ArcaneSchool, ArcaneThesis, CharacterClass, FeatDefinition } from './api'

const wizard = { id: 'class.wizard', name: 'Wizard', rules: [] } as unknown as CharacterClass
const school = {
  benefits: [{ id: 'school.extra', kind: 'ExtraClassFeat', name: 'Extra Feat' }],
} as unknown as ArcaneSchool
const thesis = {
  effects: [{ id: 'thesis.spellshape', kind: 'FirstLevelSpellshapeFeatChoice', name: 'Spellshape' }],
} as unknown as ArcaneThesis
const feats = [
  { id: 'feat.reach_spell', name: 'Reach Spell', level: 1, traits: ['Wizard'] },
  { id: 'feat.familiar', name: 'Familiar', level: 1, traits: ['Wizard'] },
] as FeatDefinition[]

describe('class feat choices', () => {
  it('builds independent Wizard school and thesis slots', () => {
    const slots = getRequiredClassFeatChoiceSlots(wizard, school, thesis)

    expect(slots.map((slot) => slot.sourceId)).toEqual(['thesis.spellshape', 'school.extra'])
    expect(getAvailableClassFeatOptions(wizard, feats, slots[0]!, [])).toEqual([feats[0]])
  })

  it('requires every slot and rejects duplicate feat selections', () => {
    const slots = getRequiredClassFeatChoiceSlots(wizard, school, thesis)

    expect(isClassFeatChoiceComplete(slots, [
      { sourceId: 'thesis.spellshape', featId: 'feat.reach_spell' },
      { sourceId: 'school.extra', featId: 'feat.familiar' },
    ])).toBe(true)
    expect(isClassFeatChoiceComplete(slots, [
      { sourceId: 'thesis.spellshape', featId: 'feat.reach_spell' },
      { sourceId: 'school.extra', featId: 'feat.reach_spell' },
    ])).toBe(false)
  })
})
