import type { AdminInventoryContainer, PublishedItemRevision } from './adminApi'

export interface CatalogConfigurationOption {
  value: number
  title: string
}

export type CatalogConfigurationFormatter = (
  configuration: PublishedItemRevision['configurations'][number],
) => string

const defaultConfigurationFormatter: CatalogConfigurationFormatter = (configuration) =>
  `${configuration.size} · ${configuration.materialType}`

export function catalogConfigurationOptions(
  revisions: PublishedItemRevision[],
  format: CatalogConfigurationFormatter = defaultConfigurationFormatter,
): CatalogConfigurationOption[] {
  return revisions.flatMap((revision) =>
    revision.configurations.map((configuration) => ({
      value: configuration.itemConfigurationId,
      title: `${revision.name} · ${format(configuration)}`,
    })),
  )
}

export function signedAdjustment(amountCopper: number, direction: 'credit' | 'debit'): number {
  const normalized = Math.max(0, Math.trunc(amountCopper))
  return direction === 'credit' ? normalized : -normalized
}

export function containerTitle(container: AdminInventoryContainer): string {
  const owner = container.ownerName?.trim() || `#${container.ownerId}`
  return `${container.ownerKind}: ${owner}`
}

export function canForceMove(
  sourceContainerKey: string | null,
  itemInstanceKey: string | null,
  destinationContainerKey: string | null,
  reason: string,
): boolean {
  return Boolean(
    sourceContainerKey &&
    itemInstanceKey &&
    destinationContainerKey &&
    sourceContainerKey !== destinationContainerKey &&
    reason.trim(),
  )
}
