import type {
  RogueRacket,
  RogueSkillGrant,
  RogueTrainingChoice,
  Skill,
} from '@/features/character-creation/api'
import type { AbilityCode } from '@/features/characters/api'

const stealthGrant: RogueSkillGrant = {
  id: 'class.rogue.skill.stealth',
  targetId: 'skill.stealth',
  options: [],
  requiresChoice: false,
}

export function getRogueKeyAbilities(racket: RogueRacket | null): AbilityCode[] {
  return racket?.alternativeKeyAbility
    ? ['Dexterity', racket.alternativeKeyAbility]
    : ['Dexterity']
}

export function createRogueTrainingChoices(racket: RogueRacket | null): RogueTrainingChoice[] {
  if (!racket) return []
  return [stealthGrant, ...racket.skillGrants].map((grant) => ({
    grantId: grant.id,
    selectedSkillId: null,
    replacementSkillId: null,
  }))
}

export function getRogueGrants(racket: RogueRacket | null): RogueSkillGrant[] {
  return racket ? [stealthGrant, ...racket.skillGrants] : []
}

export function getResolvedRogueTarget(
  grant: RogueSkillGrant,
  choices: RogueTrainingChoice[],
): string | null {
  if (!grant.requiresChoice) return grant.targetId
  return choices.find((choice) => choice.grantId === grant.id)?.selectedSkillId ?? null
}

export function requiresRogueReplacement(
  grantId: string,
  racket: RogueRacket | null,
  choices: RogueTrainingChoice[],
  backgroundSkillIds: string[],
): boolean {
  const trained = new Set(backgroundSkillIds)
  for (const grant of getRogueGrants(racket)) {
    const target = getResolvedRogueTarget(grant, choices)
    if (grant.id === grantId) return target !== null && trained.has(target)
    if (target) {
      const choice = choices.find((item) => item.grantId === grant.id)
      trained.add(trained.has(target) ? choice?.replacementSkillId ?? '' : target)
    }
  }
  return false
}

export function isRogueRacketChoiceComplete(
  racket: RogueRacket | null,
  keyAbility: AbilityCode | null,
  choices: RogueTrainingChoice[],
  backgroundSkillIds: string[],
  skills: Skill[],
): boolean {
  if (!racket || !keyAbility || !getRogueKeyAbilities(racket).includes(keyAbility)) return false

  const catalogIds = new Set(skills.map((skill) => skill.id))
  const trained = new Set(backgroundSkillIds)
  for (const grant of getRogueGrants(racket)) {
    const choice = choices.find((item) => item.grantId === grant.id)
    const target = getResolvedRogueTarget(grant, choices)
    if (!choice || !target || !catalogIds.has(target)) return false
    if (grant.requiresChoice && !grant.options.includes(target)) return false

    if (trained.has(target)) {
      if (!choice.replacementSkillId || !catalogIds.has(choice.replacementSkillId)) return false
      if (trained.has(choice.replacementSkillId)) return false
      trained.add(choice.replacementSkillId)
    } else {
      if (choice.replacementSkillId) return false
      trained.add(target)
    }
  }
  return true
}
