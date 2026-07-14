import type { CharacterClass, HuntersEdge } from './api'

export function isHuntersEdgeChoiceComplete(
  characterClass: CharacterClass | null,
  huntersEdge: HuntersEdge | null,
): boolean {
  return characterClass?.id === 'class.ranger' ? huntersEdge !== null : huntersEdge === null
}
