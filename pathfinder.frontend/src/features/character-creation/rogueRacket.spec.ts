import { describe, expect, it } from 'vitest'
import type { RogueRacket, Skill } from './api'
import {
  createRogueTrainingChoices,
  getRogueKeyAbilities,
  isRogueRacketChoiceComplete,
} from './rogueRacket'

const skills = ['stealth', 'thievery', 'athletics'].map((id) => ({
  id: `skill.${id}`,
  name: id,
  keyAbility: 'Dexterity',
})) as Skill[]
const thief: RogueRacket = {
  id: 'rogue_racket.thief',
  name: 'Thief',
  alternativeKeyAbility: null,
  skillGrants: [
    { id: 'rogue_racket.thief.skill.thievery', targetId: 'skill.thievery', options: [], requiresChoice: false },
  ],
  proficiencyGrants: [],
  effects: [],
  deferredDependencies: [],
}

describe('rogue racket selection', () => {
  it('keeps Dexterity and adds only the racket alternative', () => {
    expect(getRogueKeyAbilities(thief)).toEqual(['Dexterity'])
    expect(getRogueKeyAbilities({ ...thief, alternativeKeyAbility: 'Strength' })).toEqual([
      'Dexterity',
      'Strength',
    ])
  })

  it('requires a replacement for duplicate background training', () => {
    const choices = createRogueTrainingChoices(thief)
    expect(isRogueRacketChoiceComplete(thief, 'Dexterity', choices, ['skill.thievery'], skills)).toBe(false)
    choices.find((choice) => choice.grantId.includes('thievery'))!.replacementSkillId = 'skill.athletics'
    expect(isRogueRacketChoiceComplete(thief, 'Dexterity', choices, ['skill.thievery'], skills)).toBe(true)
  })
})
