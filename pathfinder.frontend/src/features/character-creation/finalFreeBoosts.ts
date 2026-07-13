import type { AbilityCode } from '@/features/characters/api'

export type AbilityScorePreview = Record<AbilityCode, number>

const abilityCodes: AbilityCode[] = [
  'Strength',
  'Dexterity',
  'Constitution',
  'Intelligence',
  'Wisdom',
  'Charisma',
]

export function isFinalFreeBoostSelectionComplete(boosts: AbilityCode[]): boolean {
  return boosts.length === 4 && new Set(boosts).size === 4
}

export function isFinalFreeBoostDisabled(
  ability: AbilityCode,
  selectedBoosts: AbilityCode[],
): boolean {
  return !selectedBoosts.includes(ability) && selectedBoosts.length >= 4
}

export function calculateAbilityScorePreview(
  boosts: AbilityCode[],
  flaws: AbilityCode[],
): AbilityScorePreview {
  const scores = Object.fromEntries(abilityCodes.map((ability) => [ability, 10])) as AbilityScorePreview

  for (const boost of boosts) scores[boost] += 2
  for (const flaw of flaws) scores[flaw] -= 2

  return scores
}
