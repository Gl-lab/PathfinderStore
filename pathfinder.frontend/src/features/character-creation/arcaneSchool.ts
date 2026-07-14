import type { ArcaneSchool, CharacterClass } from './api'

export function isArcaneSchoolChoiceComplete(
  characterClass: CharacterClass | null,
  school: ArcaneSchool | null,
): boolean {
  return characterClass?.id === 'class.wizard' ? school !== null : school === null
}

export function groupArcaneSchoolCurriculum(school: ArcaneSchool | null) {
  if (!school) return []
  return Array.from({ length: 10 }, (_, rank) => ({
    rank,
    spells: school.curriculumSpells.filter((spell) => spell.rank === rank),
  })).filter((group) => group.spells.length > 0)
}
