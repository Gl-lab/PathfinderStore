import { describe, expect, it } from 'vitest'
import type { PermanentUpgradeRequest } from './api'
import {
  configurationSummary,
  enumOptions,
  isDuplicateConfiguration,
  upgradeValidationErrors,
} from './options'

const striking: PermanentUpgradeRequest = {
  code: 'rune.striking',
  kind: 'StrikingRune',
  rank: 1,
  visibility: 'Public',
}

describe('options', () => {
  it('builds an option per enum member exactly once', () => {
    const options = enumOptions(['Tiny', 'Small', 'Medium'] as const, (value) => `L:${value}`)
    expect(options).toEqual([
      { value: 'Tiny', title: 'L:Tiny' },
      { value: 'Small', title: 'L:Small' },
      { value: 'Medium', title: 'L:Medium' },
    ])
  })

  it('formats configuration summary through the injected formatter', () => {
    const summary = configurationSummary(
      {
        size: 'Medium',
        materialType: 'ColdIron',
        materialGrade: 'Standard',
        permanentUpgrades: [striking],
      },
      (group, code) => `${group}.${code}`,
    )
    expect(summary).toBe('sizes.Medium · materials.ColdIron · grades.Standard · rune.striking')
    expect(
      configurationSummary({ size: 'Medium', materialType: 'Standard', materialGrade: 'Low' }),
    ).toBe('Medium · Standard · Low')
  })

  it('detects duplicates by shape including upgrades regardless of order', () => {
    const other: PermanentUpgradeRequest = {
      code: 'rune.potency',
      kind: 'WeaponPotencyRune',
      rank: 1,
      visibility: 'Public',
    }
    const existing = [
      {
        size: 'Medium',
        materialType: 'ColdIron',
        materialGrade: 'Standard',
        permanentUpgrades: [striking, other],
      },
    ]

    expect(
      isDuplicateConfiguration(existing, {
        size: 'Medium',
        materialType: 'ColdIron',
        materialGrade: 'Standard',
        permanentUpgrades: [other, striking],
      }),
    ).toBe(true)
    expect(
      isDuplicateConfiguration(existing, {
        size: 'Large',
        materialType: 'ColdIron',
        materialGrade: 'Standard',
        permanentUpgrades: [striking, other],
      }),
    ).toBe(false)
    expect(
      isDuplicateConfiguration(existing, {
        size: 'Medium',
        materialType: 'ColdIron',
        materialGrade: 'Standard',
        permanentUpgrades: [striking],
      }),
    ).toBe(false)
  })

  it('validates upgrade lists', () => {
    expect(upgradeValidationErrors([striking])).toEqual([])
    expect(upgradeValidationErrors([striking, { ...striking }])).toContain(
      'itemCatalogUi.configuration.validation.duplicateCodes',
    )
    expect(upgradeValidationErrors([{ ...striking, code: ' ' }])).toContain(
      'itemCatalogUi.configuration.validation.codeRequired',
    )
    expect(upgradeValidationErrors([{ ...striking, rank: 0 }])).toContain(
      'itemCatalogUi.configuration.validation.rankInvalid',
    )
    const seventeen = Array.from({ length: 17 }, (_, index) => ({
      ...striking,
      code: `rune.${index}`,
    }))
    expect(upgradeValidationErrors(seventeen)).toContain(
      'itemCatalogUi.configuration.validation.tooManyUpgrades',
    )
  })
})
