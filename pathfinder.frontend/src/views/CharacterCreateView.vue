<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import {
  getAbilityLabel,
  getAncestryChoiceLabel,
  getAncestryLabel,
  getBackgroundLabel,
  getCharacterClassLabel,
} from '@/i18n/domain'
import type {
  AbilityCode,
  AncestryCode,
  ProficiencyCategory,
  ProficiencyRank,
} from '@/features/characters/api'
import {
  formatProficiency,
  groupProficiencies,
} from '@/features/characters/proficiencies'
import {
  createCharacter,
  getAncestries,
  getBackgrounds,
  getCharacterClasses,
  getDruidicOrders,
  getBardMuses,
  getWitchPatrons,
  getHuntersEdges,
  getClericDoctrines,
  getDeities,
  getRogueRackets,
  getSkills,
  type Ancestry,
  type Background,
  type BackgroundTrainingChoice,
  type CharacterClass,
  type ClassSkillGrantChoice,
  type ClassTrainingTargetChoice,
  type ClericDoctrine,
  type Deity,
  type DivineFont,
  type DivineSanctification,
  type DruidicOrder,
  type BardMuse,
  type WitchPatron,
  type HuntersEdge,
  type RogueRacket,
  type RogueTrainingChoice,
  type Skill,
} from '@/features/character-creation/api'
import {
  getBackgroundFreeBoostOptions,
  createBackgroundTrainingChoices,
  getBackgroundTrainingLabels,
  isBackgroundChoiceComplete,
  isBackgroundTrainingComplete,
} from '@/features/character-creation/background'
import { isCharacterClassChoiceComplete } from '@/features/character-creation/characterClass'
import {
  createRogueTrainingChoices,
  getResolvedRogueTarget,
  getRogueGrants,
  getRogueKeyAbilities,
  isRogueRacketChoiceComplete,
  requiresRogueReplacement,
} from '@/features/character-creation/rogueRacket'
import {
  getEffectiveClassProficiencies,
  isClericDoctrineChoiceComplete,
} from '@/features/character-creation/clericDoctrine'
import {
  getDeityProficiencies,
  isDeityChoiceComplete,
  requiresDeitySkillReplacement,
} from '@/features/character-creation/deity'
import {
  calculateAbilityScorePreview,
  isFinalFreeBoostDisabled,
  isFinalFreeBoostSelectionComplete,
} from '@/features/character-creation/finalFreeBoosts'
import {
  createAdditionalClassTrainingChoices,
  createClassSkillGrantChoices,
  getAdditionalClassTrainingCount,
  getClassTrainingLabels,
  getCustomLoreId,
  isClassTrainingComplete,
  requiresClassGrantReplacement,
} from '@/features/character-creation/classTraining'
import { isHuntersEdgeChoiceComplete } from '@/features/character-creation/huntersEdge'
import {
  isDruidicOrderChoiceComplete,
  withDruidicOrderSkillGrant,
} from '@/features/character-creation/druidicOrder'
import { isBardMuseChoiceComplete } from '@/features/character-creation/bardMuse'
import {
  getWitchPatronFamiliarSpellOptions,
  isWitchPatronChoiceComplete,
  withWitchPatron,
} from '@/features/character-creation/witchPatron'

