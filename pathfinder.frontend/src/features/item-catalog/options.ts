import type { PermanentUpgradeRequest } from './api'

export interface SelectOption<T extends string = string> {
  value: T
  title: string
}

export function enumOptions<T extends string>(
  values: readonly T[],
  format: (value: T) => string,
): SelectOption<T>[] {
  return values.map((value) => ({ value, title: format(value) }))
}

export interface ConfigurationUpgradeInput {
  code: string
  kind: string
  rank: number
  visibility: string
}

export interface ConfigurationSummaryInput {
  size: string
  materialType: string
  materialGrade: string
  campaignId?: number | null
  permanentUpgrades?: ConfigurationUpgradeInput[]
}

export function configurationSummary(
  configuration: ConfigurationSummaryInput,
  format: (group: string, code: string) => string = (_group, code) => code,
): string {
  const parts = [
    format('sizes', configuration.size),
    format('materials', configuration.materialType),
    format('grades', configuration.materialGrade),
  ]
  const upgrades = configuration.permanentUpgrades ?? []
  if (upgrades.length > 0) {
    parts.push(upgrades.map((upgrade) => upgrade.code).join(', '))
  }
  return parts.join(' · ')
}

export interface ConfigurationShape {
  size: string
  materialType: string
  materialGrade: string
  permanentUpgrades: PermanentUpgradeRequest[]
}

function upgradesKey(upgrades: ConfigurationUpgradeInput[]): string {
  return [...upgrades]
    .sort((left, right) => left.code.localeCompare(right.code))
    .map((upgrade) => `${upgrade.code}:${upgrade.kind}:${upgrade.rank}:${upgrade.visibility}`)
    .join(',')
}

export function isDuplicateConfiguration(
  existing: ConfigurationSummaryInput[],
  candidate: ConfigurationShape,
): boolean {
  return existing.some(
    (configuration) =>
      configuration.size === candidate.size &&
      configuration.materialType === candidate.materialType &&
      configuration.materialGrade === candidate.materialGrade &&
      upgradesKey(configuration.permanentUpgrades ?? []) ===
        upgradesKey(candidate.permanentUpgrades),
  )
}

export function upgradeValidationErrors(upgrades: PermanentUpgradeRequest[]): string[] {
  const errors: string[] = []
  if (upgrades.length > 16) {
    errors.push('itemCatalogUi.configuration.validation.tooManyUpgrades')
  }
  if (upgrades.some((upgrade) => !upgrade.code.trim())) {
    errors.push('itemCatalogUi.configuration.validation.codeRequired')
  }
  if (upgrades.some((upgrade) => upgrade.rank <= 0)) {
    errors.push('itemCatalogUi.configuration.validation.rankInvalid')
  }
  const codes = upgrades.map((upgrade) => upgrade.code.trim().toLowerCase()).filter(Boolean)
  if (new Set(codes).size !== codes.length) {
    errors.push('itemCatalogUi.configuration.validation.duplicateCodes')
  }
  return errors
}
