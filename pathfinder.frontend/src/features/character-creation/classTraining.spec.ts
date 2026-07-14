import { describe, expect, it } from 'vitest'
import type { CharacterClass, Skill } from './api'
import {
  createAdditionalClassTrainingChoices,
  createClassSkillGrantChoices,
  getAdditionalClassTrainingCount,
  getAvailableClassTrainingSkills,
  getCustomLoreId,
  isClassTrainingComplete,
} from './classTraining'

const skills: Skill[] = [
  { id: 'skill.arcana', name: 'Arcana', keyAbility: 'Intelligence' },
  { id: 'skill.athletics', name: 'Athletics', keyAbility: 'Strength' },
  { id: 'skill.crafting', name: 'Crafting', keyAbility: 'Intelligence' },
]
const wizard = {
  id: 'class.wizard',
  initialSkillGrants: [{ id: 'class.wizard.skill.arcana', skillOptions: ['skill.arcana'] }],
  additionalSkillCountBase: 2,
} as CharacterClass

describe('class training', () => {
  it('uses final Intelligence modifier for additional choices', () => {
    expect(getAdditionalClassTrainingCount(wizard, 16)).toBe(5)
  })

  it('requires a replacement when an earlier package already trained the fixed skill', () => {
    const grants = createClassSkillGrantChoices(wizard)
    grants[0].replacementTarget = { skillId: 'skill.athletics', customLoreTopic: null }
    const additional = [
      { skillId: 'skill.crafting', customLoreTopic: null },
      { skillId: null, customLoreTopic: 'Warfare' },
    ]

    expect(isClassTrainingComplete(wizard, grants, additional, 2, ['skill.arcana'], skills)).toBe(true)
  })

  it('rejects duplicate skill choices', () => {
    const grants = createClassSkillGrantChoices(wizard)
    const additional = [
      { skillId: 'skill.athletics', customLoreTopic: null },
      { skillId: 'skill.athletics', customLoreTopic: null },
    ]

    expect(isClassTrainingComplete(wizard, grants, additional, 2, [], skills)).toBe(false)
  })

  it('hides skills trained by earlier packages and other class choices', () => {
    const grants = createClassSkillGrantChoices(wizard)
    const currentChoice = { skillId: 'skill.crafting', customLoreTopic: null }
    const additional = [
      currentChoice,
      { skillId: 'skill.athletics', customLoreTopic: null },
    ]

    expect(getAvailableClassTrainingSkills(
      skills,
      wizard,
      grants,
      additional,
      [],
      currentChoice,
    )).toEqual([skills[2]])
  })

  it('creates the requested number of empty slots', () => {
    expect(createAdditionalClassTrainingChoices(3)).toHaveLength(3)
  })

  it('uses the server-compatible identity for custom Lore', () => {
    expect(getCustomLoreId('  Ancient   Forest Lore ')).toBe('lore.custom.ancient_forest')
  })

  it('rejects a custom Lore topic that duplicates an existing Lore', () => {
    const grants = createClassSkillGrantChoices(wizard)
    const additional = [
      { skillId: 'skill.athletics', customLoreTopic: null },
      { skillId: null, customLoreTopic: 'Ancient Forest' },
    ]

    expect(isClassTrainingComplete(
      wizard,
      grants,
      additional,
      2,
      ['lore.custom.ancient_forest'],
      skills,
    )).toBe(false)
  })
})
