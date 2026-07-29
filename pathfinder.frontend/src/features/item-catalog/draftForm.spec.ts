import { describe, expect, it } from 'vitest'
import type { AdminItemDefinition } from './api'
import {
  buildRulesRequest,
  createDraftFormModel,
  draftValidationErrors,
  prefillDraftFromDefinition,
  visibleSections,
} from './draftForm'

describe('draftForm', () => {
  it('maps every category to its rules sections', () => {
    expect(visibleSections('Weapon')).toEqual(['attacks', 'equipment'])
    expect(visibleSections('Armor')).toEqual(['armor', 'equipment'])
    expect(visibleSections('Shield')).toEqual(['shield', 'attacks', 'equipment'])
    expect(visibleSections('Consumable')).toEqual(['consumption', 'equipment'])
    expect(visibleSections('Ammunition')).toEqual(['consumption', 'attacks', 'equipment'])
    expect(visibleSections('Rune')).toEqual(['equipment'])
    expect(visibleSections('Tool')).toEqual(['equipment'])
    expect(visibleSections('Container')).toEqual(['equipment'])
    expect(visibleSections('OtherEquipment')).toEqual(['equipment'])
  })

  it('prunes hidden sections when the category changes', () => {
    const model = createDraftFormModel()
    model.category = 'Weapon'
    model.attacks[0].name = 'Blade'
    const weaponRules = buildRulesRequest(model)
    expect(weaponRules.attacks).toHaveLength(1)
    expect(weaponRules.armor).toBeUndefined()

    model.category = 'Armor'
    const armorRules = buildRulesRequest(model)
    expect(armorRules.attacks).toBeUndefined()
    expect(armorRules.armor).toBeDefined()
    expect(armorRules.equipment).toBeDefined()
  })

  it('includes charges and durability only when enabled', () => {
    const model = createDraftFormModel()
    expect(buildRulesRequest(model).charges).toBeUndefined()
    expect(buildRulesRequest(model).durability).toBeUndefined()

    model.chargesEnabled = true
    model.durabilityEnabled = true
    const rules = buildRulesRequest(model)
    expect(rules.charges).toBeDefined()
    expect(rules.durability).toBeDefined()
  })

  it('validates required fields and category coherence', () => {
    const model = createDraftFormModel()
    model.key = ''
    model.name = ''
    model.level = -1
    expect(draftValidationErrors(model, true)).toEqual(
      expect.arrayContaining([
        'itemCatalogUi.draft.validation.keyRequired',
        'itemCatalogUi.draft.validation.nameRequired',
        'itemCatalogUi.draft.validation.levelInvalid',
      ]),
    )

    const weapon = createDraftFormModel()
    weapon.key = 'equipment.sword'
    weapon.name = 'Sword'
    weapon.category = 'Weapon'
    weapon.attacks = []
    expect(draftValidationErrors(weapon, true)).toContain(
      'itemCatalogUi.draft.validation.attacksRequired',
    )

    const shield = createDraftFormModel()
    shield.key = 'equipment.shield'
    shield.name = 'Shield'
    shield.category = 'Shield'
    shield.durabilityEnabled = false
    expect(draftValidationErrors(shield, true)).toContain(
      'itemCatalogUi.draft.validation.shieldComponents',
    )

    shield.durabilityEnabled = true
    expect(draftValidationErrors(shield, true)).toEqual([])
  })

  it('skips key validation when the key is locked', () => {
    const model = createDraftFormModel()
    model.key = ''
    model.name = 'Item'
    expect(draftValidationErrors(model, false)).toEqual([])
  })

  it('prefills a new revision from the highest-numbered revision and locks the key', () => {
    const definition = {
      itemDefinitionId: 1,
      key: 'equipment.longsword',
      scope: 'Global',
      campaignId: null,
      createdAtUtc: '2026-07-27T10:00:00Z',
      revisions: [
        {
          itemRevisionId: 1,
          revisionNumber: 1,
          name: 'Old name',
          level: 1,
          priceInCopperPieces: 100,
          bulk: 1,
          primaryCategory: 'Weapon',
          rarity: 'Common',
          status: 'Retired',
          createdAtUtc: '2026-07-27T10:00:00Z',
          publishedAtUtc: null,
          retiredAtUtc: '2026-07-27T12:00:00Z',
        },
        {
          itemRevisionId: 2,
          revisionNumber: 2,
          name: 'New name',
          description: 'New description',
          level: 4,
          priceInCopperPieces: 600,
          bulk: 2,
          primaryCategory: 'Weapon',
          rarity: 'Uncommon',
          status: 'Published',
          createdAtUtc: '2026-07-27T11:00:00Z',
          publishedAtUtc: '2026-07-27T12:00:00Z',
          retiredAtUtc: null,
        },
      ],
    } as AdminItemDefinition

    const model = prefillDraftFromDefinition(definition)

    expect(model.key).toBe('equipment.longsword')
    expect(model.name).toBe('New name')
    expect(model.description).toBe('New description')
    expect(model.level).toBe(4)
    expect(model.priceInCopperPieces).toBe(600)
    expect(model.rarity).toBe('Uncommon')
  })
})
