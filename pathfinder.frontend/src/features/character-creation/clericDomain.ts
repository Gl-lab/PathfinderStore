import type { CharacterClass, ClericDoctrine, ClericDomain, Deity } from './api'

export function getAvailableClericDomains(
  deity: Deity | null,
  domains: ClericDomain[],
): ClericDomain[] {
  if (!deity) return []
  const primaryDomainIds = new Set(deity.primaryDomainIds)
  return domains.filter((domain) => primaryDomainIds.has(domain.id))
}

export function isClericDomainChoiceComplete(
  characterClass: CharacterClass | null,
  doctrine: ClericDoctrine | null,
  deity: Deity | null,
  domain: ClericDomain | null,
): boolean {
  if (characterClass?.id !== 'class.cleric') return domain === null
  if (doctrine?.id !== 'cleric_doctrine.cloistered') return domain === null
  return Boolean(deity && domain && deity.primaryDomainIds.includes(domain.id))
}
