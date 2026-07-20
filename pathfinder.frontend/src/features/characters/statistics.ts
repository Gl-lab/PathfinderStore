import type { CharacterProficiencyStatistic } from '@/features/characters/api'

export function formatStatisticBreakdown(
  statistic: Pick<CharacterProficiencyStatistic, 'abilityModifier' | 'proficiencyBonus'>,
  formatSigned: (value: number) => string,
): { ability: string; proficiency: string } {
  return {
    ability: formatSigned(statistic.abilityModifier),
    proficiency: formatSigned(statistic.proficiencyBonus),
  }
}
