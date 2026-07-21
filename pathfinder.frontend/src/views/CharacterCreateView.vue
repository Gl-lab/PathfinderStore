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
  isSelectableCharacterGender,
  type SelectableCharacterGender,
} from '@/features/characters/gender'
import {
  createCharacter,
  getAncestries,
  getBackgrounds,
  getCharacterClasses,
  getDruidicOrders,
  getBardMuses,
  getWitchPatrons,
  getArcaneSchools,
  getArcaneTheses,
  getHuntersEdges,
  getClericDoctrines,
  getClericDomains,
  getClericSpellOptions,
  getSpellOptions,
  getDeities,
  getRogueRackets,
  getSkills,
  getFeatOptions,
  getLanguageSelectionOptions,
  getEquipment,
  getClassKits,
  type Ancestry,
  type Background,
  type BackgroundTrainingChoice,
  type CharacterClass,
  type ClassSkillGrantChoice,
  type ClassTrainingTargetChoice,
  type ClericDoctrine,
  type ClericDomain,
  type ClericSpellOptions,
  type BardSpellOptions,
  type DruidSpellOptions,
  type WitchSpellOptions,
  type SpellTradition,
  type Deity,
  type DivineFont,
  type DivineSanctification,
  type DruidicOrder,
  type BardMuse,
  type WitchPatron,
  type ArcaneSchool,
  type ArcaneThesis,
  type HuntersEdge,
  type RogueRacket,
  type RogueTrainingChoice,
  type Skill,
  type FeatDefinition,
  type FeatChoice,
  type LanguageSelectionOptions,
  type EquipmentDefinition,
  type ClassKit,
  type ClassKitOptionGroup,
} from '@/features/character-creation/api'
import {
  getBackgroundFreeBoostOptions,
  createBackgroundTrainingChoices,
  getBackgroundTrainingLabels,
  isBackgroundChoiceComplete,
  isBackgroundTrainingComplete,
} from '@/features/character-creation/background'
import {
  getAutomaticallySelectedKeyAbility,
  isCharacterClassChoiceComplete,
} from '@/features/character-creation/characterClass'
import {
  getAvailableClassFeatOptions,
  getRequiredClassFeatChoiceSlots,
  isClassFeatChoiceComplete as isRequiredClassFeatChoiceComplete,
} from '@/features/character-creation/classFeatChoices'
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
  getAvailableClericDomains,
  isClericDomainChoiceComplete,
} from '@/features/character-creation/clericDomain'
import { isClericSpellLoadoutComplete } from '@/features/character-creation/clericSpellLoadout'
import {
  calculateAbilityScorePreview,
  isFinalFreeBoostDisabled,
  isFinalFreeBoostSelectionComplete,
} from '@/features/character-creation/finalFreeBoosts'
import {
  createAdditionalClassTrainingChoices,
  createClassSkillGrantChoices,
  getAdditionalClassTrainingCount,
  getAvailableClassTrainingSkills,
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
  getMuseGrantedSpellId,
  isBardSpellLoadoutComplete,
} from '@/features/character-creation/bardSpellLoadout'
import { isDruidSpellLoadoutComplete } from '@/features/character-creation/druidSpellLoadout'
import { isWitchSpellLoadoutComplete } from '@/features/character-creation/witchSpellLoadout'
import {
  isWizardSpellLoadoutComplete,
  reconcileWizardSpellLoadoutForSchool,
  type WizardSpellOptions,
} from '@/features/character-creation/wizardSpellLoadout'
import {
  getWitchPatronFamiliarSpellOptions,
  isWitchPatronChoiceComplete,
  withWitchPatron,
} from '@/features/character-creation/witchPatron'
import {
  groupArcaneSchoolCurriculum,
  isArcaneSchoolChoiceComplete,
} from '@/features/character-creation/arcaneSchool'
import {
  formatArcaneThesisMilestones,
  isArcaneThesisChoiceComplete,
} from '@/features/character-creation/arcaneThesis'
import {
  isLanguageSelectionComplete,
  reconcileLanguageSelection,
} from '@/features/character-creation/languageSelection'

