import { describe, expect, it } from 'vitest'
import type { CharacterClass, ClericDoctrine } from './api'
import {
  getEffectiveClassProficiencies,
  isClericDoctrineChoiceComplete,
} from './clericDoctrine'

const cleric: CharacterClass = {
  id: 'class.cleric',
  name: 'Cleric',
  baseHitPoints: 8,
  keyAbilityOptions: ['Wisdom'],
  initialProficiencies: [
    {
      targetId: 'proficiency.save.fortitude',
      name: 'Fortitude',
      category: 'SavingThrow',
      rank: 'Trained',
      sourceGrantId: 'class.cleric.initial_proficiencies',
    },
  ],
  initialSkillGrants: [],
  additionalSkillCountBase: 2,
  spellTradition: 'Divine',
  rules: [],
  deferredDependencies: [],
}

const warpriest: ClericDoctrine = {
  id: 'cleric_doctrine.warpriest',
  name: 'Warpriest',
  proficiencyGrants: [
    {
      targetId: 'proficiency.save.fortitude',
      name: 'Fortitude',
      category: 'SavingThrow',
      rank: 'Expert',
      sourceGrantId: 'cleric_doctrine.warpriest.proficiency.fortitude',
    },
    {
      targetId: 'proficiency.defense.medium_armor',
      name: 'Medium Armor',
      category: 'Defense',
      rank: 'Trained',
      sourceGrantId: 'cleric_doctrine.warpriest.proficiency.medium_armor',
    },
  ],
  effects: [],
  deferredDependencies: [],
}

describe('cleric doctrine flow', () => {
  it('requires a doctrine only for Cleric', () => {
    expect(isClericDoctrineChoiceComplete(cleric, null)).toBe(false)
    expect(isClericDoctrineChoiceComplete(cleric, warpriest)).toBe(true)
    expect(isClericDoctrineChoiceComplete({ ...cleric, id: 'class.fighter' }, null)).toBe(true)
  })

  it('uses the highest rank and adds doctrine targets', () => {
    const result = getEffectiveClassProficiencies(cleric, warpriest)

    expect(result).toHaveLength(2)
    const fortitude = result.find((item) => item.targetId === 'proficiency.save.fortitude')
    expect(fortitude?.rank).toBe('Expert')
    expect(fortitude?.sourceGrantIds).toEqual([
      'class.cleric.initial_proficiencies',
      'cleric_doctrine.warpriest.proficiency.fortitude',
    ])
    expect(result.some((item) => item.targetId === 'proficiency.defense.medium_armor')).toBe(true)
  })
})
