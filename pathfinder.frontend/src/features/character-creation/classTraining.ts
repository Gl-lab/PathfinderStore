import type {
  CharacterClass,
  ClassSkillGrantChoice,
  ClassTrainingTargetChoice,
  Skill,
} from './api'

export function createClassSkillGrantChoices(
  characterClass: CharacterClass | null,
): ClassSkillGrantChoice[] {
  return (characterClass?.initialSkillGrants ?? []).map((grant) => ({
    grantId: grant.id,
    selectedSkillId: null,
    replacementTarget: null,
  }))
}

export function getAdditionalClassTrainingCount(
  characterClass: CharacterClass | null,
  intelligenceScore: number,
): number {
  if (!characterClass) return 0
  return Math.max(0, characterClass.additionalSkillCountBase + Math.floor((intelligenceScore - 10) / 2))
}

export function createAdditionalClassTrainingChoices(count: number): ClassTrainingTargetChoice[] {
  return Array.from({ length: count }, () => ({ skillId: null, customLoreTopic: null }))
}

export function isClassTrainingComplete(
  characterClass: CharacterClass | null,
  grantChoices: ClassSkillGrantChoice[],
  additionalChoices: ClassTrainingTargetChoice[],
  requiredAdditionalCount: number,
  existingSkillIds: string[],
  skills: Skill[],
): boolean {
  if (!characterClass || grantChoices.length !== characterClass.initialSkillGrants.length) return false
  if (additionalChoices.length !== requiredAdditionalCount) return false

  const choices = new Map(grantChoices.map((choice) => [choice.grantId, choice]))
  const skillIds = new Set(skills.map((skill) => skill.id))
  const skillNames = new Set(skills.map((skill) => skill.name.toLocaleLowerCase()))
  const usedTargets = new Set(existingSkillIds)

  for (const grant of characterClass.initialSkillGrants) {
    const choice = choices.get(grant.id)
    if (!choice || grant.skillOptions.length === 0) return false
    if (grant.skillOptions.length === 1 && choice.selectedSkillId) return false
    const initialSkillId = grant.skillOptions.length === 1 ? grant.skillOptions[0] : choice.selectedSkillId
    if (!initialSkillId || !grant.skillOptions.includes(initialSkillId) || !skillIds.has(initialSkillId)) return false

    if (usedTargets.has(initialSkillId)) {
      if (!choice.replacementTarget || !addTarget(choice.replacementTarget, usedTargets, skillIds, skillNames)) return false
    } else {
      if (choice.replacementTarget) return false
      usedTargets.add(initialSkillId)
    }
  }

  return additionalChoices.every((choice) => addTarget(choice, usedTargets, skillIds, skillNames))
}

export function requiresClassGrantReplacement(
  grantId: string,
  characterClass: CharacterClass | null,
  grantChoices: ClassSkillGrantChoice[],
  existingTargetIds: string[],
): boolean {
  if (!characterClass) return false
  const choices = new Map(grantChoices.map((choice) => [choice.grantId, choice]))
  const usedTargets = new Set(existingTargetIds)
  for (const grant of characterClass.initialSkillGrants) {
    const choice = choices.get(grant.id)
    const initialSkillId = grant.skillOptions.length === 1 ? grant.skillOptions[0] : choice?.selectedSkillId
    if (grant.id === grantId) return Boolean(initialSkillId && usedTargets.has(initialSkillId))
    if (!initialSkillId) continue
    if (usedTargets.has(initialSkillId) && choice?.replacementTarget) {
      usedTargets.add(targetKey(choice.replacementTarget))
    } else {
      usedTargets.add(initialSkillId)
    }
  }
  return false
}

export function getClassTrainingLabels(
  grantChoices: ClassSkillGrantChoice[],
  additionalChoices: ClassTrainingTargetChoice[],
  characterClass: CharacterClass | null,
  existingSkillIds: string[],
  skills: Skill[],
): string[] {
  if (!characterClass) return []
  const labels: string[] = []
  const choices = new Map(grantChoices.map((choice) => [choice.grantId, choice]))
  const usedTargets = new Set(existingSkillIds)
  const skillNames = new Map(skills.map((skill) => [skill.id, skill.name]))

  for (const grant of characterClass.initialSkillGrants) {
    const choice = choices.get(grant.id)
    const initialSkillId = grant.skillOptions.length === 1 ? grant.skillOptions[0] : choice?.selectedSkillId
    if (!initialSkillId) continue
    if (usedTargets.has(initialSkillId) && choice?.replacementTarget) {
      labels.push(formatTarget(choice.replacementTarget, skillNames))
      usedTargets.add(targetKey(choice.replacementTarget))
    } else {
      labels.push(skillNames.get(initialSkillId) ?? initialSkillId)
      usedTargets.add(initialSkillId)
    }
  }

  return [...labels, ...additionalChoices.map((choice) => formatTarget(choice, skillNames))].filter(Boolean)
}

function addTarget(
  choice: ClassTrainingTargetChoice,
  usedTargets: Set<string>,
  skillIds: Set<string>,
  skillNames: Set<string>,
): boolean {
  const hasSkill = Boolean(choice.skillId)
  const hasLore = Boolean(choice.customLoreTopic?.trim())
  if (hasSkill === hasLore) return false
  if (choice.skillId && !skillIds.has(choice.skillId)) return false
  if (choice.customLoreTopic) {
    const normalizedTopic = removeLoreSuffix(choice.customLoreTopic)
    if (!normalizedTopic || normalizedTopic.length > 100) return false
    if (skillNames.has(normalizedTopic.toLocaleLowerCase())) return false
  }

  const key = targetKey(choice)
  if (!key || usedTargets.has(key)) return false
  usedTargets.add(key)
  return true
}

function targetKey(choice: ClassTrainingTargetChoice): string {
  if (choice.skillId) return choice.skillId
  return getCustomLoreId(choice.customLoreTopic)
}

export function getCustomLoreId(topic: string | null | undefined): string {
  const normalizedTopic = removeLoreSuffix(topic ?? '')
  const canonicalKey = normalizedTopic
    .normalize('NFKC')
    .toLocaleLowerCase()
    .replace(/[^\p{L}\p{N}]+/gu, '_')
    .replace(/^_+|_+$/g, '')
  return canonicalKey ? `lore.custom.${canonicalKey}` : ''
}

function removeLoreSuffix(topic: string): string {
  const trimmedTopic = topic.trim()
  if (trimmedTopic.toLocaleLowerCase() === 'lore') return ''
  return trimmedTopic.toLocaleLowerCase().endsWith(' lore')
    ? trimmedTopic.slice(0, -5).trim()
    : trimmedTopic
}

function formatTarget(choice: ClassTrainingTargetChoice, skillNames: Map<string, string>): string {
  if (choice.skillId) return skillNames.get(choice.skillId) ?? choice.skillId
  const topic = choice.customLoreTopic?.trim()
  return topic ? `${topic} Lore` : ''
}