const router = useRouter()
const { t } = useI18n()
const step = ref(1)
const ancestries = ref<Ancestry[]>([])
const backgrounds = ref<Background[]>([])
const characterClasses = ref<CharacterClass[]>([])
const rogueRackets = ref<RogueRacket[]>([])
const huntersEdges = ref<HuntersEdge[]>([])
const druidicOrders = ref<DruidicOrder[]>([])
const bardMuses = ref<BardMuse[]>([])
const witchPatrons = ref<WitchPatron[]>([])
const clericDoctrines = ref<ClericDoctrine[]>([])
const deities = ref<Deity[]>([])
const skills = ref<Skill[]>([])
const isLoadingCatalogs = ref(true)
const isSubmitting = ref(false)
const errorMessages = ref<string[]>([])
const form = ref({
  name: '',
  concept: '',
  age: null as number | null,
  ancestryType: null as AncestryCode | null,
  heritageId: null as string | null,
  ancestryFeatId: null as string | null,
  freeBoosts: [] as AbilityCode[],
  backgroundId: null as string | null,
  backgroundRestrictedBoost: null as AbilityCode | null,
  backgroundFreeBoost: null as AbilityCode | null,
  backgroundTrainingChoices: [] as BackgroundTrainingChoice[],
  classId: null as string | null,
  classKeyAbility: null as AbilityCode | null,
  rogueRacketId: null as string | null,
  rogueTrainingChoices: [] as RogueTrainingChoice[],
  huntersEdgeId: null as string | null,
  druidicOrderId: null as string | null,
  bardMuseId: null as string | null,
  witchPatronId: null as string | null,
  witchPatronFamiliarSpellId: null as string | null,
  clericDoctrineId: null as string | null,
  deityId: null as string | null,
  divineFont: null as DivineFont | null,
  divineSanctification: null as DivineSanctification | null,
  deitySkillReplacementId: null as string | null,
  finalFreeBoosts: [] as AbilityCode[],
  classSkillGrantChoices: [] as ClassSkillGrantChoice[],
  additionalClassTrainingChoices: [] as ClassTrainingTargetChoice[],
})
const abilityCodes: AbilityCode[] = ['Strength', 'Dexterity', 'Constitution', 'Intelligence', 'Wisdom', 'Charisma']
const selectedAncestry = computed(
  () => ancestries.value.find((item) => item.type === form.value.ancestryType) ?? null,
)
const freeBoostSlots = computed(
  () => selectedAncestry.value?.abilityBoosts.filter((boost) => boost.isFree).length ?? 0,
)
const fixedBoosts = computed(
  () =>
    selectedAncestry.value?.abilityBoosts
      .filter((boost) => !boost.isFree)
      .map((boost) => boost.abilityType)
      .filter((type): type is AbilityCode => type !== null) ?? [],
)
const selectedHeritage = computed(
  () => selectedAncestry.value?.heritages.find((item) => item.id === form.value.heritageId) ?? null,
)
const selectedAncestryFeat = computed(
  () => selectedAncestry.value?.ancestryFeats.find((item) => item.id === form.value.ancestryFeatId) ?? null,
)
const selectedBackground = computed(
  () => backgrounds.value.find((item) => item.id === form.value.backgroundId) ?? null,
)
const selectedCharacterClass = computed(
  () => characterClasses.value.find((item) => item.id === form.value.classId) ?? null,
)
const selectedRogueRacket = computed(
  () => rogueRackets.value.find((item) => item.id === form.value.rogueRacketId) ?? null,
)
const selectedHuntersEdge = computed(
  () => huntersEdges.value.find((item) => item.id === form.value.huntersEdgeId) ?? null,
)
const selectedDruidicOrder = computed(
  () => druidicOrders.value.find((item) => item.id === form.value.druidicOrderId) ?? null,
)
const selectedBardMuse = computed(
  () => bardMuses.value.find((item) => item.id === form.value.bardMuseId) ?? null,
)
const selectedWitchPatron = computed(
  () => witchPatrons.value.find((item) => item.id === form.value.witchPatronId) ?? null,
)
const witchPatronFamiliarSpellOptions = computed(() =>
  getWitchPatronFamiliarSpellOptions(selectedWitchPatron.value),
)
const effectiveCharacterClass = computed(() =>
  withWitchPatron(
    withDruidicOrderSkillGrant(selectedCharacterClass.value, selectedDruidicOrder.value),
    selectedWitchPatron.value,
  ),
)
const selectedClericDoctrine = computed(
  () => clericDoctrines.value.find((item) => item.id === form.value.clericDoctrineId) ?? null,
)
const selectedDeity = computed(
  () => deities.value.find((item) => item.id === form.value.deityId) ?? null,
)
const eligibleDeities = computed(() => deities.value.filter((deity) => deity.canGrantClericPowers))
const effectiveClassProficiencies = computed(() =>
  [
    ...getEffectiveClassProficiencies(selectedCharacterClass.value, selectedClericDoctrine.value),
    ...getDeityProficiencies(selectedDeity.value),
  ],
)
const backgroundSkillIds = computed(() => {
  if (!selectedBackground.value) return []
  return selectedBackground.value.grants
    .filter((grant) => grant.kind === 'SkillTraining')
    .map((grant) => {
      if (!grant.requiresChoice) return grant.targetId
      return form.value.backgroundTrainingChoices.find((choice) => choice.grantId === grant.id)?.targetId ?? null
    })
    .filter((id): id is string => id !== null)
})
const backgroundLoreIds = computed(() => {
  if (!selectedBackground.value) return []
  return selectedBackground.value.grants
    .filter((grant) => grant.kind === 'LoreTraining')
    .map((grant) => {
      if (!grant.requiresChoice) return grant.targetId
      const choice = form.value.backgroundTrainingChoices.find((item) => item.grantId === grant.id)
      return choice?.targetId ?? getCustomLoreId(choice?.customLoreTopic)
    })
    .filter((id): id is string => Boolean(id))
})
const rogueKeyAbilities = computed(() => getRogueKeyAbilities(selectedRogueRacket.value))
const backgroundFreeBoostOptions = computed(() =>
  getBackgroundFreeBoostOptions(abilityCodes, form.value.backgroundRestrictedBoost),
)
const boostsBeforeFinal = computed<AbilityCode[]>(() => [
  ...fixedBoosts.value,
  ...form.value.freeBoosts,
  ...(form.value.backgroundRestrictedBoost ? [form.value.backgroundRestrictedBoost] : []),
  ...(form.value.backgroundFreeBoost ? [form.value.backgroundFreeBoost] : []),
  ...(form.value.classKeyAbility ? [form.value.classKeyAbility] : []),
])
const abilityScoresBeforeFinal = computed(() =>
  calculateAbilityScorePreview(boostsBeforeFinal.value, selectedAncestry.value?.abilityFlaws ?? []),
)
const abilityScoresAfterFinal = computed(() =>
  calculateAbilityScorePreview(
    [...boostsBeforeFinal.value, ...form.value.finalFreeBoosts],
    selectedAncestry.value?.abilityFlaws ?? [],
  ),
)
const existingClassTrainingSkillIds = computed(() => {
  const rogueSkillIds = getRogueGrants(selectedRogueRacket.value)
    .map((grant) => getResolvedRogueTarget(grant, form.value.rogueTrainingChoices))
    .filter((id): id is string => id !== null)
  const deitySkillId = selectedDeity.value?.divineSkillId ?? null
  const effectiveDeitySkillId = deitySkillId && backgroundSkillIds.value.includes(deitySkillId)
    ? form.value.deitySkillReplacementId
    : deitySkillId
  return Array.from(new Set([
    ...backgroundSkillIds.value,
    ...backgroundLoreIds.value,
    ...rogueSkillIds,
    ...(effectiveDeitySkillId ? [effectiveDeitySkillId] : []),
  ]))
})
const additionalClassTrainingCount = computed(() => getAdditionalClassTrainingCount(
  selectedCharacterClass.value,
  abilityScoresAfterFinal.value.Intelligence,
))
const classTrainingLabels = computed(() => getClassTrainingLabels(
  form.value.classSkillGrantChoices,
  form.value.additionalClassTrainingChoices,
  effectiveCharacterClass.value,
  existingClassTrainingSkillIds.value,
  skills.value,
))
const canContinue = computed(() => {
  if (step.value === 1)
    return (
      Boolean(form.value.name.trim()) &&
      (form.value.age === null || (Number.isInteger(form.value.age) && form.value.age > 0))
  )
  if (step.value === 2) return selectedAncestry.value !== null
  if (step.value === 3) return selectedHeritage.value !== null && selectedAncestryFeat.value !== null
  if (step.value === 4) return form.value.freeBoosts.length === freeBoostSlots.value
  if (step.value === 5)
    return isBackgroundChoiceComplete(
      selectedBackground.value,
      form.value.backgroundRestrictedBoost,
      form.value.backgroundFreeBoost,
    ) && isBackgroundTrainingComplete(
      selectedBackground.value,
      form.value.backgroundTrainingChoices,
    )
  if (step.value === 6)
    return (
      isClericDoctrineChoiceComplete(selectedCharacterClass.value, selectedClericDoctrine.value) &&
      isHuntersEdgeChoiceComplete(selectedCharacterClass.value, selectedHuntersEdge.value) &&
      isDruidicOrderChoiceComplete(selectedCharacterClass.value, selectedDruidicOrder.value) &&
      isBardMuseChoiceComplete(selectedCharacterClass.value, selectedBardMuse.value) &&
      isWitchPatronChoiceComplete(
        selectedCharacterClass.value,
        selectedWitchPatron.value,
        form.value.witchPatronFamiliarSpellId,
      ) &&
      isDeityChoiceComplete(
        selectedCharacterClass.value,
        selectedDeity.value,
        form.value.divineFont,
        form.value.divineSanctification,
        form.value.deitySkillReplacementId,
        backgroundSkillIds.value,
        skills.value,
      ) &&
      (selectedCharacterClass.value?.id === 'class.rogue'
        ? isRogueRacketChoiceComplete(
          selectedRogueRacket.value,
          form.value.classKeyAbility,
          form.value.rogueTrainingChoices,
          backgroundSkillIds.value,
          skills.value,
        )
        : isCharacterClassChoiceComplete(selectedCharacterClass.value, form.value.classKeyAbility))
    )
  if (step.value === 7)
    return isFinalFreeBoostSelectionComplete(form.value.finalFreeBoosts)
  if (step.value === 8)
    return isClassTrainingComplete(
      effectiveCharacterClass.value,
      form.value.classSkillGrantChoices,
      form.value.additionalClassTrainingChoices,
      additionalClassTrainingCount.value,
      existingClassTrainingSkillIds.value,
      skills.value,
    )
  return true
})

