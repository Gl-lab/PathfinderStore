import { describe, expect, it } from 'vitest'
import type { CharacterClass, ClericDoctrine, ClericDomain, Deity } from './api'
import { getAvailableClericDomains, hasInitialClericFocusPool, isClericDomainChoiceComplete } from './clericDomain'

const cleric = { id: 'class.cleric' } as CharacterClass
const fighter = { id: 'class.fighter' } as CharacterClass
const cloistered = { id: 'cleric_doctrine.cloistered' } as ClericDoctrine
const warpriest = { id: 'cleric_doctrine.warpriest' } as ClericDoctrine
const domain = (id: string, spellId: string): ClericDomain => ({
  id,
  initialFocusSpell: { id: spellId, kind: 'Focus' },
  initialFocusPool: {
    maximumFocusPoints: 1,
    focusSpell: { id: spellId, kind: 'Focus' },
    sourceGrantId: 'cleric_doctrine.cloistered.effect.domain_initiate',
  },
}) as ClericDomain
const might = domain('domain.might', 'spell.athletic_rush')
const truth = domain('domain.truth', 'spell.word_of_truth')
const iomedae = { primaryDomainIds: ['domain.might'] } as Deity

describe('Cleric domain choice', () => {
  it('offers only primary domains of the selected deity', () => {
    expect(getAvailableClericDomains(iomedae, [might, truth])).toEqual([might])
  })

  it('requires a primary domain for Cloistered Cleric', () => {
    expect(isClericDomainChoiceComplete(cleric, cloistered, iomedae, might)).toBe(true)
    expect(isClericDomainChoiceComplete(cleric, cloistered, iomedae, truth)).toBe(false)
    expect(isClericDomainChoiceComplete(cleric, cloistered, iomedae, null)).toBe(false)
  })

  it('forbids a domain for Warpriest and non-Cleric classes', () => {
    expect(isClericDomainChoiceComplete(cleric, warpriest, iomedae, null)).toBe(true)
    expect(isClericDomainChoiceComplete(cleric, warpriest, iomedae, might)).toBe(false)
    expect(isClericDomainChoiceComplete(fighter, null, null, might)).toBe(false)
  })

  it('requires a server-provided one-point focus package matching the domain spell', () => {
    expect(hasInitialClericFocusPool(might)).toBe(true)
    const invalid = domain('domain.might', 'spell.athletic_rush')
    invalid.initialFocusPool.focusSpell.id = 'spell.fire_ray'
    expect(hasInitialClericFocusPool(invalid)).toBe(false)
    expect(isClericDomainChoiceComplete(cleric, cloistered, iomedae, invalid)).toBe(false)
  })
})
