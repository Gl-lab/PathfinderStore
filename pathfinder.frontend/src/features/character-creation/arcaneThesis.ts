import type { ArcaneThesis, CharacterClass } from './api'

export function isArcaneThesisChoiceComplete(
  characterClass: CharacterClass | null,
  thesis: ArcaneThesis | null,
): boolean {
  return characterClass?.id === 'class.wizard' ? thesis !== null : thesis === null
}

export function formatArcaneThesisMilestones(levels: number[]): string {
  return levels.join(', ')
}
