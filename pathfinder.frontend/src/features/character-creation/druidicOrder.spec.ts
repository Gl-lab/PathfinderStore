import { describe, expect, it } from 'vitest'
import type { CharacterClass, DruidicOrder } from './api'
import {
  isDruidicOrderChoiceComplete,
  withDruidicOrderSkillGrant,
} from './druidicOrder'

const druid = { id: 'class.druid', initialSkillGrants: [] } as unknown as CharacterClass
const fighter = { id: 'class.fighter', initialSkillGrants: [] } as unknown as CharacterClass
const animal = {
  id: 'druidic_order.animal',
  skillGrant: { id: 'druidic_order.animal.skill.order', skillOptions: ['skill.athletics'] },
} as DruidicOrder

describe('Druidic Order choice', () => {
  it('requires a catalog entry only for Druid', () => {
    expect(isDruidicOrderChoiceComplete(druid, animal)).toBe(true)
    expect(isDruidicOrderChoiceComplete(druid, null)).toBe(false)
    expect(isDruidicOrderChoiceComplete(fighter, animal)).toBe(false)
    expect(isDruidicOrderChoiceComplete(fighter, null)).toBe(true)
  })

  it('adds the Order skill grant only to Druid training', () => {
    expect(withDruidicOrderSkillGrant(druid, animal)?.initialSkillGrants).toEqual([
      animal.skillGrant,
    ])
    expect(withDruidicOrderSkillGrant(fighter, animal)).toBe(fighter)
  })
})
