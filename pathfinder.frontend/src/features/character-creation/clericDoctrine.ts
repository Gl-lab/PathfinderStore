import type { CharacterClass, ClericDoctrine } from './api'
import type { Proficiency, ProficiencyRank } from '@/features/characters/api'

const rankValues: Record<ProficiencyRank, number> = {
  Untrained: 0,
  Trained: 1,
  Expert: 2,
  Master: 3,
  Legendary: 4,
}

export function isClericDoctrineChoiceComplete(
  characterClass: CharacterClass | null,
  doctrine: ClericDoctrine | null,
): boolean {
  if (!characterClass) return false
  return characterClass.id === 'class.cleric' ? doctrine !== null : doctrine === null
}

export function getEffectiveClassProficiencies(
  characterClass: CharacterClass | null,
  doctrine: ClericDoctrine | null,
): Proficiency[] {
  if (!characterClass) return []

  const effective = new Map<string, Proficiency>()
  for (const proficiency of [
    ...characterClass.initialProficiencies,
    ...(doctrine?.proficiencyGrants ?? []),
  ]) {
    const current = effective.get(proficiency.targetId)
    if (!current) {
      effective.set(proficiency.targetId, {
        ...proficiency,
        sourceGrantIds: proficiency.sourceGrantIds ?? [proficiency.sourceGrantId],
      })
      continue
    }

    const sourceGrantIds = [
      ...(current.sourceGrantIds ?? [current.sourceGrantId]),
      ...(proficiency.sourceGrantIds ?? [proficiency.sourceGrantId]),
    ].filter((sourceId, index, sources) => sources.indexOf(sourceId) === index)
    const highest = rankValues[proficiency.rank] > rankValues[current.rank]
      ? proficiency
      : current
    effective.set(proficiency.targetId, { ...highest, sourceGrantIds })
  }
  return [...effective.values()]
}
