import type { AdminItemDefinition, ItemCatalogScopeFilter, ItemRevisionStatus } from './api'

export interface CatalogFilters {
  search: string
  status: ItemRevisionStatus | 'All'
  scope?: ItemCatalogScopeFilter
}

function matchesSearch(definition: AdminItemDefinition, search: string): boolean {
  const normalized = search.trim().toLowerCase()
  if (!normalized) return true
  if (definition.key.toLowerCase().includes(normalized)) return true
  return definition.revisions.some((revision) => revision.name.toLowerCase().includes(normalized))
}

function matchesStatus(
  definition: AdminItemDefinition,
  status: ItemRevisionStatus | 'All',
): boolean {
  if (status === 'All') return true
  return definition.revisions.some((revision) => revision.status === status)
}

export function filterDefinitions(
  definitions: AdminItemDefinition[],
  filters: CatalogFilters,
): AdminItemDefinition[] {
  return definitions.filter(
    (definition) =>
      matchesSearch(definition, filters.search) && matchesStatus(definition, filters.status),
  )
}

export function hasActiveFilters(filters: CatalogFilters): boolean {
  return (
    Boolean(filters.search.trim()) ||
    filters.status !== 'All' ||
    (filters.scope !== undefined && filters.scope !== 'All')
  )
}

export function resetFilters(): CatalogFilters {
  return { search: '', status: 'All' }
}
