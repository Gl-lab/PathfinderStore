import { describe, expect, it } from 'vitest'
import type { AbilityCode } from '@/features/characters/api'
import type { Background } from '@/features/character-creation/api'
import {
  createBackgroundTrainingChoices,
  getBackgroundTrainingLabels,
  getBackgroundFreeBoostOptions,
  isBackgroundChoiceComplete,
  isBackgroundTrainingComplete,
} from '@/features/character-creation/background'

const acrobat: Background = {
  id: 'background.acrobat',
  name: 'Acrobat',
  restrictedBoostOptions: ['Strength', 'Dexterity'],
  freeBoostCount: 1,
  grants: [],
}

const hermit: Background = {
  id: 'background.hermit',
  name: 'Hermit',
  restrictedBoostOptions: ['Constitution', 'Intelligence'],
  freeBoostCount: 1,
  grants: [
    {
      id: 'background.hermit.skill',
      kind: 'SkillTraining',
      name: 'Hermit skill',
      summary: 'Choose Nature or Occultism.',
      requiresChoice: true,
      allowsCustomLore: false,
      targetId: null,
      options: [
        { id: 'skill.nature', name: 'Nature' },
        { id: 'skill.occultism', name: 'Occultism' },
      ],
      deferredDependencies: [],
    },
    {
      id: 'background.hermit.lore',
      kind: 'LoreTraining',
      name: 'Terrain Lore',
      summary: 'Choose terrain Lore.',
      requiresChoice: true,
      allowsCustomLore: true,
      targetId: null,
      options: [],
      deferredDependencies: [],
    },
  ],
}

const martialDisciple: Background = {
  id: 'background.martial_disciple',
  name: 'Martial Disciple',
  restrictedBoostOptions: ['Strength', 'Dexterity'],
  freeBoostCount: 1,
  grants: [
    {
      id: 'background.martial_disciple.feat',
      kind: 'SkillFeat',
      name: 'Martial discipline feat',
      summary: 'Choose Cat Fall or Quick Jump.',
      requiresChoice: true,
      allowsCustomLore: false,
      targetId: null,
      options: [
        { id: 'skill_feat.cat_fall', name: 'Cat Fall' },
        { id: 'skill_feat.quick_jump', name: 'Quick Jump' },
      ],
      deferredDependencies: [],
    },
  ],
}

describe('background choice', () => {
  it('accepts one restricted and one different free boost', () => {
    expect(isBackgroundChoiceComplete(acrobat, 'Dexterity', 'Intelligence')).toBe(true)
  })

  it('rejects a restricted boost outside the background options', () => {
    expect(isBackgroundChoiceComplete(acrobat, 'Wisdom', 'Intelligence')).toBe(false)
  })

  it('rejects duplicate boosts from the same background package', () => {
    expect(isBackgroundChoiceComplete(acrobat, 'Dexterity', 'Dexterity')).toBe(false)
  })

  it('removes the restricted boost from free boost options', () => {
    const abilities: AbilityCode[] = ['Strength', 'Dexterity', 'Intelligence']

    expect(getBackgroundFreeBoostOptions(abilities, 'Dexterity')).toEqual([
      'Strength',
      'Intelligence',
    ])
  })

  it('requires every skill and Lore choice', () => {
    const choices = createBackgroundTrainingChoices(hermit)

    expect(isBackgroundTrainingComplete(hermit, choices)).toBe(false)
    choices[0].targetId = 'skill.nature'
    choices[1].customLoreTopic = 'Forest'

    expect(isBackgroundTrainingComplete(hermit, choices)).toBe(true)
    expect(getBackgroundTrainingLabels(hermit, choices)).toEqual(['Nature', 'Forest Lore'])

    choices[1].customLoreTopic = 'Forest Lore'
    expect(getBackgroundTrainingLabels(hermit, choices)).toEqual(['Nature', 'Forest Lore'])
  })

  it('rejects a target outside the grant options', () => {
    const choices = createBackgroundTrainingChoices(hermit)
    choices[0].targetId = 'skill.arcana'
    choices[1].customLoreTopic = 'Forest'

    expect(isBackgroundTrainingComplete(hermit, choices)).toBe(false)
  })

  it('requires and labels a background skill feat choice', () => {
    const choices = createBackgroundTrainingChoices(martialDisciple)

    expect(choices).toHaveLength(1)
    expect(isBackgroundTrainingComplete(martialDisciple, choices)).toBe(false)

    choices[0].targetId = 'skill_feat.quick_jump'

    expect(isBackgroundTrainingComplete(martialDisciple, choices)).toBe(true)
    expect(getBackgroundTrainingLabels(martialDisciple, choices)).toEqual(['Quick Jump'])
  })
})