function selectAncestry(type: AncestryCode | null): void {
  form.value.ancestryType = type
  form.value.heritageId = null
  form.value.ancestryFeatId = null
  form.value.freeBoosts = []
}
function isBoostDisabled(type: AbilityCode): boolean {
  return (
    fixedBoosts.value.includes(type) ||
    (!form.value.freeBoosts.includes(type) && form.value.freeBoosts.length >= freeBoostSlots.value)
  )
}
function selectBackground(backgroundId: string | null): void {
  form.value.backgroundId = backgroundId
  form.value.backgroundRestrictedBoost = null
  form.value.backgroundFreeBoost = null
  const background = backgrounds.value.find((item) => item.id === backgroundId) ?? null
  form.value.backgroundTrainingChoices = createBackgroundTrainingChoices(background)
  form.value.deitySkillReplacementId = null
  resetClassTraining()
}

function getBackgroundTrainingChoice(grantId: string): BackgroundTrainingChoice | undefined {
  return form.value.backgroundTrainingChoices.find((choice) => choice.grantId === grantId)
}
function selectBackgroundTrainingTarget(grantId: string, targetId: string | null): void {
  const choice = getBackgroundTrainingChoice(grantId)
  if (!choice) return
  choice.targetId = targetId
  choice.customLoreTopic = null
  form.value.deitySkillReplacementId = null
  resetClassTrainingTargets()
}
function setBackgroundLoreTopic(grantId: string, topic: string): void {
  const choice = getBackgroundTrainingChoice(grantId)
  if (!choice) return
  choice.targetId = null
  choice.customLoreTopic = topic
  resetClassTrainingTargets()
}
function selectBackgroundRestrictedBoost(boost: AbilityCode | null): void {
  form.value.backgroundRestrictedBoost = boost
  if (form.value.backgroundFreeBoost === boost) form.value.backgroundFreeBoost = null
}
function selectCharacterClass(classId: string | null): void {
  form.value.classId = classId
  form.value.classKeyAbility = null
  form.value.rogueRacketId = null
  form.value.rogueTrainingChoices = []
  form.value.huntersEdgeId = null
  form.value.druidicOrderId = null
  form.value.bardMuseId = null
  form.value.witchPatronId = null
  form.value.witchPatronFamiliarSpellId = null
  form.value.clericDoctrineId = null
  form.value.deityId = null
  form.value.divineFont = null
  form.value.divineSanctification = null
  form.value.deitySkillReplacementId = null
  form.value.classSkillGrantChoices = createClassSkillGrantChoices(
    characterClasses.value.find((item) => item.id === classId) ?? null,
  )
  resetAdditionalClassTraining()
}
function selectDeity(deityId: string | null): void {
  form.value.deityId = deityId
  const deity = deities.value.find((item) => item.id === deityId) ?? null
  form.value.divineFont = deity?.divineFontOptions.length === 1 ? deity.divineFontOptions[0] : null
  form.value.divineSanctification = deity?.requiredSanctification ?? null
  form.value.deitySkillReplacementId = null
  resetClassTrainingTargets()
}
function selectDruidicOrder(druidicOrderId: string | null): void {
  form.value.druidicOrderId = druidicOrderId
  resetClassTraining()
}
function selectWitchPatron(witchPatronId: string | null): void {
  form.value.witchPatronId = witchPatronId
  form.value.witchPatronFamiliarSpellId = null
  resetClassTraining()
}
function selectRogueRacket(racketId: string | null): void {
  form.value.rogueRacketId = racketId
  form.value.classKeyAbility = null
  form.value.rogueTrainingChoices = createRogueTrainingChoices(
    rogueRackets.value.find((item) => item.id === racketId) ?? null,
  )
  resetClassTrainingTargets()
}
function getRogueChoice(grantId: string): RogueTrainingChoice | undefined {
  return form.value.rogueTrainingChoices.find((choice) => choice.grantId === grantId)
}
function getSkillName(skillId: string | null): string {
  return skills.value.find((skill) => skill.id === skillId)?.name ?? skillId ?? ''
}
function getReplacementOptions(grantId: string): Skill[] {
  const used = new Set([
    ...backgroundSkillIds.value,
    ...getRogueGrants(selectedRogueRacket.value)
      .filter((grant) => grant.id !== grantId)
      .map((grant) => getResolvedRogueTarget(grant, form.value.rogueTrainingChoices))
      .filter((id): id is string => id !== null),
  ])
  return skills.value.filter((skill) => !used.has(skill.id))
}
function getDeityReplacementOptions(): Skill[] {
  return skills.value.filter((skill) => !backgroundSkillIds.value.includes(skill.id))
}
function getClassGrantChoice(grantId: string): ClassSkillGrantChoice | undefined {
  return form.value.classSkillGrantChoices.find((choice) => choice.grantId === grantId)
}
function classGrantRequiresReplacement(grantId: string): boolean {
  return requiresClassGrantReplacement(
    grantId,
    effectiveCharacterClass.value,
    form.value.classSkillGrantChoices,
    existingClassTrainingSkillIds.value,
  )
}
function setTrainingSkill(target: ClassTrainingTargetChoice, skillId: string | null): void {
  target.skillId = skillId
  target.customLoreTopic = null
}
function setTrainingLore(target: ClassTrainingTargetChoice, topic: string): void {
  target.skillId = null
  target.customLoreTopic = topic
}
function setClassGrantReplacementSkill(grantId: string, skillId: string | null): void {
  const choice = getClassGrantChoice(grantId)
  if (!choice) return
  choice.replacementTarget = skillId ? { skillId, customLoreTopic: null } : null
}
function setClassGrantReplacementLore(grantId: string, topic: string): void {
  const choice = getClassGrantChoice(grantId)
  if (!choice) return
  choice.replacementTarget = topic ? { skillId: null, customLoreTopic: topic } : null
}
function resetAdditionalClassTraining(): void {
  form.value.additionalClassTrainingChoices = createAdditionalClassTrainingChoices(
    additionalClassTrainingCount.value,
  )
}
function resetClassTrainingTargets(): void {
  form.value.classSkillGrantChoices.forEach((choice) => { choice.replacementTarget = null })
  resetAdditionalClassTraining()
}
function resetClassTraining(): void {
  form.value.classSkillGrantChoices = createClassSkillGrantChoices(effectiveCharacterClass.value)
  resetAdditionalClassTraining()
}
function isFinalBoostDisabled(type: AbilityCode): boolean {
  return isFinalFreeBoostDisabled(type, form.value.finalFreeBoosts)
}
function formatFinalBoostScore(type: AbilityCode): string {
  return t('wizard.finalBoostScore', {
    before: abilityScoresBeforeFinal.value[type],
    after: abilityScoresAfterFinal.value[type],
  })
}
function getProficiencyRankLabel(rank: ProficiencyRank): string {
  return t(`proficiencies.ranks.${rank}`)
}
function getProficiencyCategoryLabel(category: ProficiencyCategory): string {
  return t(`proficiencies.categories.${category}`)
}
function formatAbilities(types: AbilityCode[]): string {
  return types.map(getAbilityLabel).join(', ') || t('wizard.none')
}
function next(): void {
  if (canContinue.value && step.value < 9) step.value += 1
}
function previous(): void {
  if (step.value > 1) step.value -= 1
}
async function submit(): Promise<void> {
  if (
    !selectedAncestry.value ||
    !selectedBackground.value ||
    !selectedCharacterClass.value ||
    !form.value.backgroundRestrictedBoost ||
    !form.value.backgroundFreeBoost ||
    !form.value.classKeyAbility ||
    !isHuntersEdgeChoiceComplete(selectedCharacterClass.value, selectedHuntersEdge.value) ||
    !isDruidicOrderChoiceComplete(selectedCharacterClass.value, selectedDruidicOrder.value) ||
    !isBardMuseChoiceComplete(selectedCharacterClass.value, selectedBardMuse.value) ||
    !isWitchPatronChoiceComplete(
      selectedCharacterClass.value,
      selectedWitchPatron.value,
      form.value.witchPatronFamiliarSpellId,
    ) ||
    !isClericDoctrineChoiceComplete(selectedCharacterClass.value, selectedClericDoctrine.value) ||
    !isDeityChoiceComplete(
      selectedCharacterClass.value,
      selectedDeity.value,
      form.value.divineFont,
      form.value.divineSanctification,
      form.value.deitySkillReplacementId,
      backgroundSkillIds.value,
      skills.value,
    ) ||
    !isFinalFreeBoostSelectionComplete(form.value.finalFreeBoosts)
    || !isClassTrainingComplete(
      effectiveCharacterClass.value,
      form.value.classSkillGrantChoices,
      form.value.additionalClassTrainingChoices,
      additionalClassTrainingCount.value,
      existingClassTrainingSkillIds.value,
      skills.value,
    )
  )
    return
  errorMessages.value = []
  isSubmitting.value = true
  try {
    await createCharacter({
      name: form.value.name.trim(),
      concept: form.value.concept.trim() || null,
      age: form.value.age,
      ancestryType: selectedAncestry.value.type,
      heritageId: selectedHeritage.value?.id ?? '',
      ancestryFeatId: selectedAncestryFeat.value?.id ?? '',
      freeBoosts: form.value.freeBoosts,
      backgroundId: selectedBackground.value.id,
      backgroundRestrictedBoost: form.value.backgroundRestrictedBoost,
      backgroundFreeBoost: form.value.backgroundFreeBoost,
      backgroundTrainingChoices: form.value.backgroundTrainingChoices.map((choice) => ({
        ...choice,
        customLoreTopic: choice.customLoreTopic?.trim() || null,
      })),
      classId: selectedCharacterClass.value.id,
      classKeyAbility: form.value.classKeyAbility,
      rogueRacketId: form.value.rogueRacketId,
      rogueTrainingChoices: form.value.rogueTrainingChoices,
      huntersEdgeId: form.value.huntersEdgeId,
      druidicOrderId: form.value.druidicOrderId,
      bardMuseId: form.value.bardMuseId,
      witchPatronId: form.value.witchPatronId,
      witchPatronFamiliarSpellId: form.value.witchPatronFamiliarSpellId,
      clericDoctrineId: form.value.clericDoctrineId,
      deityId: form.value.deityId,
      divineFont: form.value.divineFont,
      divineSanctification: form.value.divineSanctification,
      deitySkillReplacementId: form.value.deitySkillReplacementId,
      finalFreeBoosts: form.value.finalFreeBoosts,
      classSkillGrantChoices: form.value.classSkillGrantChoices.map((choice) => ({
        ...choice,
        replacementTarget: choice.replacementTarget
          ? {
            ...choice.replacementTarget,
            customLoreTopic: choice.replacementTarget.customLoreTopic?.trim() || null,
          }
          : null,
      })),
      additionalClassTrainingChoices: form.value.additionalClassTrainingChoices.map((choice) => ({
        ...choice,
        customLoreTopic: choice.customLoreTopic?.trim() || null,
      })),
    })
    await router.replace('/')
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isSubmitting.value = false
  }
}
async function loadCatalogs(): Promise<void> {
  isLoadingCatalogs.value = true
  errorMessages.value = []
  try {
    const [ancestryCatalog, backgroundCatalog, classCatalog, racketCatalog, huntersEdgeCatalog, druidicOrderCatalog, bardMuseCatalog, witchPatronCatalog, doctrineCatalog, deityCatalog, skillCatalog] = await Promise.all([
      getAncestries(),
      getBackgrounds(),
      getCharacterClasses(),
      getRogueRackets(),
      getHuntersEdges(),
      getDruidicOrders(),
      getBardMuses(),
      getWitchPatrons(),
      getClericDoctrines(),
      getDeities(),
      getSkills(),
    ])
    ancestries.value = ancestryCatalog
    backgrounds.value = backgroundCatalog
    characterClasses.value = classCatalog
    rogueRackets.value = racketCatalog
    huntersEdges.value = huntersEdgeCatalog
    druidicOrders.value = druidicOrderCatalog
    bardMuses.value = bardMuseCatalog
    witchPatrons.value = witchPatronCatalog
    clericDoctrines.value = doctrineCatalog
    deities.value = deityCatalog
    skills.value = skillCatalog
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isLoadingCatalogs.value = false
  }
}
onMounted(loadCatalogs)
watch(additionalClassTrainingCount, (count) => {
  if (form.value.additionalClassTrainingChoices.length !== count) {
    form.value.additionalClassTrainingChoices = createAdditionalClassTrainingChoices(count)
  }
})
watch(
  () => existingClassTrainingSkillIds.value.join('|'),
  () => resetClassTrainingTargets(),
)
</script>