const router = useRouter()
const { t } = useI18n()
const step = ref(1)
const ancestries = ref<Ancestry[]>([])
const backgrounds = ref<Background[]>([])
const characterClasses = ref<CharacterClass[]>([])
const equipment = ref<EquipmentDefinition[]>([])
const classKits = ref<ClassKit[]>([])
const rogueRackets = ref<RogueRacket[]>([])
const huntersEdges = ref<HuntersEdge[]>([])
const druidicOrders = ref<DruidicOrder[]>([])
const bardMuses = ref<BardMuse[]>([])
const witchPatrons = ref<WitchPatron[]>([])
const arcaneSchools = ref<ArcaneSchool[]>([])
const arcaneTheses = ref<ArcaneThesis[]>([])
const clericDoctrines = ref<ClericDoctrine[]>([])
const deities = ref<Deity[]>([])
const clericDomains = ref<ClericDomain[]>([])
const clericSpellOptions = ref<ClericSpellOptions>({ cantrips: [], rankOneSpells: [] })
const bardSpellOptions = ref<BardSpellOptions>({ cantrips: [], rankOneSpells: [] })
const druidSpellOptions = ref<DruidSpellOptions>({ cantrips: [], rankOneSpells: [] })
const witchSpellOptionsByTradition = ref<Record<SpellTradition, WitchSpellOptions>>({
  Arcane: { cantrips: [], rankOneSpells: [] },
  Divine: { cantrips: [], rankOneSpells: [] },
  Occult: { cantrips: [], rankOneSpells: [] },
  Primal: { cantrips: [], rankOneSpells: [] },
})
const wizardSpellOptions = ref<WizardSpellOptions>({ cantrips: [], rankOneSpells: [] })
const skills = ref<Skill[]>([])
const ancestryFeatOptions = ref<FeatDefinition[]>([])
const skillFeatOptions = ref<FeatDefinition[]>([])
const classFeatOptions = ref<FeatDefinition[]>([])
const languageSelectionOptions = ref<LanguageSelectionOptions>({
  requiredCount: 0,
  availableLanguages: [],
})
const isLoadingCatalogs = ref(true)
const isLoadingLanguageOptions = ref(false)
const isSubmitting = ref(false)
const errorMessages = ref<string[]>([])
const form = ref({
  name: '',
  concept: '',
  age: null as number | null,
  gender: null as SelectableCharacterGender | null,
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
  classFeatChoices: [] as FeatChoice[],
  rogueRacketId: null as string | null,
  rogueTrainingChoices: [] as RogueTrainingChoice[],
  huntersEdgeId: null as string | null,
  druidicOrderId: null as string | null,
  bardMuseId: null as string | null,
  witchPatronId: null as string | null,
  witchPatronFamiliarSpellId: null as string | null,
  arcaneSchoolId: null as string | null,
  arcaneThesisId: null as string | null,
  clericDoctrineId: null as string | null,
  deityId: null as string | null,
  clericDomainId: null as string | null,
  divineFont: null as DivineFont | null,
  divineSanctification: null as DivineSanctification | null,
  deitySkillReplacementId: null as string | null,
  clericCantripIds: [] as string[],
  clericPreparedSpellIds: [] as (string | null)[],
  bardCantripIds: [] as string[],
  bardSpellIds: [] as string[],
  druidCantripIds: [] as string[],
  druidPreparedSpellIds: [] as (string | null)[],
  witchFamiliarCantripIds: [] as string[],
  witchFamiliarSpellIds: [] as string[],
  witchPreparedCantripIds: [] as string[],
  witchPreparedSpellIds: [] as (string | null)[],
  witchFocusHexId: null as string | null,
  wizardSpellbookCantripIds: [] as string[],
  wizardSpellbookSpellIds: [] as string[],
  wizardCurriculumCantripId: null as string | null,
  wizardCurriculumSpellIds: [] as string[],
  wizardPreparedCantripIds: [] as string[],
  wizardPreparedSpellIds: [] as (string | null)[],
  wizardPreparedCurriculumCantripId: null as string | null,
  wizardPreparedCurriculumSpellId: null as string | null,
  finalFreeBoosts: [] as AbilityCode[],
  additionalLanguageIds: [] as string[],
  classSkillGrantChoices: [] as ClassSkillGrantChoice[],
  additionalClassTrainingChoices: [] as ClassTrainingTargetChoice[],
  classKitOptionIds: [] as string[],
  deityFavoredWeaponEquipmentId: null as string | null,
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
const selectedAncestryFeatDefinition = computed(
  () => ancestryFeatOptions.value.find((feat) => feat.id === form.value.ancestryFeatId) ?? null,
)
const selectedBackgroundFeatDefinition = computed(() => {
  const grant = selectedBackground.value?.grants.find((item) => item.kind === 'SkillFeat')
  if (!grant) return null

  const featId = grant.requiresChoice
    ? form.value.backgroundTrainingChoices.find((choice) => choice.grantId === grant.id)?.targetId
    : grant.targetId
  return skillFeatOptions.value.find((feat) => feat.id === featId) ?? null
})
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
const bardRankOneSpellOptions = computed(() => {
  const museGrantedSpellId = getMuseGrantedSpellId(selectedBardMuse.value)
  return bardSpellOptions.value.rankOneSpells.filter((spell) => spell.id !== museGrantedSpellId)
})
const selectedBardCantrips = computed(() =>
  form.value.bardCantripIds
    .map((spellId) => bardSpellOptions.value.cantrips.find((spell) => spell.id === spellId))
    .filter((spell) => spell !== undefined),
)
const selectedBardSpells = computed(() =>
  form.value.bardSpellIds
    .map((spellId) => bardSpellOptions.value.rankOneSpells.find((spell) => spell.id === spellId))
    .filter((spell) => spell !== undefined),
)
const selectedDruidCantrips = computed(() =>
  form.value.druidCantripIds
    .map((spellId) => druidSpellOptions.value.cantrips.find((spell) => spell.id === spellId))
    .filter((spell) => spell !== undefined),
)
const selectedDruidSpells = computed(() =>
  form.value.druidPreparedSpellIds
    .map((spellId) => druidSpellOptions.value.rankOneSpells.find((spell) => spell.id === spellId))
    .filter((spell) => spell !== undefined),
)
const selectedDruidFocusSpell = computed(() =>
  selectedDruidicOrder.value?.benefits.find((benefit) => benefit.kind === 'FocusSpell') ?? null,
)
const selectedWitchPatron = computed(
  () => witchPatrons.value.find((item) => item.id === form.value.witchPatronId) ?? null,
)
const witchPatronFamiliarSpellOptions = computed(() =>
  getWitchPatronFamiliarSpellOptions(selectedWitchPatron.value),
)
const witchSpellOptions = computed<WitchSpellOptions>(() =>
  selectedWitchPatron.value
    ? witchSpellOptionsByTradition.value[selectedWitchPatron.value.spellTradition]
    : { cantrips: [], rankOneSpells: [] },
)
const selectedWitchPatronSpell = computed(() => {
  const options = witchPatronFamiliarSpellOptions.value
  return options.length === 1
    ? options[0] ?? null
    : options.find((spell) => spell.id === form.value.witchPatronFamiliarSpellId) ?? null
})
const witchFamiliarRankOneOptions = computed(() =>
  witchSpellOptions.value.rankOneSpells.filter(
    (spell) => spell.id !== selectedWitchPatronSpell.value?.id,
  ),
)
const selectedWitchFamiliarCantrips = computed(() =>
  form.value.witchFamiliarCantripIds
    .map((id) => witchSpellOptions.value.cantrips.find((spell) => spell.id === id))
    .filter((spell) => spell !== undefined),
)
const selectedWitchFamiliarSpells = computed(() =>
  form.value.witchFamiliarSpellIds
    .map((id) => witchSpellOptions.value.rankOneSpells.find((spell) => spell.id === id))
    .filter((spell) => spell !== undefined),
)
const witchKnownRankOneOptions = computed(() => [
  ...selectedWitchFamiliarSpells.value,
  ...(selectedWitchPatronSpell.value ? [selectedWitchPatronSpell.value] : []),
])
const selectedArcaneSchool = computed(
  () => arcaneSchools.value.find((item) => item.id === form.value.arcaneSchoolId) ?? null,
)
const arcaneSchoolCurriculum = computed(() =>
  groupArcaneSchoolCurriculum(selectedArcaneSchool.value),
)
const wizardCurriculumCantripOptions = computed(() =>
  selectedArcaneSchool.value?.curriculumSpells.filter((spell) => spell.rank === 0) ?? [],
)
const wizardCurriculumRankOneOptions = computed(() =>
  selectedArcaneSchool.value?.curriculumSpells.filter((spell) => spell.rank === 1) ?? [],
)
const selectedWizardSpellbookCantrips = computed(() =>
  form.value.wizardSpellbookCantripIds
    .map((id) => wizardSpellOptions.value.cantrips.find((spell) => spell.id === id))
    .filter((spell) => spell !== undefined),
)
const selectedWizardSpellbookSpells = computed(() =>
  form.value.wizardSpellbookSpellIds
    .map((id) => wizardSpellOptions.value.rankOneSpells.find((spell) => spell.id === id))
    .filter((spell) => spell !== undefined),
)
const selectedArcaneThesis = computed(
  () => arcaneTheses.value.find((item) => item.id === form.value.arcaneThesisId) ?? null,
)
const selectedClassKit = computed(
  () => classKits.value.find((item) => item.characterClassId === form.value.classId) ?? null,
)
const availableFavoredWeapons = computed(() => {
  if (!selectedDeity.value) return []
  const equipmentIds = new Set(
    selectedDeity.value.favoredWeapons.map((weapon) => `equipment.${weapon.id.replace('weapon.', '')}`),
  )
  return equipment.value.filter((item) => equipmentIds.has(item.id))
})
const selectedKitItems = computed(() => {
  if (!selectedClassKit.value) return []
  const quantities = new Map<string, number>()
  const addItems = (items: { equipmentId: string; purchaseQuantity: number }[]) => {
    items.forEach((item) => quantities.set(
      item.equipmentId,
      (quantities.get(item.equipmentId) ?? 0) + item.purchaseQuantity,
    ))
  }
  addItems(selectedClassKit.value.items)
  selectedClassKit.value.optionGroups
    .flatMap((group) => group.options)
    .filter((option) => form.value.classKitOptionIds.includes(option.id))
    .forEach((option) => addItems(option.items))
  if (form.value.deityFavoredWeaponEquipmentId) {
    addItems([{ equipmentId: form.value.deityFavoredWeaponEquipmentId, purchaseQuantity: 1 }])
  }
  return [...quantities.entries()].map(([equipmentId, purchaseQuantity]) => ({
    definition: equipment.value.find((item) => item.id === equipmentId) ?? null,
    purchaseQuantity,
  }))
})
const startingEquipmentCostCopper = computed(() => selectedKitItems.value.reduce(
  (total, item) => total + ((item.definition?.priceCopper ?? 0) * item.purchaseQuantity),
  0,
))
const isStartingEquipmentComplete = computed(() => {
  if (!selectedClassKit.value || selectedKitItems.value.some((item) => !item.definition)) return false
  const selectedOptions = selectedClassKit.value.optionGroups
    .flatMap((group) => group.options)
    .filter((option) => form.value.classKitOptionIds.includes(option.id))
  const requiresFavoredWeapon = selectedOptions.some(
    (option) => option.dependency === 'DeityFavoredWeapon',
  )
  return (
    (!requiresFavoredWeapon || Boolean(form.value.deityFavoredWeaponEquipmentId)) &&
    startingEquipmentCostCopper.value <= selectedClassKit.value.startingWealthCopper
  )
})
const classFeatChoiceSlots = computed(() => getRequiredClassFeatChoiceSlots(
  selectedCharacterClass.value,
  selectedArcaneSchool.value,
  selectedArcaneThesis.value,
))
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
const selectedClericDomain = computed(
  () => clericDomains.value.find((item) => item.id === form.value.clericDomainId) ?? null,
)
const selectedClericCantrips = computed(() =>
  form.value.clericCantripIds
    .map((spellId) => clericSpellOptions.value.cantrips.find((option) => option.spell.id === spellId)?.spell)
    .filter((spell) => spell !== undefined),
)
const selectedClericPreparedSpells = computed(() =>
  form.value.clericPreparedSpellIds
    .map((spellId) => clericSpellOptions.value.rankOneSpells.find((option) => option.spell.id === spellId)?.spell)
    .filter((spell) => spell !== undefined),
)
const classKeyAbilityOptions = computed(() => {
  if (!selectedCharacterClass.value) return []

  return selectedCharacterClass.value.id === 'class.rogue'
    ? getRogueKeyAbilities(selectedRogueRacket.value)
    : selectedCharacterClass.value.keyAbilityOptions
})
const eligibleDeities = computed(() => deities.value.filter((deity) => deity.canGrantClericPowers))
const availableClericDomains = computed(() =>
  getAvailableClericDomains(selectedDeity.value, clericDomains.value),
)
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
const selectedAdditionalLanguages = computed(() =>
  languageSelectionOptions.value.availableLanguages.filter((language) =>
    form.value.additionalLanguageIds.includes(language.id),
  ),
)
let languageOptionsRequestId = 0
async function loadLanguageOptions(): Promise<void> {
  const requestId = ++languageOptionsRequestId
  if (
    !form.value.ancestryType ||
    !isFinalFreeBoostSelectionComplete(form.value.finalFreeBoosts)
  ) {
    languageSelectionOptions.value = { requiredCount: 0, availableLanguages: [] }
    form.value.additionalLanguageIds = []
    isLoadingLanguageOptions.value = false
    return
  }

  isLoadingLanguageOptions.value = true
  try {
    const options = await getLanguageSelectionOptions(
      form.value.ancestryType,
      abilityScoresAfterFinal.value.Intelligence,
    )
    if (requestId !== languageOptionsRequestId) return

    languageSelectionOptions.value = options
    form.value.additionalLanguageIds = reconcileLanguageSelection(
      form.value.additionalLanguageIds,
      options,
    )
  } catch (error) {
    if (requestId !== languageOptionsRequestId) return
    languageSelectionOptions.value = { requiredCount: 0, availableLanguages: [] }
    form.value.additionalLanguageIds = []
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    if (requestId === languageOptionsRequestId) isLoadingLanguageOptions.value = false
  }
}
watch(
  [
    () => form.value.ancestryType,
    () => abilityScoresAfterFinal.value.Intelligence,
    () => form.value.finalFreeBoosts.length,
  ],
  () => void loadLanguageOptions(),
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
function isSelectedClassChoiceComplete(): boolean {
  return selectedCharacterClass.value?.id === 'class.rogue'
    ? isRogueRacketChoiceComplete(
      selectedRogueRacket.value,
      form.value.classKeyAbility,
      form.value.rogueTrainingChoices,
      backgroundSkillIds.value,
      skills.value,
    )
    : isCharacterClassChoiceComplete(selectedCharacterClass.value, form.value.classKeyAbility)
}
function getClassFeatChoice(sourceId: string): string | null {
  return form.value.classFeatChoices.find((choice) => choice.sourceId === sourceId)?.featId ?? null
}
function setClassFeatChoice(sourceId: string, featId: string | null): void {
  form.value.classFeatChoices = form.value.classFeatChoices.filter(
    (choice) => choice.sourceId !== sourceId,
  )
  if (featId) form.value.classFeatChoices.push({ sourceId, featId })
}
function getClassFeatDefinition(featId: string): FeatDefinition | null {
  return classFeatOptions.value.find((feat) => feat.id === featId) ?? null
}
function getClassFeatOptions(sourceId: string, requiresSpellshape: boolean): FeatDefinition[] {
  const slot = classFeatChoiceSlots.value.find((item) => item.sourceId === sourceId)
  if (!slot) return []
  return getAvailableClassFeatOptions(
    selectedCharacterClass.value,
    [...classFeatOptions.value, ...skillFeatOptions.value],
    { ...slot, requiresSpellshape },
    form.value.classFeatChoices,
  )
}
function isClassFeatChoiceComplete(): boolean {
  return isRequiredClassFeatChoiceComplete(
    classFeatChoiceSlots.value,
    form.value.classFeatChoices,
  )
}
const canContinue = computed(() => {
  if (step.value === 1)
    return (
      Boolean(form.value.name.trim()) &&
      isSelectableCharacterGender(form.value.gender) &&
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
      isArcaneSchoolChoiceComplete(selectedCharacterClass.value, selectedArcaneSchool.value) &&
      isArcaneThesisChoiceComplete(selectedCharacterClass.value, selectedArcaneThesis.value) &&
      isDeityChoiceComplete(
        selectedCharacterClass.value,
        selectedDeity.value,
        form.value.divineFont,
        form.value.divineSanctification,
        form.value.deitySkillReplacementId,
        backgroundSkillIds.value,
        skills.value,
      ) &&
      isClericDomainChoiceComplete(
        selectedCharacterClass.value,
        selectedClericDoctrine.value,
        selectedDeity.value,
        selectedClericDomain.value,
      ) &&
      isSelectedClassChoiceComplete() &&
      isClassFeatChoiceComplete()
    )
  if (step.value === 7)
    return (
      isClericSpellLoadoutComplete(
        selectedCharacterClass.value,
        form.value.clericCantripIds,
        form.value.clericPreparedSpellIds,
        clericSpellOptions.value,
      ) &&
      isBardSpellLoadoutComplete(
        selectedCharacterClass.value,
        selectedBardMuse.value,
        form.value.bardCantripIds,
        form.value.bardSpellIds,
        bardSpellOptions.value,
      ) &&
      isDruidSpellLoadoutComplete(
        selectedCharacterClass.value,
        form.value.druidCantripIds,
        form.value.druidPreparedSpellIds,
        druidSpellOptions.value,
      ) &&
      isWitchSpellLoadoutComplete(
        selectedCharacterClass.value,
        selectedWitchPatron.value,
        form.value.witchPatronFamiliarSpellId,
        form.value.witchFamiliarCantripIds,
        form.value.witchFamiliarSpellIds,
        form.value.witchPreparedCantripIds,
        form.value.witchPreparedSpellIds,
        form.value.witchFocusHexId,
        witchSpellOptions.value,
      ) &&
      isWizardSpellLoadoutComplete(
        selectedCharacterClass.value,
        selectedArcaneSchool.value,
        form.value.wizardSpellbookCantripIds,
        form.value.wizardSpellbookSpellIds,
        form.value.wizardCurriculumCantripId,
        form.value.wizardCurriculumSpellIds,
        form.value.wizardPreparedCantripIds,
        form.value.wizardPreparedSpellIds,
        form.value.wizardPreparedCurriculumCantripId,
        form.value.wizardPreparedCurriculumSpellId,
        wizardSpellOptions.value,
      )
    )
  if (step.value === 8)
    return (
      isFinalFreeBoostSelectionComplete(form.value.finalFreeBoosts) &&
      !isLoadingLanguageOptions.value &&
      isLanguageSelectionComplete(
        form.value.additionalLanguageIds,
        languageSelectionOptions.value,
      )
    )
  if (step.value === 9)
    return isClassTrainingComplete(
      effectiveCharacterClass.value,
      form.value.classSkillGrantChoices,
      form.value.additionalClassTrainingChoices,
      additionalClassTrainingCount.value,
      existingClassTrainingSkillIds.value,
      skills.value,
    )
  if (step.value === 10) return isStartingEquipmentComplete.value
  return true
})

function toggleClassKitOption(group: ClassKitOptionGroup, optionId: string): void {
  if (form.value.classKitOptionIds.includes(optionId)) {
    form.value.classKitOptionIds = form.value.classKitOptionIds.filter((id) => id !== optionId)
  } else if (group.selection === 'AtMostOne') {
    const groupOptionIds = new Set(group.options.map((option) => option.id))
    form.value.classKitOptionIds = form.value.classKitOptionIds.filter(
      (id) => !groupOptionIds.has(id),
    )
    form.value.classKitOptionIds.push(optionId)
  } else {
    form.value.classKitOptionIds.push(optionId)
  }
  const favoredOptionSelected = selectedClassKit.value?.optionGroups
    .flatMap((item) => item.options)
    .some((option) => (
      option.dependency === 'DeityFavoredWeapon' &&
      form.value.classKitOptionIds.includes(option.id)
    )) ?? false
  if (!favoredOptionSelected) form.value.deityFavoredWeaponEquipmentId = null
}

function selectAncestry(type: AncestryCode | null): void {
  form.value.ancestryType = type
  form.value.heritageId = null
  form.value.ancestryFeatId = null
  form.value.freeBoosts = []
  form.value.additionalLanguageIds = []
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
  form.value.classKitOptionIds = []
  form.value.deityFavoredWeaponEquipmentId = null
  const characterClass = characterClasses.value.find((item) => item.id === classId) ?? null
  form.value.classKeyAbility = getAutomaticallySelectedKeyAbility(
    characterClass?.keyAbilityOptions ?? [],
  )
  form.value.rogueRacketId = null
  form.value.classFeatChoices = []
  form.value.rogueTrainingChoices = []
  form.value.huntersEdgeId = null
  form.value.druidicOrderId = null
  form.value.bardMuseId = null
  form.value.witchPatronId = null
  form.value.witchPatronFamiliarSpellId = null
  form.value.arcaneSchoolId = null
  form.value.arcaneThesisId = null
  form.value.clericDoctrineId = null
  form.value.deityId = null
  form.value.clericDomainId = null
  form.value.divineFont = null
  form.value.divineSanctification = null
  form.value.deitySkillReplacementId = null
  form.value.clericCantripIds = []
  form.value.clericPreparedSpellIds = []
  form.value.bardCantripIds = []
  form.value.bardSpellIds = []
  form.value.druidCantripIds = []
  form.value.druidPreparedSpellIds = characterClass?.id === 'class.druid' ? [null, null] : []
  form.value.witchFamiliarCantripIds = []
  form.value.witchFamiliarSpellIds = []
  form.value.witchPreparedCantripIds = []
  form.value.witchPreparedSpellIds = characterClass?.id === 'class.witch' ? [null, null] : []
  form.value.witchFocusHexId = null
  resetWizardSpellLoadout()
  clericSpellOptions.value = { cantrips: [], rankOneSpells: [] }
  form.value.classSkillGrantChoices = createClassSkillGrantChoices(
    characterClasses.value.find((item) => item.id === classId) ?? null,
  )
  resetAdditionalClassTraining()
}
async function selectDeity(deityId: string | null): Promise<void> {
  form.value.deityId = deityId
  form.value.deityFavoredWeaponEquipmentId = null
  const deity = deities.value.find((item) => item.id === deityId) ?? null
  form.value.divineFont = deity?.divineFontOptions.length === 1 ? deity.divineFontOptions[0] : null
  form.value.divineSanctification = deity?.requiredSanctification ?? null
  form.value.deitySkillReplacementId = null
  form.value.clericDomainId = null
  form.value.clericCantripIds = []
  form.value.clericPreparedSpellIds = deity ? [null, null] : []
  clericSpellOptions.value = { cantrips: [], rankOneSpells: [] }
  resetClassTrainingTargets()
  if (deity) {
    try {
      const options = await getClericSpellOptions(deity.id)
      if (form.value.deityId === deity.id) clericSpellOptions.value = options
    } catch (error) {
      if (form.value.deityId === deity.id) errorMessages.value = getApiErrorMessages(error)
    }
  }
}
function selectClericDoctrine(clericDoctrineId: string | null): void {
  form.value.clericDoctrineId = clericDoctrineId
  form.value.clericDomainId = null
}
function selectDruidicOrder(druidicOrderId: string | null): void {
  form.value.druidicOrderId = druidicOrderId
  resetClassTraining()
}
function resetWizardSpellLoadout(): void {
  form.value.wizardSpellbookCantripIds = []
  form.value.wizardSpellbookSpellIds = []
  form.value.wizardCurriculumCantripId = null
  form.value.wizardCurriculumSpellIds = []
  form.value.wizardPreparedCantripIds = []
  form.value.wizardPreparedSpellIds = form.value.classId === 'class.wizard' ? [null, null] : []
  form.value.wizardPreparedCurriculumCantripId = null
  form.value.wizardPreparedCurriculumSpellId = null
}
function selectArcaneSchool(arcaneSchoolId: string | null): void {
  form.value.arcaneSchoolId = arcaneSchoolId
  const school = arcaneSchools.value.find((item) => item.id === arcaneSchoolId) ?? null
  const reconciled = reconcileWizardSpellLoadoutForSchool(school, {
    spellbookCantripIds: form.value.wizardSpellbookCantripIds,
    spellbookSpellIds: form.value.wizardSpellbookSpellIds,
    curriculumCantripId: form.value.wizardCurriculumCantripId,
    curriculumSpellIds: form.value.wizardCurriculumSpellIds,
    preparedCantripIds: form.value.wizardPreparedCantripIds,
    preparedSpellIds: form.value.wizardPreparedSpellIds,
    preparedCurriculumCantripId: form.value.wizardPreparedCurriculumCantripId,
    preparedCurriculumSpellId: form.value.wizardPreparedCurriculumSpellId,
  })
  form.value.wizardSpellbookCantripIds = reconciled.spellbookCantripIds
  form.value.wizardSpellbookSpellIds = reconciled.spellbookSpellIds
  form.value.wizardCurriculumCantripId = reconciled.curriculumCantripId
  form.value.wizardCurriculumSpellIds = reconciled.curriculumSpellIds
  form.value.wizardPreparedCantripIds = reconciled.preparedCantripIds
  form.value.wizardPreparedSpellIds = reconciled.preparedSpellIds
  form.value.wizardPreparedCurriculumCantripId = reconciled.preparedCurriculumCantripId
  form.value.wizardPreparedCurriculumSpellId = reconciled.preparedCurriculumSpellId
}
function selectBardMuse(bardMuseId: string | null): void {
  form.value.bardMuseId = bardMuseId
  const muse = bardMuses.value.find((item) => item.id === bardMuseId) ?? null
  const museGrantedSpellId = getMuseGrantedSpellId(muse)
  form.value.bardSpellIds = form.value.bardSpellIds.filter(
    (spellId) => spellId !== museGrantedSpellId,
  )
}
function selectWitchPatron(witchPatronId: string | null): void {
  const previousPatron = selectedWitchPatron.value
  const previousPatronSpellOptions = getWitchPatronFamiliarSpellOptions(previousPatron)
  const previousPatronSpellId = previousPatronSpellOptions.length === 1
    ? previousPatronSpellOptions[0]?.id
    : form.value.witchPatronFamiliarSpellId
  const nextPatron = witchPatrons.value.find((item) => item.id === witchPatronId) ?? null
  form.value.witchPatronId = witchPatronId
  form.value.witchPatronFamiliarSpellId = null
  if (!nextPatron || previousPatron?.spellTradition !== nextPatron.spellTradition) {
    form.value.witchFamiliarCantripIds = []
    form.value.witchFamiliarSpellIds = []
    form.value.witchPreparedCantripIds = []
    form.value.witchPreparedSpellIds = nextPatron ? [null, null] : []
    if (!nextPatron) form.value.witchFocusHexId = null
  } else {
    form.value.witchPreparedSpellIds = form.value.witchPreparedSpellIds.map((id) =>
      id === previousPatronSpellId ? null : id,
    )
  }
  resetClassTraining()
}
function selectRogueRacket(racketId: string | null): void {
  form.value.rogueRacketId = racketId
  const racket = rogueRackets.value.find((item) => item.id === racketId) ?? null
  form.value.classKeyAbility = getAutomaticallySelectedKeyAbility(getRogueKeyAbilities(racket))
  form.value.rogueTrainingChoices = createRogueTrainingChoices(racket)
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
function getClassTrainingSkillOptions(currentChoice: ClassTrainingTargetChoice | null): Skill[] {
  return getAvailableClassTrainingSkills(
    skills.value,
    effectiveCharacterClass.value,
    form.value.classSkillGrantChoices,
    form.value.additionalClassTrainingChoices,
    existingClassTrainingSkillIds.value,
    currentChoice,
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
function selectAdditionalLanguages(languageIds: string[]): void {
  form.value.additionalLanguageIds = languageIds.slice(
    0,
    languageSelectionOptions.value.requiredCount,
  )
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
function formatFeatReview(
  feat: FeatDefinition,
  acquisition: 'Selected' | 'Granted',
  sourceLabel: string,
): string {
  const deferred = feat.deferredDependencies.length
    ? t('feats.deferred', { dependencies: feat.deferredDependencies.join(', ') })
    : t('feats.resolved')

  return [
    feat.name,
    t(`feats.categories.${feat.category}`),
    t(`feats.acquisition.${acquisition}`),
    sourceLabel,
    `${feat.source.book}, ${t('feats.page', { page: feat.source.page })}`,
    deferred,
  ].join(' · ')
}
function selectClericCantrips(spellIds: string[]): void {
  form.value.clericCantripIds = spellIds.slice(0, 5)
}
function selectBardCantrips(spellIds: string[]): void {
  form.value.bardCantripIds = spellIds.slice(0, 5)
}
function selectBardSpells(spellIds: string[]): void {
  form.value.bardSpellIds = spellIds.slice(0, 2)
}
function selectDruidCantrips(spellIds: string[]): void {
  form.value.druidCantripIds = spellIds.slice(0, 5)
}
function selectWitchFamiliarCantrips(spellIds: string[]): void {
  form.value.witchFamiliarCantripIds = spellIds.slice(0, 10)
  form.value.witchPreparedCantripIds = form.value.witchPreparedCantripIds.filter((id) =>
    form.value.witchFamiliarCantripIds.includes(id),
  )
}
function selectWitchFamiliarSpells(spellIds: string[]): void {
  form.value.witchFamiliarSpellIds = spellIds.slice(0, 5)
  const knownIds = new Set([
    ...form.value.witchFamiliarSpellIds,
    ...(selectedWitchPatronSpell.value ? [selectedWitchPatronSpell.value.id] : []),
  ])
  form.value.witchPreparedSpellIds = form.value.witchPreparedSpellIds.map((id) =>
    id && knownIds.has(id) ? id : null,
  )
}
function selectWitchPreparedCantrips(spellIds: string[]): void {
  form.value.witchPreparedCantripIds = spellIds.slice(0, 5)
}
function next(): void {
  if (canContinue.value && step.value < 11) step.value += 1
}
function previous(): void {
  if (step.value > 1) step.value -= 1
}
async function submit(): Promise<void> {
  if (
    !selectedAncestry.value ||
    !selectedBackground.value ||
    !selectedCharacterClass.value ||
    !isSelectableCharacterGender(form.value.gender) ||
    !form.value.backgroundRestrictedBoost ||
    !form.value.backgroundFreeBoost ||
    !form.value.classKeyAbility ||
    !isSelectedClassChoiceComplete() ||
    !isHuntersEdgeChoiceComplete(selectedCharacterClass.value, selectedHuntersEdge.value) ||
    !isDruidicOrderChoiceComplete(selectedCharacterClass.value, selectedDruidicOrder.value) ||
    !isBardMuseChoiceComplete(selectedCharacterClass.value, selectedBardMuse.value) ||
    !isWitchPatronChoiceComplete(
      selectedCharacterClass.value,
      selectedWitchPatron.value,
      form.value.witchPatronFamiliarSpellId,
    ) ||
    !isArcaneSchoolChoiceComplete(selectedCharacterClass.value, selectedArcaneSchool.value) ||
    !isArcaneThesisChoiceComplete(selectedCharacterClass.value, selectedArcaneThesis.value) ||
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
    !isClericDomainChoiceComplete(
      selectedCharacterClass.value,
      selectedClericDoctrine.value,
      selectedDeity.value,
      selectedClericDomain.value,
    ) ||
    !isClericSpellLoadoutComplete(
      selectedCharacterClass.value,
      form.value.clericCantripIds,
      form.value.clericPreparedSpellIds,
      clericSpellOptions.value,
    ) ||
    !isBardSpellLoadoutComplete(
      selectedCharacterClass.value,
      selectedBardMuse.value,
      form.value.bardCantripIds,
      form.value.bardSpellIds,
      bardSpellOptions.value,
    ) ||
    !isDruidSpellLoadoutComplete(
      selectedCharacterClass.value,
      form.value.druidCantripIds,
      form.value.druidPreparedSpellIds,
      druidSpellOptions.value,
    ) ||
    !isWitchSpellLoadoutComplete(
      selectedCharacterClass.value,
      selectedWitchPatron.value,
      form.value.witchPatronFamiliarSpellId,
      form.value.witchFamiliarCantripIds,
      form.value.witchFamiliarSpellIds,
      form.value.witchPreparedCantripIds,
      form.value.witchPreparedSpellIds,
      form.value.witchFocusHexId,
      witchSpellOptions.value,
    ) ||
    !isWizardSpellLoadoutComplete(
      selectedCharacterClass.value,
      selectedArcaneSchool.value,
      form.value.wizardSpellbookCantripIds,
      form.value.wizardSpellbookSpellIds,
      form.value.wizardCurriculumCantripId,
      form.value.wizardCurriculumSpellIds,
      form.value.wizardPreparedCantripIds,
      form.value.wizardPreparedSpellIds,
      form.value.wizardPreparedCurriculumCantripId,
      form.value.wizardPreparedCurriculumSpellId,
      wizardSpellOptions.value,
    ) ||
    !isFinalFreeBoostSelectionComplete(form.value.finalFreeBoosts) ||
    !isLanguageSelectionComplete(
      form.value.additionalLanguageIds,
      languageSelectionOptions.value,
    ) ||
    !isClassTrainingComplete(
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
      gender: form.value.gender,
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
      classFeatChoices: form.value.classFeatChoices,
      rogueRacketId: form.value.rogueRacketId,
      rogueTrainingChoices: form.value.rogueTrainingChoices,
      huntersEdgeId: form.value.huntersEdgeId,
      druidicOrderId: form.value.druidicOrderId,
      bardMuseId: form.value.bardMuseId,
      witchPatronId: form.value.witchPatronId,
      witchPatronFamiliarSpellId: form.value.witchPatronFamiliarSpellId,
      arcaneSchoolId: form.value.arcaneSchoolId,
      arcaneThesisId: form.value.arcaneThesisId,
      clericDoctrineId: form.value.clericDoctrineId,
      deityId: form.value.deityId,
      clericDomainId: form.value.clericDomainId,
      divineFont: form.value.divineFont,
      divineSanctification: form.value.divineSanctification,
      deitySkillReplacementId: form.value.deitySkillReplacementId,
      clericCantripIds: form.value.clericCantripIds,
      clericPreparedSpellIds: form.value.clericPreparedSpellIds.filter(
        (spellId): spellId is string => spellId !== null,
      ),
      bardCantripIds: form.value.bardCantripIds,
      bardSpellIds: form.value.bardSpellIds,
      druidCantripIds: form.value.druidCantripIds,
      druidPreparedSpellIds: form.value.druidPreparedSpellIds.filter(
        (spellId): spellId is string => spellId !== null,
      ),
      witchFamiliarCantripIds: form.value.witchFamiliarCantripIds,
      witchFamiliarSpellIds: form.value.witchFamiliarSpellIds,
      witchPreparedCantripIds: form.value.witchPreparedCantripIds,
      witchPreparedSpellIds: form.value.witchPreparedSpellIds.filter(
        (spellId): spellId is string => spellId !== null,
      ),
      witchFocusHexId: form.value.witchFocusHexId,
      wizardSpellbookCantripIds: form.value.wizardSpellbookCantripIds,
      wizardSpellbookSpellIds: form.value.wizardSpellbookSpellIds,
      wizardCurriculumCantripId: form.value.wizardCurriculumCantripId,
      wizardCurriculumSpellIds: form.value.wizardCurriculumSpellIds,
      wizardPreparedCantripIds: form.value.wizardPreparedCantripIds,
      wizardPreparedSpellIds: form.value.wizardPreparedSpellIds.filter(
        (spellId): spellId is string => spellId !== null,
      ),
      wizardPreparedCurriculumCantripId: form.value.wizardPreparedCurriculumCantripId,
      wizardPreparedCurriculumSpellId: form.value.wizardPreparedCurriculumSpellId,
      finalFreeBoosts: form.value.finalFreeBoosts,
      additionalLanguageIds: form.value.additionalLanguageIds,
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
      classKitOptionIds: form.value.classKitOptionIds,
      deityFavoredWeaponEquipmentId: form.value.deityFavoredWeaponEquipmentId,
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
    const [ancestryCatalog, backgroundCatalog, classCatalog, equipmentCatalog, classKitCatalog, racketCatalog, huntersEdgeCatalog, druidicOrderCatalog, bardMuseCatalog, witchPatronCatalog, arcaneSchoolCatalog, arcaneThesisCatalog, doctrineCatalog, deityCatalog, clericDomainCatalog, skillCatalog, ancestryFeatCatalog, skillFeatCatalog, classFeatCatalog, arcaneCantripCatalog, arcaneRankOneSpellCatalog, divineCantripCatalog, divineRankOneSpellCatalog, occultCantripCatalog, occultRankOneSpellCatalog, primalCantripCatalog, primalRankOneSpellCatalog] = await Promise.all([
      getAncestries(),
      getBackgrounds(),
      getCharacterClasses(),
      getEquipment(),
      getClassKits(),
      getRogueRackets(),
      getHuntersEdges(),
      getDruidicOrders(),
      getBardMuses(),
      getWitchPatrons(),
      getArcaneSchools(),
      getArcaneTheses(),
      getClericDoctrines(),
      getDeities(),
      getClericDomains(),
      getSkills(),
      getFeatOptions('Ancestry', 1),
      getFeatOptions('Skill', 1),
      getFeatOptions('Class', 1),
      getSpellOptions('Arcane', 1, 'Cantrip'),
      getSpellOptions('Arcane', 1, 'Spell'),
      getSpellOptions('Divine', 1, 'Cantrip'),
      getSpellOptions('Divine', 1, 'Spell'),
      getSpellOptions('Occult', 1, 'Cantrip'),
      getSpellOptions('Occult', 1, 'Spell'),
      getSpellOptions('Primal', 1, 'Cantrip'),
      getSpellOptions('Primal', 1, 'Spell'),
    ])
    ancestries.value = ancestryCatalog
    backgrounds.value = backgroundCatalog
    characterClasses.value = classCatalog
    equipment.value = equipmentCatalog
    classKits.value = classKitCatalog
    rogueRackets.value = racketCatalog
    huntersEdges.value = huntersEdgeCatalog
    druidicOrders.value = druidicOrderCatalog
    bardMuses.value = bardMuseCatalog
    witchPatrons.value = witchPatronCatalog
    arcaneSchools.value = arcaneSchoolCatalog
    arcaneTheses.value = arcaneThesisCatalog
    clericDoctrines.value = doctrineCatalog
    deities.value = deityCatalog
    clericDomains.value = clericDomainCatalog
    skills.value = skillCatalog
    ancestryFeatOptions.value = ancestryFeatCatalog
    skillFeatOptions.value = skillFeatCatalog
    classFeatOptions.value = classFeatCatalog
    bardSpellOptions.value = {
      cantrips: occultCantripCatalog,
      rankOneSpells: occultRankOneSpellCatalog,
    }
    druidSpellOptions.value = {
      cantrips: primalCantripCatalog,
      rankOneSpells: primalRankOneSpellCatalog,
    }
    witchSpellOptionsByTradition.value = {
      Arcane: { cantrips: arcaneCantripCatalog, rankOneSpells: arcaneRankOneSpellCatalog },
      Divine: { cantrips: divineCantripCatalog, rankOneSpells: divineRankOneSpellCatalog },
      Occult: { cantrips: occultCantripCatalog, rankOneSpells: occultRankOneSpellCatalog },
      Primal: { cantrips: primalCantripCatalog, rankOneSpells: primalRankOneSpellCatalog },
    }
    wizardSpellOptions.value = {
      cantrips: arcaneCantripCatalog,
      rankOneSpells: arcaneRankOneSpellCatalog,
    }
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
watch(
  () => classFeatChoiceSlots.value.map((slot) => slot.sourceId).join('|'),
  () => {
    const sourceIds = new Set(classFeatChoiceSlots.value.map((slot) => slot.sourceId))
    form.value.classFeatChoices = form.value.classFeatChoices.filter(
      (choice) => sourceIds.has(choice.sourceId),
    )
  },
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
    <v-progress-linear :model-value="(step / 11) * 100" color="accent" height="8" rounded />
    <ol class="steps">
      <li
        v-for="(item, index) in [t('wizard.basic'), t('wizard.ancestry'), t('wizard.choices'), t('wizard.boosts'), t('wizard.background'), t('classUi.characterClass'), t('classUi.spells'), t('wizard.finalFreeBoosts'), t('classUi.classTraining'), t('equipment.title'), t('wizard.review')]"
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
      ><v-card-text v-else
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
          <v-radio-group
            v-model="form.gender"
            :label="t('wizard.gender')"
            :rules="[(value) => Boolean(value) || t('wizard.genderRequired')]"
            required
          >
            <v-radio :label="t('wizard.genders.Male')" value="Male" />
            <v-radio :label="t('wizard.genders.Female')" value="Female" />
          </v-radio-group>
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
                  v-if="grant.requiresChoice && !grant.allowsCustomLore"
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
              :model-value="form.bardMuseId"
              :items="bardMuses"
              item-title="name"
              item-value="id"
              :label="t('classUi.bardMuse')"
              @update:model-value="selectBardMuse"
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
              v-if="selectedCharacterClass.id === 'class.wizard'"
              :model-value="form.arcaneSchoolId"
              :items="arcaneSchools"
              item-title="name"
              item-value="id"
              :label="t('classUi.arcaneSchool')"
              @update:model-value="selectArcaneSchool"
            />
            <v-list v-if="selectedArcaneSchool" density="compact">
              <v-list-item
                v-for="group in arcaneSchoolCurriculum"
                :key="`curriculum-${group.rank}`"
                :title="t('classUi.curriculumRank', { rank: group.rank })"
                :subtitle="group.spells.map((spell) => spell.name).join(', ')"
              />
            </v-list>
            <v-alert
              v-for="benefit in selectedArcaneSchool?.benefits ?? []"
              :key="benefit.id"
              type="info"
              variant="tonal"
            >
              {{ t(`classUi.arcaneSchoolBenefitKinds.${benefit.kind}`) }}:
              {{ benefit.name }} — {{ benefit.summary }}
            </v-alert>
            <v-select
              v-if="selectedCharacterClass.id === 'class.wizard'"
              v-model="form.arcaneThesisId"
              :items="arcaneTheses"
              item-title="name"
              item-value="id"
              :label="t('classUi.arcaneThesis')"
            />
            <v-alert
              v-for="effect in selectedArcaneThesis?.effects ?? []"
              :key="effect.id"
              type="info"
              variant="tonal"
            >
              {{ t(`classUi.arcaneThesisEffectKinds.${effect.kind}`) }}:
              {{ effect.name }} — {{ effect.summary }}
              {{
                t('classUi.arcaneThesisMilestones', {
                  levels: formatArcaneThesisMilestones(effect.milestoneLevels),
                })
              }}
            </v-alert>
            <v-select
              v-for="slot in classFeatChoiceSlots"
              :key="slot.sourceId"
              :model-value="getClassFeatChoice(slot.sourceId)"
              :items="getClassFeatOptions(slot.sourceId, slot.requiresSpellshape)"
              item-title="name"
              item-value="id"
              :label="slot.name"
              @update:model-value="setClassFeatChoice(slot.sourceId, $event)"
            />
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
              :model-value="form.clericDoctrineId"
              :items="clericDoctrines"
              item-title="name"
              item-value="id"
              :label="t('classUi.clericDoctrine')"
              @update:model-value="selectClericDoctrine"
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
              <v-select
                v-if="selectedClericDoctrine?.id === 'cleric_doctrine.cloistered' && selectedDeity"
                v-model="form.clericDomainId"
                :items="availableClericDomains"
                item-title="name"
                item-value="id"
                :label="t('classUi.clericDomain')"
              />
              <v-alert v-if="selectedClericDomain" type="info" variant="tonal">
                {{ t('classUi.initialDomainSpell') }}:
                {{ selectedClericDomain.initialFocusPool.focusSpell.name }}.
                {{ t('classUi.focusPoints') }}:
                {{ selectedClericDomain.initialFocusPool.maximumFocusPoints }}.
              </v-alert>
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
            <v-radio-group
              v-if="classKeyAbilityOptions.length > 1"
              v-model="form.classKeyAbility"
              :label="t('classUi.keyAbility')"
            >
              <v-radio
                v-for="code in classKeyAbilityOptions"
                :key="code"
                :value="code"
                :label="getAbilityLabel(code)"
              />
            </v-radio-group>
            <p v-else-if="form.classKeyAbility">
              {{ t('classUi.keyAbility') }}: {{ getAbilityLabel(form.classKeyAbility) }}
            </p>
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
              >{{ effect.name }}: {{ effect.summary }}
                <template v-if="effect.deferredDependencies.length">{{ t('classUi.deferredEffect') }}</template>
              </v-alert>
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
          <h2>{{ t('classUi.spells') }}</h2>
          <template v-if="selectedCharacterClass?.id === 'class.cleric'">
            <p class="hint">{{ t('classUi.clericSpellsHint') }}</p>
            <v-select
              :model-value="form.clericCantripIds"
              :items="clericSpellOptions.cantrips.map((option) => option.spell)"
              item-title="name"
              item-value="id"
              :label="t('classUi.clericCantrips')"
              multiple
              chips
              closable-chips
              :counter="5"
              @update:model-value="selectClericCantrips"
            />
            <h3>{{ t('classUi.clericPreparedSpells') }}</h3>
            <v-select
              v-for="(_, index) in form.clericPreparedSpellIds"
              :key="index"
              v-model="form.clericPreparedSpellIds[index]"
              :items="clericSpellOptions.rankOneSpells.map((option) => option.spell)"
              item-title="name"
              item-value="id"
              :label="t('classUi.clericSpellSlot', { number: index + 1 })"
            />
            <v-alert v-if="form.divineFont" type="info" variant="tonal">
              {{ t('classUi.divineFontSpells') }}: 4 × {{ t(`classUi.divineFonts.${form.divineFont}`) }}
            </v-alert>
          </template>
          <template v-else-if="selectedCharacterClass?.id === 'class.bard'">
            <p class="hint">{{ t('classUi.bardSpellsHint') }}</p>
            <v-select
              :model-value="form.bardCantripIds"
              :items="bardSpellOptions.cantrips"
              item-title="name"
              item-value="id"
              :label="t('classUi.bardCantrips')"
              multiple
              chips
              closable-chips
              :counter="5"
              @update:model-value="selectBardCantrips"
            />
            <v-select
              :model-value="form.bardSpellIds"
              :items="bardRankOneSpellOptions"
              item-title="name"
              item-value="id"
              :label="t('classUi.bardRepertoireSpells')"
              multiple
              chips
              closable-chips
              :counter="2"
              @update:model-value="selectBardSpells"
            />
            <v-alert v-if="selectedBardMuse" type="info" variant="tonal">
              {{ t('classUi.museGrantedSpell') }}:
              {{ selectedBardMuse.benefits.find((benefit) => benefit.kind === 'RepertoireSpell')?.name }}
            </v-alert>
            <v-alert type="info" variant="tonal">
              {{ t('classUi.bardCompositionSummary') }}
            </v-alert>
          </template>
          <template v-else-if="selectedCharacterClass?.id === 'class.druid'">
            <p class="hint">{{ t('classUi.druidSpellsHint') }}</p>
            <v-select
              :model-value="form.druidCantripIds"
              :items="druidSpellOptions.cantrips"
              item-title="name"
              item-value="id"
              :label="t('classUi.druidCantrips')"
              multiple
              chips
              closable-chips
              :counter="5"
              @update:model-value="selectDruidCantrips"
            />
            <h3>{{ t('classUi.druidPreparedSpells') }}</h3>
            <v-select
              v-for="(_, index) in form.druidPreparedSpellIds"
              :key="index"
              v-model="form.druidPreparedSpellIds[index]"
              :items="druidSpellOptions.rankOneSpells"
              item-title="name"
              item-value="id"
              :label="t('classUi.druidSpellSlot', { number: index + 1 })"
            />
            <v-alert v-if="selectedDruidFocusSpell" type="info" variant="tonal">
              {{ t('classUi.druidOrderFocusSpell') }}: {{ selectedDruidFocusSpell.name }}.
              {{ t('classUi.focusPoints') }}: 1
            </v-alert>
          </template>
          <template v-else-if="selectedCharacterClass?.id === 'class.witch'">
            <p class="hint">{{ t('classUi.witchSpellsHint') }}</p>
            <v-select
              :model-value="form.witchFamiliarCantripIds"
              :items="witchSpellOptions.cantrips"
              item-title="name"
              item-value="id"
              :label="t('classUi.witchFamiliarCantrips')"
              multiple chips closable-chips :counter="10"
              @update:model-value="selectWitchFamiliarCantrips"
            />
            <v-select
              :model-value="form.witchFamiliarSpellIds"
              :items="witchFamiliarRankOneOptions"
              item-title="name"
              item-value="id"
              :label="t('classUi.witchFamiliarSpells')"
              multiple chips closable-chips :counter="5"
              @update:model-value="selectWitchFamiliarSpells"
            />
            <v-alert v-if="selectedWitchPatronSpell" type="info" variant="tonal">
              {{ t('classUi.witchPatronGrantedSpell') }}: {{ selectedWitchPatronSpell.name }}
            </v-alert>
            <v-select
              :model-value="form.witchPreparedCantripIds"
              :items="selectedWitchFamiliarCantrips"
              item-title="name"
              item-value="id"
              :label="t('classUi.witchPreparedCantrips')"
              multiple chips closable-chips :counter="5"
              @update:model-value="selectWitchPreparedCantrips"
            />
            <h3>{{ t('classUi.witchPreparedSpells') }}</h3>
            <v-select
              v-for="(_, index) in form.witchPreparedSpellIds"
              :key="index"
              v-model="form.witchPreparedSpellIds[index]"
              :items="witchKnownRankOneOptions"
              item-title="name"
              item-value="id"
              :label="t('classUi.witchSpellSlot', { number: index + 1 })"
            />
            <v-select
              v-model="form.witchFocusHexId"
              :items="selectedWitchPatron?.initialFocusHexOptions ?? []"
              item-title="name"
              item-value="id"
              :label="t('classUi.witchFocusHex')"
            />
          </template>
          <template v-else-if="selectedCharacterClass?.id === 'class.wizard'">
            <p class="hint">{{ t('classUi.wizardSpellsHint') }}</p>
            <v-select
              v-model="form.wizardSpellbookCantripIds"
              :items="wizardSpellOptions.cantrips"
              item-title="name" item-value="id"
              :label="t('classUi.wizardSpellbookCantrips')"
              multiple chips closable-chips :counter="10"
            />
            <v-select
              v-model="form.wizardSpellbookSpellIds"
              :items="wizardSpellOptions.rankOneSpells"
              item-title="name" item-value="id"
              :label="t('classUi.wizardSpellbookSpells', { count: selectedArcaneSchool?.hasCurriculum ? 5 : 6 })"
              multiple chips closable-chips
              :counter="selectedArcaneSchool?.hasCurriculum ? 5 : 6"
            />
            <template v-if="selectedArcaneSchool?.hasCurriculum">
              <v-select
                v-model="form.wizardCurriculumCantripId"
                :items="wizardCurriculumCantripOptions"
                item-title="name" item-value="id"
                :label="t('classUi.wizardCurriculumCantrip')"
              />
              <v-select
                v-model="form.wizardCurriculumSpellIds"
                :items="wizardCurriculumRankOneOptions"
                item-title="name" item-value="id"
                :label="t('classUi.wizardCurriculumSpells')"
                multiple chips closable-chips :counter="2"
              />
            </template>
            <v-select
              v-model="form.wizardPreparedCantripIds"
              :items="selectedWizardSpellbookCantrips"
              item-title="name" item-value="id"
              :label="t('classUi.wizardPreparedCantrips')"
              multiple chips closable-chips :counter="5"
            />
            <v-select
              v-for="(_, index) in form.wizardPreparedSpellIds"
              :key="`wizard-slot-${index}`"
              v-model="form.wizardPreparedSpellIds[index]"
              :items="selectedWizardSpellbookSpells"
              item-title="name" item-value="id"
              :label="t('classUi.wizardSpellSlot', { number: index + 1 })"
            />
            <template v-if="selectedArcaneSchool?.hasCurriculum">
              <v-select
                v-model="form.wizardPreparedCurriculumCantripId"
                :items="wizardCurriculumCantripOptions"
                item-title="name" item-value="id"
                :label="t('classUi.wizardCurriculumCantripSlot')"
              />
              <v-select
                v-model="form.wizardPreparedCurriculumSpellId"
                :items="wizardCurriculumRankOneOptions"
                item-title="name" item-value="id"
                :label="t('classUi.wizardCurriculumSpellSlot')"
              />
            </template>
            <v-alert type="info" variant="tonal">
              {{ t('classUi.wizardSchoolMagicSummary') }}
            </v-alert>
          </template>
          <v-alert v-else type="info" variant="tonal">{{ t('wizard.none') }}</v-alert>
        </section>
        <section v-else-if="step === 8">
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
          <v-divider class="my-4" />
          <h3>{{ t('wizard.additionalLanguages') }}</h3>
          <p class="hint">
            {{ t('wizard.additionalLanguagesHint', {
              selected: form.additionalLanguageIds.length,
              count: languageSelectionOptions.requiredCount,
            }) }}
          </p>
          <v-select
            v-if="languageSelectionOptions.requiredCount > 0"
            :model-value="form.additionalLanguageIds"
            :items="languageSelectionOptions.availableLanguages"
            item-title="name"
            item-value="id"
            multiple
            chips
            :loading="isLoadingLanguageOptions"
            :disabled="isLoadingLanguageOptions"
            :label="t('wizard.additionalLanguages')"
            @update:model-value="selectAdditionalLanguages"
          />
          <v-alert v-else-if="!isLoadingLanguageOptions" type="info" variant="tonal">
            {{ t('wizard.noAdditionalLanguages') }}
          </v-alert>
          <v-progress-linear v-else indeterminate color="primary" />
        </section>
        <section v-else-if="step === 9 && selectedCharacterClass">
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
                :items="getClassTrainingSkillOptions(getClassGrantChoice(grant.id)?.replacementTarget ?? null)"
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
              :items="getClassTrainingSkillOptions(choice)"
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
        <section v-else-if="step === 10 && selectedClassKit">
          <h2>{{ t('equipment.title') }}</h2>
          <p class="hint">{{ t('equipment.hint') }}</p>
          <h3>{{ selectedClassKit.name }}</h3>
          <v-list density="compact">
            <v-list-item
              v-for="item in selectedKitItems.filter((line) => line.definition)"
              :key="item.definition!.id"
              :title="`${item.definition!.name} × ${item.purchaseQuantity * item.definition!.unitsPerPurchase}`"
              :subtitle="`${item.definition!.priceCopper * item.purchaseQuantity} cp`"
            />
          </v-list>
          <div v-for="group in selectedClassKit.optionGroups" :key="group.id">
            <h3>{{ t('equipment.options') }}</h3>
            <v-checkbox
              v-for="option in group.options"
              :key="option.id"
              :model-value="form.classKitOptionIds.includes(option.id)"
              :label="option.name"
              hide-details
              @update:model-value="toggleClassKitOption(group, option.id)"
            />
          </div>
          <v-select
            v-if="selectedClassKit.optionGroups.flatMap((group) => group.options).some((option) => option.dependency === 'DeityFavoredWeapon' && form.classKitOptionIds.includes(option.id))"
            v-model="form.deityFavoredWeaponEquipmentId"
            :items="availableFavoredWeapons"
            item-title="name"
            item-value="id"
            :label="t('equipment.favoredWeapon')"
          />
          <v-alert
            :type="startingEquipmentCostCopper <= selectedClassKit.startingWealthCopper ? 'info' : 'error'"
            variant="tonal"
          >
            {{ t('equipment.budget', {
              spent: startingEquipmentCostCopper,
              remaining: selectedClassKit.startingWealthCopper - startingEquipmentCostCopper,
            }) }}
          </v-alert>
        </section>
        <section v-else-if="step === 11 && selectedAncestry">
          <h2>{{ t('wizard.review') }}</h2>
          <v-list lines="two"
            ><v-list-item :title="t('common.name')" :subtitle="form.name" /><v-list-item
              v-if="form.gender"
              :title="t('wizard.gender')"
              :subtitle="t(`wizard.genders.${form.gender}`)" /><v-list-item
              :title="t('wizard.selectedAncestry')"
              :subtitle="getAncestryLabel(selectedAncestry.type)" /><v-list-item
              :title="t('wizard.heritage')"
              :subtitle="selectedHeritage ? getAncestryChoiceLabel(selectedHeritage.id, selectedHeritage.name) : ''" /><v-list-item
              :title="t('wizard.ancestryFeat')"
              :subtitle="selectedAncestryFeatDefinition ? formatFeatReview(selectedAncestryFeatDefinition, 'Selected', getAncestryLabel(selectedAncestry.type)) : (selectedAncestryFeat ? getAncestryChoiceLabel(selectedAncestryFeat.id, selectedAncestryFeat.name) : '')" /><v-list-item
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
              v-if="selectedBackground && selectedBackgroundFeatDefinition"
              :title="t('feats.backgroundFeat')"
              :subtitle="formatFeatReview(selectedBackgroundFeatDefinition, 'Granted', getBackgroundLabel(selectedBackground.id, selectedBackground.name))" /><v-list-item
              v-if="selectedCharacterClass"
              :title="t('classUi.characterClass')"
              :subtitle="getCharacterClassLabel(selectedCharacterClass.id, selectedCharacterClass.name)" />
            <template v-for="choice in form.classFeatChoices" :key="choice.sourceId">
              <v-list-item
                v-if="getClassFeatDefinition(choice.featId)"
                :title="t('feats.classFeat')"
                :subtitle="formatFeatReview(getClassFeatDefinition(choice.featId)!, 'Selected', choice.sourceId)"
              />
            </template>
            <v-list-item
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
              v-if="selectedDruidCantrips.length"
              :title="t('classUi.druidCantrips')"
              :subtitle="selectedDruidCantrips.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="selectedDruidSpells.length"
              :title="t('classUi.druidPreparedSpells')"
              :subtitle="selectedDruidSpells.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="selectedDruidFocusSpell"
              :title="t('classUi.druidOrderFocusSpell')"
              :subtitle="`${selectedDruidFocusSpell.name} (${t('classUi.focusPoints')}: 1)`" /><v-list-item
              v-if="selectedBardMuse"
              :title="t('classUi.bardMuse')"
              :subtitle="selectedBardMuse.name" /><v-list-item
              v-if="selectedWitchPatron"
              :title="t('classUi.witchPatron')"
              :subtitle="selectedWitchPatron.name" /><v-list-item
              v-if="form.witchFamiliarCantripIds.length"
              :title="t('classUi.witchFamiliarCantrips')"
              :subtitle="selectedWitchFamiliarCantrips.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="selectedWitchFamiliarSpells.length"
              :title="t('classUi.witchFamiliarSpells')"
              :subtitle="selectedWitchFamiliarSpells.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="form.witchPreparedCantripIds.length"
              :title="t('classUi.witchPreparedCantrips')"
              :subtitle="form.witchPreparedCantripIds.map((id) => selectedWitchFamiliarCantrips.find((spell) => spell.id === id)?.name).filter(Boolean).join(', ')" /><v-list-item
              v-if="form.witchPreparedSpellIds.some(Boolean)"
              :title="t('classUi.witchPreparedSpells')"
              :subtitle="form.witchPreparedSpellIds.map((id) => witchKnownRankOneOptions.find((spell) => spell.id === id)?.name).filter(Boolean).join(', ')" /><v-list-item
              v-if="form.witchFocusHexId"
              :title="t('classUi.witchFocusHex')"
              :subtitle="selectedWitchPatron?.initialFocusHexOptions.find((spell) => spell.id === form.witchFocusHexId)?.name" /><v-list-item
              v-if="selectedWitchPatron"
              :title="t('classUi.spellTradition')"
              :subtitle="t(`classUi.spellTraditions.${selectedWitchPatron.spellTradition}`)" /><v-list-item
              v-for="benefit in selectedWitchPatron?.benefits ?? []"
              :key="`review-${benefit.id}`"
              :title="t(`classUi.witchPatronBenefitKinds.${benefit.kind}`)"
              :subtitle="`${benefit.name} — ${benefit.summary}`" /><v-list-item
              v-if="selectedArcaneSchool"
              :title="t('classUi.arcaneSchool')"
              :subtitle="selectedArcaneSchool.name" /><v-list-item
              v-if="form.wizardSpellbookCantripIds.length"
              :title="t('classUi.wizardSpellbookCantrips')"
              :subtitle="selectedWizardSpellbookCantrips.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="form.wizardSpellbookSpellIds.length"
              :title="t('classUi.wizardSpellbookSpells', { count: form.wizardSpellbookSpellIds.length })"
              :subtitle="selectedWizardSpellbookSpells.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="form.wizardCurriculumCantripId"
              :title="t('classUi.wizardCurriculumCantrip')"
              :subtitle="wizardCurriculumCantripOptions.find((spell) => spell.id === form.wizardCurriculumCantripId)?.name" /><v-list-item
              v-if="form.wizardCurriculumSpellIds.length"
              :title="t('classUi.wizardCurriculumSpells')"
              :subtitle="form.wizardCurriculumSpellIds.map((id) => wizardCurriculumRankOneOptions.find((spell) => spell.id === id)?.name).filter(Boolean).join(', ')" /><v-list-item
              v-if="form.wizardPreparedCantripIds.length"
              :title="t('classUi.wizardPreparedCantrips')"
              :subtitle="form.wizardPreparedCantripIds.map((id) => selectedWizardSpellbookCantrips.find((spell) => spell.id === id)?.name).filter(Boolean).join(', ')" /><v-list-item
              v-if="form.wizardPreparedSpellIds.some(Boolean)"
              :title="t('classUi.wizardSpellSlot', { number: '1–2' })"
              :subtitle="form.wizardPreparedSpellIds.map((id) => selectedWizardSpellbookSpells.find((spell) => spell.id === id)?.name).filter(Boolean).join(', ')" /><v-list-item
              v-for="benefit in selectedArcaneSchool?.benefits ?? []"
              :key="`review-school-${benefit.id}`"
              :title="t(`classUi.arcaneSchoolBenefitKinds.${benefit.kind}`)"
              :subtitle="`${benefit.name} — ${benefit.summary}`" /><v-list-item
              v-if="selectedArcaneThesis"
              :title="t('classUi.arcaneThesis')"
              :subtitle="selectedArcaneThesis.name" /><v-list-item
              v-for="effect in selectedArcaneThesis?.effects ?? []"
              :key="`review-thesis-${effect.id}`"
              :title="t(`classUi.arcaneThesisEffectKinds.${effect.kind}`)"
              :subtitle="`${effect.name} — ${effect.summary} (${formatArcaneThesisMilestones(effect.milestoneLevels)})`" /><v-list-item
              v-for="benefit in selectedBardMuse?.benefits ?? []"
              :key="`review-${benefit.id}`"
              :title="t(`classUi.bardMuseBenefitKinds.${benefit.kind}`)"
              :subtitle="benefit.kind === 'RepertoireSpell' ? benefit.name : `${benefit.name}. ${t('classUi.deferredEffect')}`" /><v-list-item
              v-if="selectedBardCantrips.length"
              :title="t('classUi.bardCantrips')"
              :subtitle="selectedBardCantrips.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="selectedBardSpells.length"
              :title="t('classUi.bardRepertoireSpells')"
              :subtitle="selectedBardSpells.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="selectedCharacterClass?.id === 'class.bard'"
              :title="t('classUi.compositionSpells')"
              :subtitle="t('classUi.bardCompositionSummary')" /><v-list-item
              v-if="selectedClericDoctrine"
              :title="t('classUi.clericDoctrine')"
              :subtitle="selectedClericDoctrine.name" /><v-list-item
              v-if="selectedDeity"
              :title="t('classUi.deity')"
              :subtitle="selectedDeity.name" /><v-list-item
              v-if="selectedClericDomain"
              :title="t('classUi.clericDomain')"
              :subtitle="`${selectedClericDomain.name} — ${selectedClericDomain.initialFocusSpell.name}`" /><v-list-item
              v-if="selectedClericDomain"
              :title="t('classUi.focusPool')"
              :subtitle="`${selectedClericDomain.initialFocusPool.focusSpell.name}: ${selectedClericDomain.initialFocusPool.maximumFocusPoints}`" /><v-list-item
              v-if="selectedDeity && form.divineFont"
              :title="t('classUi.divineFont')"
              :subtitle="t(`classUi.divineFonts.${form.divineFont}`)" /><v-list-item
              v-if="selectedClericCantrips.length"
              :title="t('classUi.clericCantrips')"
              :subtitle="selectedClericCantrips.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="selectedClericPreparedSpells.length"
              :title="t('classUi.clericPreparedSpells')"
              :subtitle="selectedClericPreparedSpells.map((spell) => spell.name).join(', ')" /><v-list-item
              v-if="form.divineFont"
              :title="t('classUi.divineFontSpells')"
              :subtitle="`4 × ${t(`classUi.divineFonts.${form.divineFont}`)}`" /><v-list-item
              v-if="selectedDeity"
              :title="t('classUi.domains')"
              :subtitle="selectedDeity.primaryDomainIds.join(', ')" /><v-list-item
              v-if="selectedCharacterClass"
              :title="t('classUi.initialProficiencies')"
              :subtitle="groupProficiencies(effectiveClassProficiencies).map((group) => getProficiencyCategoryLabel(group.category)).join(', ')" /><v-list-item
              :title="t('wizard.finalFreeBoosts')"
              :subtitle="formatAbilities(form.finalFreeBoosts)" /><v-list-item
              :title="t('wizard.additionalLanguages')"
              :subtitle="selectedAdditionalLanguages.map((language) => language.name).join(', ') || t('wizard.none')" /><v-list-item
              :title="t('classUi.classTraining')"
              :subtitle="classTrainingLabels.join(', ')" /><v-list-item
              v-if="selectedClassKit"
              :title="t('equipment.title')"
              :subtitle="`${selectedKitItems.map((item) => `${item.definition?.name} × ${item.purchaseQuantity * (item.definition?.unitsPerPurchase ?? 1)}`).join(', ')} · ${startingEquipmentCostCopper} cp`" /><v-list-item
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
        </section></v-card-text
      ></v-card
    >
    <footer>
      <v-btn variant="text" :disabled="step === 1 || isSubmitting" @click="previous">{{ t('common.back') }}</v-btn
      ><v-spacer /><v-btn v-if="step < 11" color="primary" :disabled="!canContinue" @click="next"
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
