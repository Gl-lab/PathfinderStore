import { describe, expect, it } from 'vitest'
import type { CharacterClass, WitchPatron } from './api'
import {
  getWitchPatronFamiliarSpellOptions,
  isWitchPatronChoiceComplete,
  withWitchPatron,
} from './witchPatron'

function characterClass(id: string): CharacterClass {
  return {
    id,
    name: id,
    baseHitPoints: 6,
    keyAbilityOptions: ['Intelligence'],
    initialProficiencies: [],
    initialSkillGrants: [],
    additionalSkillCountBase: 3,
    spellTradition: null,
    rules: [],
    deferredDependencies: [],
  }
}

const witch = characterClass('class.witch')
const fighter = characterClass('class.fighter')
const flamekeeper = {
  id: 'witch_patron.faiths_flamekeeper',
  spellTradition: 'Divine',
  skillGrant: { id: 'witch_patron.faiths_flamekeeper.skill.patron', skillOptions: ['skill.religion'] },
  benefits: [{ id: 'spell.command', kind: 'FamiliarSpell' }],
} as WitchPatron
const wilding = {
  ...flamekeeper,
  id: 'witch_patron.wilding_steward',
  benefits: [
    { id: 'spell.summon_animal', kind: 'FamiliarSpell' },
    { id: 'spell.summon_plant_or_fungus', kind: 'FamiliarSpell' },
  ],
} as WitchPatron

describe('Witch Patron choice', () => {
  it('derives a single familiar spell and requires an explicit multi-option choice', () => {
    expect(isWitchPatronChoiceComplete(witch, flamekeeper, null)).toBe(true)
    expect(isWitchPatronChoiceComplete(witch, flamekeeper, 'spell.command')).toBe(false)
    expect(isWitchPatronChoiceComplete(witch, wilding, null)).toBe(false)
    expect(isWitchPatronChoiceComplete(witch, wilding, 'spell.summon_animal')).toBe(true)
  })

  it('rejects Patron data for another class', () => {
    expect(isWitchPatronChoiceComplete(fighter, null, null)).toBe(true)
    expect(isWitchPatronChoiceComplete(fighter, flamekeeper, null)).toBe(false)
  })

  it('adds the Patron tradition and skill without mutating the catalog class', () => {
    const effective = withWitchPatron(witch, flamekeeper)

    expect(effective?.spellTradition).toBe('Divine')
    expect(effective?.initialSkillGrants).toEqual([flamekeeper.skillGrant])
    expect(witch.initialSkillGrants).toEqual([])
    expect(getWitchPatronFamiliarSpellOptions(wilding)).toHaveLength(2)
  })
})