<template>
  <section class="wizard">
    <header>
      <div>
        <p class="eyebrow">{{ t('wizard.eyebrow') }}</p>
        <h1>{{ t('wizard.title') }}</h1>
        <p>{{ t('wizard.lead') }}</p>
      </div>
      <v-btn variant="text" to="/">{{ t('common.cancel') }}</v-btn>
    </header>
    <v-progress-linear :model-value="(step / 9) * 100" color="accent" height="8" rounded />
    <ol class="steps">
      <li
        v-for="(item, index) in [t('wizard.basic'), t('wizard.ancestry'), t('wizard.choices'), t('wizard.boosts'), t('wizard.background'), t('classUi.characterClass'), t('wizard.finalFreeBoosts'), t('classUi.classTraining'), t('wizard.review')]"
        :key="item"
        :class="{ active: step === index + 1, complete: step > index + 1 }"
      >
        {{ item }}
      </li>
    </ol>
    <v-alert v-for="error in errorMessages" :key="error" type="error" variant="tonal">{{
      error
    }}</v-alert
    ><v-card elevation="0" class="wizard-card"
      ><v-card-text v-if="isLoadingCatalogs"
        ><v-progress-circular indeterminate color="accent" /> {{ t('wizard.loadingCatalogs') }}</v-card-text
      ><template v-else
        ><section v-if="step === 1">
          <h2>{{ t('wizard.basic') }}</h2>
          <p class="hint">{{ t('wizard.lead') }}</p>
          <v-text-field
            v-model="form.name"
            :label="t('wizard.name')"
            :rules="[(value) => Boolean(value?.trim()) || t('wizard.nameRequired')]"
            required
          /><v-textarea
            v-model="form.concept"
            :label="t('wizard.concept')"
            counter="1000"
            maxlength="1000"
            :hint="t('wizard.conceptHint')"
            persistent-hint
          /><v-text-field v-model.number="form.age" :label="t('wizard.age')" type="number" min="1" />
        </section>
        <section v-else-if="step === 2">
          <h2>{{ t('wizard.ancestry') }}</h2>
          <p class="hint">{{ t('wizard.ancestryHint') }}</p>
          <v-radio-group :model-value="form.ancestryType" @update:model-value="selectAncestry"
            ><v-radio v-for="ancestry in ancestries" :key="ancestry.type" :value="ancestry.type"
              ><template #label
                ><div>
                  <strong>{{ getAncestryLabel(ancestry.type) }}</strong>
                  <p class="radio-detail">
                    {{ ancestry.baseHitPoints }} HP · {{ t('wizard.speed', { speed: ancestry.baseSpeed }) }} ·
                    {{ t('wizard.fixedBoosts', { count: ancestry.abilityBoosts.filter((boost) => !boost.isFree).length }) }}
                  </p>
                </div></template
              ></v-radio
            ></v-radio-group
          >
        </section>
        <section v-else-if="step === 3 && selectedAncestry">
          <h2>{{ t('wizard.choices') }}</h2>
          <p class="hint">{{ t('wizard.choicesHint') }}</p>
          <v-radio-group v-model="form.heritageId" :label="t('wizard.heritage')">
            <v-radio
              v-for="heritage in selectedAncestry.heritages"
              :key="heritage.id"
              :value="heritage.id"
              :label="getAncestryChoiceLabel(heritage.id, heritage.name)"
            />
          </v-radio-group>
          <v-radio-group v-model="form.ancestryFeatId" :label="t('wizard.ancestryFeat')">
            <v-radio
              v-for="feat in selectedAncestry.ancestryFeats"
              :key="feat.id"
              :value="feat.id"
              :label="getAncestryChoiceLabel(feat.id, feat.name)"
            />
          </v-radio-group>
        </section>
        <section v-else-if="step === 4 && selectedAncestry">
          <h2>{{ t('wizard.selectedBoosts') }}</h2>
          <p class="hint">
            {{ t('wizard.freeBoostsHint', { count: freeBoostSlots, kind: freeBoostSlots === 1 ? t('wizard.oneAbility') : t('wizard.severalAbilities'), boosts: formatAbilities(fixedBoosts) }) }}
          </p>
          <v-checkbox
            v-for="code in abilityCodes"
            :key="code"
            v-model="form.freeBoosts"
            :value="code"
            :label="getAbilityLabel(code)"
            :disabled="isBoostDisabled(code)"
            hide-details
          />
        </section>
        <section v-else-if="step === 5">
          <h2>{{ t('wizard.background') }}</h2>
          <p class="hint">{{ t('wizard.backgroundHint') }}</p>
          <v-select
            :model-value="form.backgroundId"
            :items="backgrounds"
            :item-title="(background) => getBackgroundLabel(background.id, background.name)"
            item-value="id"
            :label="t('wizard.background')"
            @update:model-value="selectBackground"
          />
          <template v-if="selectedBackground">
            <v-radio-group
              :model-value="form.backgroundRestrictedBoost"
              :label="t('wizard.backgroundRestrictedBoost')"
              @update:model-value="selectBackgroundRestrictedBoost"
            >
              <v-radio
                v-for="code in selectedBackground.restrictedBoostOptions"
                :key="code"
                :value="code"
                :label="getAbilityLabel(code)"
              />
            </v-radio-group>
            <v-radio-group
              v-model="form.backgroundFreeBoost"
              :label="t('wizard.backgroundFreeBoost')"
            >
              <v-radio
                v-for="code in backgroundFreeBoostOptions"
                :key="code"
                :value="code"
                :label="getAbilityLabel(code)"
              />
            </v-radio-group>
            <div class="background-training">
              <h3>{{ t('wizard.backgroundTraining') }}</h3>
              <template v-for="grant in selectedBackground.grants" :key="grant.id">
                <v-select
                  v-if="grant.requiresChoice && !grant.allowsCustomLore && (grant.kind === 'SkillTraining' || grant.kind === 'LoreTraining')"
                  :model-value="getBackgroundTrainingChoice(grant.id)?.targetId"
                  :items="grant.options"
                  item-title="name"
                  item-value="id"
                  :label="grant.name"
                  @update:model-value="selectBackgroundTrainingTarget(grant.id, $event)"
                />
                <v-text-field
                  v-else-if="grant.allowsCustomLore && grant.kind === 'LoreTraining'"
                  :model-value="getBackgroundTrainingChoice(grant.id)?.customLoreTopic"
                  :label="grant.name"
                  :hint="grant.summary"
                  maxlength="100"
                  persistent-hint
                  @update:model-value="setBackgroundLoreTopic(grant.id, $event)"
                />
                <v-list-item
                  v-else
                  :title="grant.name"
                  :subtitle="grant.kind === 'SkillFeat' ? `${grant.summary} ${t('wizard.deferredFeat')}` : grant.summary"
                />
              </template>
            </div>
          </template>
        </section>
        <section v-else-if="step === 6">
          <h2>{{ t('classUi.characterClass') }}</h2>
          <p class="hint">{{ t('classUi.hint') }}</p>
          <v-select
            :model-value="form.classId"
            :items="characterClasses"
            :item-title="(characterClass) => getCharacterClassLabel(characterClass.id, characterClass.name)"
            item-value="id"
            :label="t('classUi.characterClass')"
            @update:model-value="selectCharacterClass"
          />
          <template v-if="selectedCharacterClass">
            <p>
              {{ t('classUi.baseHitPoints') }}: {{ selectedCharacterClass.baseHitPoints }}
            </p>
            <v-select
              v-if="selectedCharacterClass.id === 'class.bard'"
              v-model="form.bardMuseId"
              :items="bardMuses"
              item-title="name"
              item-value="id"
              :label="t('classUi.bardMuse')"
            />
            <v-alert
              v-for="benefit in selectedBardMuse?.benefits ?? []"
              :key="benefit.id"
              type="info"
              variant="tonal"
            >
              {{ t(`classUi.bardMuseBenefitKinds.${benefit.kind}`) }}:
              {{ benefit.name }}. {{ t('classUi.deferredEffect') }}
            </v-alert>
            <v-select
              v-if="selectedCharacterClass.id === 'class.witch'"
              :model-value="form.witchPatronId"
              :items="witchPatrons"
              item-title="name"
              item-value="id"
              :label="t('classUi.witchPatron')"
              @update:model-value="selectWitchPatron"
            />
            <v-select
              v-if="witchPatronFamiliarSpellOptions.length > 1"
              v-model="form.witchPatronFamiliarSpellId"
              :items="witchPatronFamiliarSpellOptions"
              item-title="name"
              item-value="id"
              :label="t('classUi.witchPatronFamiliarSpell')"
            />
            <v-alert
              v-for="benefit in selectedWitchPatron?.benefits ?? []"
              :key="benefit.id"
              type="info"
              variant="tonal"
            >
              {{ t(`classUi.witchPatronBenefitKinds.${benefit.kind}`) }}:
              {{ benefit.name }} — {{ benefit.summary }}
            </v-alert>
            <v-select
              v-if="selectedCharacterClass.id === 'class.druid'"
              :model-value="form.druidicOrderId"
              :items="druidicOrders"
              item-title="name"
              item-value="id"
              :label="t('classUi.druidicOrder')"
              @update:model-value="selectDruidicOrder"
            />
            <v-alert
              v-for="benefit in selectedDruidicOrder?.benefits ?? []"
              :key="benefit.id"
              type="info"
              variant="tonal"
            >
              {{ t(`classUi.druidicOrderBenefitKinds.${benefit.kind}`) }}:
              {{ benefit.name }}. {{ t('classUi.deferredEffect') }}
            </v-alert>
            <v-select
              v-if="selectedCharacterClass.id === 'class.ranger'"
              v-model="form.huntersEdgeId"
              :items="huntersEdges"
              item-title="name"
              item-value="id"
              :label="t('classUi.huntersEdge')"
            />
            <v-alert
              v-for="effect in selectedHuntersEdge?.effects ?? []"
              :key="effect.id"
              type="info"
              variant="tonal"
            >{{ effect.name }}: {{ effect.summary }} {{ t('classUi.deferredEffect') }}</v-alert>
            <v-select
              v-if="selectedCharacterClass.id === 'class.rogue'"
              :model-value="form.rogueRacketId"
              :items="rogueRackets"
              item-title="name"
              item-value="id"
              :label="t('classUi.rogueRacket')"
              @update:model-value="selectRogueRacket"
            />
            <v-select
              v-if="selectedCharacterClass.id === 'class.cleric'"
              v-model="form.clericDoctrineId"
              :items="clericDoctrines"
              item-title="name"
              item-value="id"
              :label="t('classUi.clericDoctrine')"
            />
            <template v-if="selectedCharacterClass.id === 'class.cleric'">
              <v-select
                :model-value="form.deityId"
                :items="eligibleDeities"
                item-title="name"
                item-value="id"
                :label="t('classUi.deity')"
                @update:model-value="selectDeity"
              />
              <v-radio-group
                v-if="selectedDeity"
                v-model="form.divineFont"
                :label="t('classUi.divineFont')"
              >
                <v-radio
                  v-for="font in selectedDeity.divineFontOptions"
                  :key="font"
                  :value="font"
                  :label="t(`classUi.divineFonts.${font}`)"
                />
              </v-radio-group>
              <v-radio-group
                v-if="selectedDeity && selectedDeity.sanctificationOptions.length > 0"
                v-model="form.divineSanctification"
                :label="t('classUi.sanctification')"
              >
                <v-radio
                  v-for="sanctification in selectedDeity.sanctificationOptions"
                  :key="sanctification"
                  :value="sanctification"
                  :label="t(`classUi.sanctifications.${sanctification}`)"
                />
              </v-radio-group>
              <v-select
                v-if="requiresDeitySkillReplacement(selectedDeity, backgroundSkillIds)"
                v-model="form.deitySkillReplacementId"
                :items="getDeityReplacementOptions()"
                item-title="name"
                item-value="id"
                :label="t('classUi.deitySkillReplacement')"
                :hint="t('classUi.deitySkillConflict', { skill: getSkillName(selectedDeity?.divineSkillId ?? null) })"
                persistent-hint
              />
              <v-alert v-if="selectedDeity" type="info" variant="tonal">
                {{ t('classUi.divineSkill') }}: {{ getSkillName(selectedDeity.divineSkillId) }}.
                {{ t('classUi.favoredWeapon') }}:
                {{ selectedDeity.favoredWeapons.map((weapon) => weapon.name).join(', ') }}.
                {{ t('classUi.domains') }}: {{ selectedDeity.primaryDomainIds.join(', ') }}.
                {{ t('classUi.grantedSpells') }}:
                {{ selectedDeity.grantedSpells.map((spell) => `${spell.rank}: ${spell.name}`).join(', ') }}.
                {{ t('classUi.deferredDeityBenefits') }}
              </v-alert>
            </template>
            <v-radio-group v-model="form.classKeyAbility" :label="t('classUi.keyAbility')">
              <v-radio
                v-for="code in selectedCharacterClass.id === 'class.rogue'
                  ? rogueKeyAbilities
                  : selectedCharacterClass.keyAbilityOptions"
                :key="code"
                :value="code"
                :label="getAbilityLabel(code)"
              />
            </v-radio-group>
            <div v-if="selectedRogueRacket" class="rogue-training">
              <h3>{{ t('classUi.rogueTraining') }}</h3>
              <template v-for="grant in getRogueGrants(selectedRogueRacket)" :key="grant.id">
                <v-select
                  v-if="grant.requiresChoice"
                  :model-value="getRogueChoice(grant.id)?.selectedSkillId"
                  :items="grant.options"
                  :item-title="getSkillName"
                  :label="t('classUi.racketSkill')"
                  @update:model-value="(value) => { const choice = getRogueChoice(grant.id); if (choice) { choice.selectedSkillId = value; choice.replacementSkillId = null } }"
                />
                <p v-else>{{ getSkillName(grant.targetId) }}</p>
                <v-select
                  v-if="requiresRogueReplacement(grant.id, selectedRogueRacket, form.rogueTrainingChoices, backgroundSkillIds)"
                  :model-value="getRogueChoice(grant.id)?.replacementSkillId"
                  :items="getReplacementOptions(grant.id)"
                  item-title="name"
                  item-value="id"
                  :label="t('classUi.replacementSkill')"
                  @update:model-value="(value) => { const choice = getRogueChoice(grant.id); if (choice) choice.replacementSkillId = value }"
                />
              </template>
              <v-alert
                v-for="effect in selectedRogueRacket.effects"
                :key="effect.id"
                type="info"
                variant="tonal"
              >{{ effect.name }}: {{ effect.summary }}</v-alert>
            </div>
            <div v-if="selectedClericDoctrine" class="rogue-training">
              <v-alert
                v-for="effect in selectedClericDoctrine.effects"
                :key="effect.id"
                type="info"
                variant="tonal"
              >{{ effect.name }}: {{ effect.summary }} {{ t('classUi.deferredEffect') }}</v-alert>
            </div>
            <v-list density="compact" :subheader="t('classUi.initialProficiencies')">
              <v-list-item
                v-for="group in groupProficiencies(effectiveClassProficiencies)"
                :key="group.category"
                :title="getProficiencyCategoryLabel(group.category)"
                :subtitle="group.items.map((item) => formatProficiency(item, getProficiencyRankLabel)).join(', ')"
              />
            </v-list>
            <v-list density="compact" :subheader="t('classUi.rules')">
              <v-list-item
                v-for="rule in selectedCharacterClass.rules"
                :key="rule.id"
                :title="rule.name"
                :subtitle="rule.deferredDependencies.length ? `${rule.summary} ${t('classUi.deferredChoice')}` : rule.summary"
              />
            </v-list>
          </template>
        </section>
        <section v-else-if="step === 7">
          <h2>{{ t('wizard.finalFreeBoosts') }}</h2>
          <p class="hint">
            {{ t('wizard.finalFreeBoostsHint', { selected: form.finalFreeBoosts.length }) }}
          </p>
          <v-checkbox
            v-for="code in abilityCodes"
            :key="code"
            v-model="form.finalFreeBoosts"
            :value="code"
            :label="`${getAbilityLabel(code)} · ${formatFinalBoostScore(code)}`"
            :disabled="isFinalBoostDisabled(code)"
            hide-details
          />
        </section>
        <section v-else-if="step === 8 && selectedCharacterClass">
          <h2>{{ t('classUi.classTraining') }}</h2>
          <p class="hint">
            {{ t('classUi.classTrainingHint', { count: additionalClassTrainingCount }) }}
          </p>
          <div
            v-for="grant in effectiveCharacterClass?.initialSkillGrants ?? []"
            :key="grant.id"
            class="training-choice"
          >
            <v-select
              v-if="grant.skillOptions.length > 1"
              :model-value="getClassGrantChoice(grant.id)?.selectedSkillId"
              :items="skills.filter((skill) => grant.skillOptions.includes(skill.id))"
              item-title="name"
              item-value="id"
              :label="t('classUi.initialClassSkill')"
              @update:model-value="(value) => { const choice = getClassGrantChoice(grant.id); if (choice) { choice.selectedSkillId = value; choice.replacementTarget = null } }"
            />
            <v-alert v-else type="info" variant="tonal">
              {{ t('classUi.initialClassSkill') }}: {{ getSkillName(grant.skillOptions[0]) }}
            </v-alert>
            <template v-if="classGrantRequiresReplacement(grant.id)">
              <p class="hint">{{ t('classUi.classSkillReplacementHint') }}</p>
              <v-select
                :model-value="getClassGrantChoice(grant.id)?.replacementTarget?.skillId"
                :items="skills"
                item-title="name"
                item-value="id"
                clearable
                :label="t('classUi.replacementSkill')"
                @update:model-value="(value) => setClassGrantReplacementSkill(grant.id, value)"
              />
              <v-text-field
                :model-value="getClassGrantChoice(grant.id)?.replacementTarget?.customLoreTopic"
                :label="t('classUi.customLore')"
                clearable
                @update:model-value="(value) => setClassGrantReplacementLore(grant.id, value ?? '')"
              />
            </template>
          </div>
          <h3>{{ t('classUi.additionalClassSkills') }}</h3>
          <div
            v-for="(choice, index) in form.additionalClassTrainingChoices"
            :key="index"
            class="training-choice"
          >
            <v-select
              :model-value="choice.skillId"
              :items="skills"
              item-title="name"
              item-value="id"
              clearable
              :label="t('classUi.additionalSkill', { number: index + 1 })"
              @update:model-value="(value) => setTrainingSkill(choice, value)"
            />
            <v-text-field
              :model-value="choice.customLoreTopic"
              :label="t('classUi.customLore')"
              clearable
              @update:model-value="(value) => setTrainingLore(choice, value ?? '')"
            />
          </div>
        </section>
        <section v-else-if="step === 9 && selectedAncestry">
          <h2>{{ t('wizard.review') }}</h2>
          <v-list lines="two"
            ><v-list-item :title="t('common.name')" :subtitle="form.name" /><v-list-item
              :title="t('wizard.selectedAncestry')"
              :subtitle="getAncestryLabel(selectedAncestry.type)" /><v-list-item
              :title="t('wizard.heritage')"
              :subtitle="selectedHeritage ? getAncestryChoiceLabel(selectedHeritage.id, selectedHeritage.name) : ''" /><v-list-item
              :title="t('wizard.ancestryFeat')"
              :subtitle="selectedAncestryFeat ? getAncestryChoiceLabel(selectedAncestryFeat.id, selectedAncestryFeat.name) : ''" /><v-list-item
              :title="t('wizard.selectedBoosts')"
              :subtitle="formatAbilities(form.freeBoosts)" /><v-list-item
              v-if="selectedBackground"
              :title="t('wizard.background')"
              :subtitle="getBackgroundLabel(selectedBackground.id, selectedBackground.name)" /><v-list-item
              v-if="form.backgroundRestrictedBoost && form.backgroundFreeBoost"
              :title="t('wizard.backgroundBoosts')"
              :subtitle="formatAbilities([form.backgroundRestrictedBoost, form.backgroundFreeBoost])" /><v-list-item
              v-if="selectedBackground"
              :title="t('wizard.backgroundTraining')"
              :subtitle="getBackgroundTrainingLabels(selectedBackground, form.backgroundTrainingChoices).join(', ')" /><v-list-item
              v-if="selectedCharacterClass"
              :title="t('classUi.characterClass')"
              :subtitle="getCharacterClassLabel(selectedCharacterClass.id, selectedCharacterClass.name)" /><v-list-item
              v-if="form.classKeyAbility"
              :title="t('classUi.keyAbility')"
              :subtitle="getAbilityLabel(form.classKeyAbility)" /><v-list-item
              v-if="selectedRogueRacket"
              :title="t('classUi.rogueRacket')"
              :subtitle="selectedRogueRacket.name" /><v-list-item
              v-if="selectedHuntersEdge"
              :title="t('classUi.huntersEdge')"
              :subtitle="selectedHuntersEdge.name" /><v-list-item
              v-if="selectedDruidicOrder"
              :title="t('classUi.druidicOrder')"
              :subtitle="selectedDruidicOrder.name" /><v-list-item
              v-if="selectedBardMuse"
              :title="t('classUi.bardMuse')"
              :subtitle="selectedBardMuse.name" /><v-list-item
              v-if="selectedWitchPatron"
              :title="t('classUi.witchPatron')"
              :subtitle="selectedWitchPatron.name" /><v-list-item
              v-if="selectedWitchPatron"
              :title="t('classUi.spellTradition')"
              :subtitle="t(`classUi.spellTraditions.${selectedWitchPatron.spellTradition}`)" /><v-list-item
              v-for="benefit in selectedWitchPatron?.benefits ?? []"
              :key="`review-${benefit.id}`"
              :title="t(`classUi.witchPatronBenefitKinds.${benefit.kind}`)"
              :subtitle="`${benefit.name} — ${benefit.summary}`" /><v-list-item
              v-for="benefit in selectedBardMuse?.benefits ?? []"
              :key="`review-${benefit.id}`"
              :title="t(`classUi.bardMuseBenefitKinds.${benefit.kind}`)"
              :subtitle="`${benefit.name}. ${t('classUi.deferredEffect')}`" /><v-list-item
              v-if="selectedClericDoctrine"
              :title="t('classUi.clericDoctrine')"
              :subtitle="selectedClericDoctrine.name" /><v-list-item
              v-if="selectedDeity"
              :title="t('classUi.deity')"
              :subtitle="selectedDeity.name" /><v-list-item
              v-if="selectedDeity && form.divineFont"
              :title="t('classUi.divineFont')"
              :subtitle="t(`classUi.divineFonts.${form.divineFont}`)" /><v-list-item
              v-if="selectedDeity"
              :title="t('classUi.domains')"
              :subtitle="selectedDeity.primaryDomainIds.join(', ')" /><v-list-item
              v-if="selectedCharacterClass"
              :title="t('classUi.initialProficiencies')"
              :subtitle="groupProficiencies(effectiveClassProficiencies).map((group) => getProficiencyCategoryLabel(group.category)).join(', ')" /><v-list-item
              :title="t('wizard.finalFreeBoosts')"
              :subtitle="formatAbilities(form.finalFreeBoosts)" /><v-list-item
              :title="t('classUi.classTraining')"
              :subtitle="classTrainingLabels.join(', ')" /><v-list-item
              v-if="form.concept"
              :title="t('wizard.selectedConcept')"
              :subtitle="form.concept" /></v-list
          ><v-alert type="info" variant="tonal"
            >{{ t('wizard.resultHint') }}</v-alert
          >
          <div class="ability-preview">
            <div v-for="code in abilityCodes" :key="code">
              <span>{{ getAbilityLabel(code) }}</span>
              <strong>{{ abilityScoresAfterFinal[code] }}</strong>
            </div>
          </div>
        </section></template
      ></v-card
    >
    <footer>
      <v-btn variant="text" :disabled="step === 1 || isSubmitting" @click="previous">{{ t('common.back') }}</v-btn
      ><v-spacer /><v-btn v-if="step < 9" color="primary" :disabled="!canContinue" @click="next"
        >{{ t('common.next') }}</v-btn
      ><v-btn v-else color="accent" :loading="isSubmitting" @click="submit"
        >{{ t('wizard.create') }}</v-btn
      >
    </footer>
  </section>
