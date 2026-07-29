import type { AdminItemDefinition } from './api'
import type { CatalogFilters } from './filters'
import { filterDefinitions, hasActiveFilters } from './filters'

export type CatalogEmptyReason = 'noDefinitions' | 'noMatches' | null

export function catalogEmptyReason(
  definitions: AdminItemDefinition[],
  filters: CatalogFilters,
): CatalogEmptyReason {
  if (definitions.length === 0) {
    return hasActiveFilters(filters) ? 'noMatches' : 'noDefinitions'
  }
  if (filterDefinitions(definitions, filters).length === 0 && hasActiveFilters(filters)) {
    return 'noMatches'
  }
  return null
}

export interface CommerceCatalogRevisionLike {
  configurations: unknown[]
}

export type CommerceCatalogEmptyReason =
  'noPublishedRevisions' | 'noConfigurations' | 'noMatches' | null

export function commerceCatalogEmptyReason(
  revisions: CommerceCatalogRevisionLike[],
  search = '',
): CommerceCatalogEmptyReason {
  if (revisions.length === 0) {
    return search.trim() ? 'noMatches' : 'noPublishedRevisions'
  }
  if (revisions.every((revision) => revision.configurations.length === 0)) {
    return 'noConfigurations'
  }
  return null
}
