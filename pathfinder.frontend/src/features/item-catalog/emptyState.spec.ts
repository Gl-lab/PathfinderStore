import { describe, expect, it } from 'vitest'
import type { AdminItemDefinition } from './api'
import { catalogEmptyReason, commerceCatalogEmptyReason } from './emptyState'

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
      name: 'Longsword',
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
} as AdminItemDefinition

describe('emptyState', () => {
  it('reports noDefinitions only when the catalog is empty without filters', () => {
    expect(catalogEmptyReason([], { search: '', status: 'All' })).toBe('noDefinitions')
    expect(catalogEmptyReason([definition], { search: '', status: 'All' })).toBeNull()
  })

  it('reports noMatches for an empty server result when filters are active', () => {
    expect(catalogEmptyReason([], { search: 'dagegr', status: 'All' })).toBe('noMatches')
    expect(catalogEmptyReason([], { search: '', status: 'Draft' })).toBe('noMatches')
    expect(catalogEmptyReason([], { search: '', status: 'All', scope: 'Campaign' })).toBe(
      'noMatches',
    )
    expect(catalogEmptyReason([], { search: '', status: 'All', scope: 'All' })).toBe(
      'noDefinitions',
    )
  })

  it('reports noMatches when filters hide every definition', () => {
    expect(catalogEmptyReason([definition], { search: 'axe', status: 'All' })).toBe('noMatches')
    expect(catalogEmptyReason([definition], { search: '', status: 'Draft' })).toBe('noMatches')
    expect(catalogEmptyReason([definition], { search: 'long', status: 'All' })).toBeNull()
  })

  it('distinguishes commerce dialog empty reasons', () => {
    expect(commerceCatalogEmptyReason([])).toBe('noPublishedRevisions')
    expect(commerceCatalogEmptyReason([], 'dagger')).toBe('noMatches')
    expect(commerceCatalogEmptyReason([], '   ')).toBe('noPublishedRevisions')
    expect(commerceCatalogEmptyReason([{ configurations: [] }, { configurations: [] }])).toBe(
      'noConfigurations',
    )
    expect(commerceCatalogEmptyReason([{ configurations: [] }, { configurations: [1] }])).toBeNull()
  })
})