</template>

<style scoped>
.wizard {
  display: grid;
  gap: 24px;
  max-width: 880px;
}
.wizard header {
  display: flex;
  justify-content: space-between;
  gap: 20px;
  align-items: start;
}
.eyebrow {
  margin: 0 0 8px;
  color: rgb(var(--v-theme-secondary));
  font-size: 0.875rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}
h1 {
  margin: 0;
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
  font-size: clamp(2rem, 5vw, 3rem);
}
.steps {
  display: grid;
  grid-template-columns: repeat(8, 1fr);
  gap: 8px;
  padding: 0;
  margin: 0;
  list-style: none;
  color: #6b7280;
  font-size: 0.875rem;
}
.steps li {
  padding: 8px 0;
  border-bottom: 2px solid rgb(var(--v-theme-surface-variant));
}
.steps .active,
.steps .complete {
  color: rgb(var(--v-theme-primary));
  border-color: rgb(var(--v-theme-accent));
  font-weight: 700;
}
.wizard-card {
  min-height: 360px;
  border: 1px solid rgb(var(--v-theme-surface-variant));
}
.hint,
.radio-detail {
  color: #52606d;
}
.ability-preview {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px 16px;
  margin-top: 16px;
}
.ability-preview div {
  display: flex;
  justify-content: space-between;
  border-bottom: 1px solid rgb(var(--v-theme-surface-variant));
  padding-bottom: 6px;
}
.radio-detail {
  margin: 4px 0 0;
  font-size: 0.875rem;
}
footer {
  display: flex;
  align-items: center;
}
h2 {
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
}
@media (max-width: 600px) {
  .wizard header {
    flex-direction: column;
  }
  .steps {
    grid-template-columns: repeat(4, 1fr);
    font-size: 0.75rem;
    gap: 4px;
  }
}
</style>
