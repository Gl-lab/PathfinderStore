import type {
  Proficiency,
  ProficiencyCategory,
  ProficiencyRank,
} from '@/features/characters/api'

export interface ProficiencyGroup {
  category: ProficiencyCategory
  items: Proficiency[]
}

const categoryOrder: ProficiencyCategory[] = [
  'Perception',
  'SavingThrow',
  'Attack',
  'Defense',
  'ClassDc',
]

export function groupProficiencies(proficiencies: Proficiency[]): ProficiencyGroup[] {
  return categoryOrder
    .map((category) => ({
      category,
      items: proficiencies.filter((proficiency) => proficiency.category === category),
    }))
    .filter((group) => group.items.length > 0)
}

export function formatProficiency(
  proficiency: Proficiency,
  getRankLabel: (rank: ProficiencyRank) => string,
): string {
  return `${proficiency.name} — ${getRankLabel(proficiency.rank)}`
}
