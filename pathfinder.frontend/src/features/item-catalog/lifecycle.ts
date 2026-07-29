import type { AdminItemDefinition, AdminItemRevision } from './api'

export type CatalogMode = 'campaign' | 'global'

export function canPublish(revision: AdminItemRevision): boolean {
  return revision.status === 'Draft'
}

export function canRetire(revision: AdminItemRevision): boolean {
  return revision.status === 'Published'
}

export function canManageDefinition(definition: AdminItemDefinition, mode: CatalogMode): boolean {
  return mode === 'campaign' ? definition.scope === 'Campaign' : definition.scope === 'Global'
}

export function publishConsequence(definition: AdminItemDefinition): AdminItemRevision | null {
  return definition.revisions.find((revision) => revision.status === 'Published') ?? null
}

export function latestDraft(definition: AdminItemDefinition): AdminItemRevision | null {
  return (
    definition.revisions
      .filter((revision) => revision.status === 'Draft')
      .sort((left, right) => right.revisionNumber - left.revisionNumber)[0] ?? null
  )
}

export function latestRevision(definition: AdminItemDefinition): AdminItemRevision | null {
  return (
    [...definition.revisions].sort(
      (left, right) => right.revisionNumber - left.revisionNumber,
    )[0] ?? null
  )
}

export type NewRevisionBlockReason = 'draftExists' | 'readOnly' | null

export function newRevisionBlockReason(
  definition: AdminItemDefinition,
  mode: CatalogMode,
): NewRevisionBlockReason {
  if (!canManageDefinition(definition, mode)) return 'readOnly'
  if (latestDraft(definition)) return 'draftExists'
  return null
}

export function canConfigure(revision: AdminItemRevision, mode: CatalogMode): boolean {
  return mode === 'campaign' && revision.status === 'Published'
}
