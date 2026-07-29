import { describe, expect, it } from 'vitest'
import type { AdminItemDefinition } from './api'
import { filterDefinitions, hasActiveFilters, resetFilters } from './filters'

const definitions = [
  {
    itemDefinitionId: 1,
    key: 'equipment.longsword',
    scope: 'Global',
    campaignId: null,
    createdAtUtc: '2026-07-27T10:00:00Z',
    revisions: [
      {
        itemRevisionId: 1,
        revisionNumber: 1,
        name: 'Длинный меч',
        level: 0,
        priceInCopperPieces: 100,
        bulk: 1,
        primaryCategory: 'Weapon',
        rarity: 'Common',
        status: 'Published',
        createdAtUtc: '2026-07-27T10:00:00Z',
        publishedAtUtc: '2026-07-27T11:00:00Z',
        retiredAtUtc: null,
      },
    ],
  },
  {
    itemDefinitionId: 2,
    key: 'campaign.tomb-blade',
    scope: 'Campaign',
    campaignId: 42,
    createdAtUtc: '2026-07-27T10:00:00Z',
    revisions: [
      {
        itemRevisionId: 2,
        revisionNumber: 1,
        name: 'Клинок гробницы',
        level: 3,
        priceInCopperPieces: 400,
        bulk: 1,
        primaryCategory: 'Weapon',
        rarity: 'Uncommon',
        status: 'Draft',
        createdAtUtc: '2026-07-27T10:00:00Z',
        publishedAtUtc: null,
        retiredAtUtc: null,
      },
    ],
  },
] as AdminItemDefinition[]

describe('filters', () => {
  it('matches search by key and by revision name case-insensitively', () => {
    expect(
      filterDefinitions(definitions, { search: 'LONGSWORD', status: 'All' }).map(
        (item) => item.itemDefinitionId,
      ),
    ).toEqual([1])
    expect(
      filterDefinitions(definitions, { search: 'клинок', status: 'All' }).map(
        (item) => item.itemDefinitionId,
      ),
    ).toEqual([2])
  })

  it('keeps definitions having at least one revision with the requested status', () => {
    expect(
      filterDefinitions(definitions, { search: '', status: 'Draft' }).map(
        (item) => item.itemDefinitionId,
      ),
    ).toEqual([2])
    expect(filterDefinitions(definitions, { search: '', status: 'Retired' })).toEqual([])
  })

  it('combines search and status filters', () => {
    expect(filterDefinitions(definitions, { search: 'клинок', status: 'Published' })).toEqual([])
  })

  it('detects active filters and resets them', () => {
    expect(hasActiveFilters({ search: '', status: 'All' })).toBe(false)
    expect(hasActiveFilters({ search: 'x', status: 'All' })).toBe(true)
    expect(hasActiveFilters({ search: '', status: 'Draft' })).toBe(true)
    expect(hasActiveFilters({ search: '', status: 'All', scope: 'Global' })).toBe(true)
    expect(hasActiveFilters({ search: '', status: 'All', scope: 'All' })).toBe(false)
    expect(resetFilters()).toEqual({ search: '', status: 'All' })
  })
})
