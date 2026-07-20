<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import {
  getAbilityLabel,
  getAncestryChoiceLabel,
  getAncestryLabel,
  getBackgroundLabel,
  getCharacterClassLabel,
  getLanguageLabel,
  getVisionLabel,
} from '@/i18n/domain'
import { useI18n } from 'vue-i18n'
import {
  deleteCharacter,
  getCharacter,
  setCharacterGender,
  type Character,
  type CharacterProficiencyStatistic,
  type ProficiencyCategory,
  type ProficiencyRank,
} from '@/features/characters/api'
import { formatSignedModifier } from '@/features/characters/hitPoints'
import { formatStatisticBreakdown } from '@/features/characters/statistics'
import { getSkillModifierSections } from '@/features/characters/skillModifiers'
import {
  isLegacyGenderSelectionRequired,
  type SelectableCharacterGender,
} from '@/features/characters/gender'
import {
  formatProficiency,
  groupProficiencies,
} from '@/features/characters/proficiencies'
import CharacterAvatar from '@/features/characters/CharacterAvatar.vue'

const route = useRoute()
const { t } = useI18n()
const router = useRouter()
const character = ref<Character | null>(null)
const errors = ref<string[]>([])
const isLoading = ref(true)
const isDeleting = ref(false)
const confirmDelete = ref(false)
const selectedGender = ref<SelectableCharacterGender | null>(null)
const isSavingGender = ref(false)
const mustSelectGender = computed(
  () => character.value !== null && isLegacyGenderSelectionRequired(character.value.gender),
)
const proficiencyGroups = computed(() => groupProficiencies(character.value?.proficiencies ?? []))
const statisticRows = computed<
  { key: string; label: string; statistic: CharacterProficiencyStatistic }[]
>(() => {
  const statistics = character.value?.derivedStatistics
  if (!statistics) return []

  return [
    { key: 'perception', label: t('statistics.perception'), statistic: statistics.perception },
    {
      key: 'fortitude',
      label: t('statistics.fortitude'),
      statistic: statistics.savingThrows.fortitude,
    },
    {
      key: 'reflex',
      label: t('statistics.reflex'),
      statistic: statistics.savingThrows.reflex,
    },
    { key: 'will', label: t('statistics.will'), statistic: statistics.savingThrows.will },
  ]
})
const skillModifierSections = computed(() => {
  const modifiers = character.value?.derivedStatistics?.skillModifiers
  if (!modifiers) return []

  return getSkillModifierSections(modifiers).map((section) => ({
    ...section,
    label: t(`statistics.${section.key === 'general' ? 'generalSkills' : 'lore'}`),
  }))
})
const abilityCodes = {
  strength: 'Strength',
  dexterity: 'Dexterity',
  constitution: 'Constitution',
  intelligence: 'Intelligence',
  wisdom: 'Wisdom',
  charisma: 'Charisma',
} as const
function formatChoiceId(id: string | null): string {
  if (!id) return '—'

  const fallback = id
    .split('.')
    .map((part) => part.replaceAll('_', ' '))
    .join(': ')

  return getAncestryChoiceLabel(id, fallback)
}
function getProficiencyRankLabel(rank: ProficiencyRank): string {
  return t(`proficiencies.ranks.${rank}`)
}
function getProficiencyCategoryLabel(category: ProficiencyCategory): string {
  return t(`proficiencies.categories.${category}`)
}
function getStatisticBreakdown(statistic: CharacterProficiencyStatistic): string {
  return t(
    'statistics.breakdown',
    formatStatisticBreakdown(statistic, formatSignedModifier),
  )
}
async function load(): Promise<void> {
  isLoading.value = true
  errors.value = []
  try {
    character.value = await getCharacter(Number(route.params.id))
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isLoading.value = false
  }
}
async function remove(): Promise<void> {
  if (!character.value) return
  isDeleting.value = true
  try {
    await deleteCharacter(character.value.id)
    await router.replace('/')
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isDeleting.value = false
    confirmDelete.value = false
  }
}
async function saveGender(): Promise<void> {
  if (!character.value || !selectedGender.value) return

  isSavingGender.value = true
  errors.value = []
  try {
    await setCharacterGender(character.value.id, selectedGender.value)
    character.value.gender = selectedGender.value
    selectedGender.value = null
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isSavingGender.value = false
  }
}
onMounted(load)
</script>

<template>
  <section class="details">
    <v-btn prepend-icon="mdi-arrow-left" variant="text" to="/">{{ t('app.navigation.characters') }}</v-btn
    ><v-progress-linear v-if="isLoading" color="accent" indeterminate rounded /><v-alert
      v-for="error in errors"
      :key="error"
      type="error"
      variant="tonal"
      >{{ error }}</v-alert
    ><template v-if="character"
      ><v-dialog :model-value="mustSelectGender" persistent max-width="520">
        <v-card>
          <v-card-title>{{ t('characters.genderRequiredTitle') }}</v-card-title>
          <v-card-text>
            <p>{{ t('characters.genderRequiredText') }}</p>
            <v-alert
              v-for="error in errors"
              :key="`gender-${error}`"
              type="error"
              variant="tonal"
            >
              {{ error }}
            </v-alert>
            <v-radio-group v-model="selectedGender" :label="t('characters.gender')">
              <v-radio :label="t('characters.genders.Male')" value="Male" />
              <v-radio :label="t('characters.genders.Female')" value="Female" />
            </v-radio-group>
          </v-card-text>
          <v-card-actions>
            <v-spacer />
            <v-btn
              color="primary"
              :disabled="!selectedGender"
              :loading="isSavingGender"
              @click="saveGender"
            >
              {{ t('characters.saveGender') }}
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <header>
        <div class="character-heading">
          <CharacterAvatar
            :path="character.avatarPath"
            :alt="t('characters.avatarAlt', { name: character.name })"
            :size="160"
          />
          <div>
            <p class="eyebrow">{{ getAncestryLabel(character.ancestryType) }}</p>
            <h1>{{ character.name }}</h1>
            <p v-if="character.concept">{{ character.concept }}</p>
          </div>
        </div>
        <v-btn
          color="error"
          variant="tonal"
          prepend-icon="mdi-delete-outline"
          @click="confirmDelete = true"
          >{{ t('common.delete') }}</v-btn
        >
      </header>
      <div class="stats">
        <v-card v-if="character.derivedStatistics" elevation="0" class="hit-points-card"
          ><v-card-item :title="t('characters.maximumHitPoints')"
            ><template #prepend><v-icon color="error" icon="mdi-heart-pulse" /></template
          ></v-card-item>
          <v-card-text>
            <strong class="hit-points-card__maximum">
              {{
                t('characters.hitPointsValue', {
                  value: character.derivedStatistics.hitPoints.maximum,
                })
              }}
            </strong>
            <dl class="hit-points-card__breakdown">
              <div>
                <dt>{{ t('characters.hitPointsAncestry') }}</dt>
                <dd>
                  {{
                    t('characters.hitPointsValue', {
                      value: character.derivedStatistics.hitPoints.ancestry,
                    })
                  }}
                </dd>
              </div>
              <div>
                <dt>{{ t('characters.hitPointsClass') }}</dt>
                <dd>
                  {{
                    t('characters.hitPointsValue', {
                      value: character.derivedStatistics.hitPoints.class,
                    })
                  }}
                </dd>
              </div>
              <div>
                <dt>{{ t('characters.hitPointsConstitution') }}</dt>
                <dd>
                  {{ t('characters.hitPointsValue', {
                    value: formatSignedModifier(
                      character.derivedStatistics.hitPoints.constitutionModifier,
                    ),
                  }) }}
                </dd>
              </div>
            </dl>
          </v-card-text></v-card
        >
        <v-card v-if="character.derivedStatistics" elevation="0" class="derived-statistics-card">
          <v-card-item :title="t('statistics.savesAndPerception')">
            <template #prepend><v-icon color="primary" icon="mdi-shield-account" /></template>
          </v-card-item>
          <v-card-text class="derived-statistics-card__grid">
            <div v-for="row in statisticRows" :key="row.key" class="derived-statistics-card__item">
              <div>
                <strong>{{ row.label }}</strong>
                <small>
                  {{ getAbilityLabel(row.statistic.ability) }} ·
                  {{ getProficiencyRankLabel(row.statistic.proficiencyRank) }}
                </small>
              </div>
              <div class="derived-statistics-card__value">
                <strong>{{ formatSignedModifier(row.statistic.total) }}</strong>
                <small>
                  {{ getStatisticBreakdown(row.statistic) }}
                </small>
              </div>
            </div>
          </v-card-text>
        </v-card>
        <v-card elevation="0"
          ><v-card-item :title="t('common.details')" /><v-card-text
            ><p v-if="character.gender !== 'NotSpecified'">
              {{ t('characters.gender') }}: {{ t(`characters.genders.${character.gender}`) }}
            </p>
            <p v-if="character.age">{{ t('characters.age', { age: character.age }) }}</p>
            <p v-else>{{ t('characters.ageUnknown') }}</p></v-card-text
          ></v-card
        ><v-card v-if="character.ancestryPackage" elevation="0"
          ><v-card-item :title="t('characters.ancestryPackage')" /><v-card-text
            ><p>{{ t('characters.heritage') }}: {{ formatChoiceId(character.ancestryPackage.selectedHeritageId) }}</p>
            <p>{{ t('characters.ancestryFeat') }}: {{ formatChoiceId(character.ancestryPackage.selectedAncestryFeatId) }}</p>
            <p>{{ t('characters.vision') }}: {{ getVisionLabel(character.ancestryPackage.effectiveVision) }}</p>
            <p>{{ t('characters.baseHitPoints') }}: {{ character.ancestryPackage.effectiveBaseHitPoints }}</p>
            <p v-if="character.ancestryPackage.startingLanguageIds.length">
              {{ t('characters.languages') }}: {{ character.ancestryPackage.startingLanguageIds.map(getLanguageLabel).join(', ') }}
            </p>
            <p v-for="rule in character.ancestryPackage.grantedRules" :key="rule.ruleId">{{ rule.summary }}</p>
          </v-card-text
        ></v-card
        ><v-card v-if="character.backgroundPackage" elevation="0"
          ><v-card-item
            :title="getBackgroundLabel(character.backgroundPackage.backgroundId, character.backgroundPackage.name)"
            :subtitle="t('characters.backgroundPackage')"
          /><v-card-text
            ><p>
              {{ t('characters.backgroundBoosts') }}:
              {{ getAbilityLabel(character.backgroundPackage.restrictedBoost) }},
              {{ getAbilityLabel(character.backgroundPackage.freeBoost) }}
            </p>
            <p><strong>{{ t('characters.backgroundGrants') }}</strong></p>
            <ul>
              <li v-for="grant in character.backgroundPackage.grants" :key="grant.id">
                {{ grant.name }}
              </li>
            </ul>
          </v-card-text
        ></v-card
        ><v-card v-if="skillModifierSections.length" elevation="0" class="skill-modifiers-card">
          <v-card-item :title="t('statistics.skillsAndLore')" />
          <v-card-text>
            <section v-for="section in skillModifierSections" :key="section.key">
              <h3>{{ section.label }}</h3>
              <div class="skill-modifiers-card__grid">
                <div v-for="skill in section.items" :key="skill.targetId" class="skill-modifiers-card__item">
                  <div>
                    <strong>{{ skill.name }}</strong>
                    <small>
                      {{ getAbilityLabel(skill.ability) }} ·
                      {{ getProficiencyRankLabel(skill.proficiencyRank) }}
                    </small>
                  </div>
                  <div class="skill-modifiers-card__value">
                    <strong>{{ formatSignedModifier(skill.total) }}</strong>
                    <small>{{ getStatisticBreakdown(skill) }}</small>
                  </div>
                </div>
              </div>
            </section>
          </v-card-text>
        </v-card
        ><v-card v-if="character.classPackage" elevation="0"
          ><v-card-item
            :title="getCharacterClassLabel(character.classPackage.classId, character.classPackage.name)"
            :subtitle="t('classUi.package')"
          /><v-card-text
            ><p>{{ t('classUi.baseHitPoints') }}: {{ character.classPackage.baseHitPoints }}</p>
            <p>
              {{ t('classUi.keyAbility') }}:
              {{ getAbilityLabel(character.classPackage.keyAbility) }}
            </p>
            <template v-if="character.classPackage.rogueRacket">
              <p>
                {{ t('classUi.rogueRacket') }}: {{ character.classPackage.rogueRacket.name }}
              </p>
              <p
                v-for="effect in character.classPackage.rogueRacket.effects"
                :key="effect.id"
              >{{ effect.name }}: {{ effect.summary }}</p>
            </template>
            <template v-if="character.classPackage.huntersEdge">
              <p>
                {{ t('classUi.huntersEdge') }}: {{ character.classPackage.huntersEdge.name }}
              </p>
              <p
                v-for="effect in character.classPackage.huntersEdge.effects"
                :key="effect.id"
              >{{ effect.name }}: {{ effect.summary }} {{ t('classUi.deferredEffect') }}</p>
            </template>
            <template v-if="character.classPackage.druidicOrder">
              <p>
                {{ t('classUi.druidicOrder') }}: {{ character.classPackage.druidicOrder.name }}
              </p>
              <p
                v-for="benefit in character.classPackage.druidicOrder.benefits"
                :key="benefit.id"
              >
                {{ t(`classUi.druidicOrderBenefitKinds.${benefit.kind}`) }}:
                {{ benefit.name }}. {{ t('classUi.deferredEffect') }}
              </p>
            </template>
            <template v-if="character.classPackage.bardMuse">
              <p>
                {{ t('classUi.bardMuse') }}: {{ character.classPackage.bardMuse.name }}
              </p>
              <p
                v-for="benefit in character.classPackage.bardMuse.benefits"
                :key="benefit.id"
              >
                {{ t(`classUi.bardMuseBenefitKinds.${benefit.kind}`) }}:
                {{ benefit.name }}.
                <template v-if="benefit.kind === 'ClassFeat'">{{ t('classUi.deferredEffect') }}</template>
              </p>
            </template>
            <template v-if="character.classPackage.bardSpellLoadout">
              <p>
                {{ t('classUi.bardCantrips') }}:
                {{ character.classPackage.bardSpellLoadout.cantrips.map((spell) => spell.name).join(', ') }}
              </p>
              <p>
                {{ t('classUi.bardRepertoireSpells') }}:
                {{ character.classPackage.bardSpellLoadout.rankOneRepertoire.map((entry) => entry.spell.name).join(', ') }}
              </p>
              <p>
                {{ t('classUi.bardSpellSlots') }}:
                {{ character.classPackage.bardSpellLoadout.rankOneSpellSlotCount }}
              </p>
            </template>
            <template v-if="character.classPackage.bardComposition">
              <p>
                {{ t('classUi.compositionSpells') }}:
                {{ character.classPackage.bardComposition.compositionCantrip.name }},
                {{ character.classPackage.bardComposition.focusSpell.name }}
              </p>
              <p>
                {{ t('classUi.focusPoints') }}:
                {{ character.classPackage.bardComposition.maximumFocusPoints }}
              </p>
            </template>
            <template v-if="character.classPackage.witchPatron">
              <p>
                {{ t('classUi.witchPatron') }}: {{ character.classPackage.witchPatron.name }}
              </p>
              <p>
                {{ t('classUi.spellTradition') }}:
                {{ t(`classUi.spellTraditions.${character.classPackage.witchPatron.spellTradition}`) }}
              </p>
              <p
                v-for="benefit in character.classPackage.witchPatron.benefits"
                :key="benefit.id"
              >
                {{ t(`classUi.witchPatronBenefitKinds.${benefit.kind}`) }}:
                {{ benefit.name }} — {{ benefit.summary }}
              </p>
              <p>
                {{ t('classUi.witchPatronFamiliarSpell') }}:
                {{ character.classPackage.witchPatron.selectedFamiliarSpell.name }}
              </p>
            </template>
            <template v-if="character.classPackage.arcaneSchool">
              <p>
                {{ t('classUi.arcaneSchool') }}: {{ character.classPackage.arcaneSchool.name }}
              </p>
              <p
                v-for="benefit in character.classPackage.arcaneSchool.benefits"
                :key="benefit.id"
              >
                {{ t(`classUi.arcaneSchoolBenefitKinds.${benefit.kind}`) }}:
                {{ benefit.name }} — {{ benefit.summary }}
              </p>
            </template>
            <template v-if="character.classPackage.arcaneThesis">
              <p>
                {{ t('classUi.arcaneThesis') }}: {{ character.classPackage.arcaneThesis.name }}
              </p>
              <p
                v-for="effect in character.classPackage.arcaneThesis.effects"
                :key="effect.id"
              >
                {{ t(`classUi.arcaneThesisEffectKinds.${effect.kind}`) }}:
                {{ effect.name }} — {{ effect.summary }}
                {{ t('classUi.arcaneThesisMilestones', { levels: effect.milestoneLevels.join(', ') }) }}
              </p>
            </template>
            <template v-if="character.classPackage.clericDoctrine">
              <p>
                {{ t('classUi.clericDoctrine') }}: {{ character.classPackage.clericDoctrine.name }}
              </p>
              <p
                v-for="effect in character.classPackage.clericDoctrine.effects"
                :key="effect.id"
              >{{ effect.name }}: {{ effect.summary }}
                <template v-if="effect.deferredDependencies.length">{{ t('classUi.deferredEffect') }}</template>
              </p>
            </template>
            <template v-if="character.classPackage.deity">
              <p>{{ t('classUi.deity') }}: {{ character.classPackage.deity.name }}</p>
              <p>
                {{ t('classUi.divineFont') }}:
                {{ t(`classUi.divineFonts.${character.classPackage.deity.divineFont}`) }}
              </p>
              <p v-if="character.classPackage.deity.sanctification">
                {{ t('classUi.sanctification') }}:
                {{ t(`classUi.sanctifications.${character.classPackage.deity.sanctification}`) }}
              </p>
              <p>
                {{ t('classUi.favoredWeapon') }}:
                {{ character.classPackage.deity.favoredWeapons.map((weapon) => weapon.name).join(', ') }}
              </p>
              <p>
                {{ t('classUi.domains') }}:
                {{ character.classPackage.deity.primaryDomainIds.join(', ') }}
              </p>
              <p>
                {{ t('classUi.grantedSpells') }}:
                {{ character.classPackage.deity.grantedSpells.map((spell) => `${spell.rank}: ${spell.name}`).join(', ') }}
              </p>
            </template>
            <template v-if="character.classPackage.clericDomain">
              <p>
                {{ t('classUi.clericDomain') }}:
                {{ character.classPackage.clericDomain.name }}
              </p>
              <p>
                {{ t('classUi.initialDomainSpell') }}:
                {{ character.classPackage.clericDomain.initialFocusSpell.name }}
              </p>
            </template>
            <template v-if="character.classPackage.clericSpellLoadout">
              <p>
                {{ t('classUi.clericCantrips') }}:
                {{ character.classPackage.clericSpellLoadout.cantrips.map((spell) => spell.name).join(', ') || t('wizard.none') }}
              </p>
              <p>
                {{ t('classUi.clericPreparedSpells') }}:
                {{ character.classPackage.clericSpellLoadout.preparedSpells.map((slot) => slot.spell.name).join(', ') || t('wizard.none') }}
              </p>
              <p>
                {{ t('classUi.divineFontSpells') }}:
                {{ character.classPackage.clericSpellLoadout.divineFontSpells.map((spell) => spell.name).join(', ') || t('wizard.none') }}
              </p>
            </template>
            <template v-if="character.classPackage.clericFocusPool">
              <p>
                {{ t('classUi.focusPool') }}:
                {{ character.classPackage.clericFocusPool.focusSpell.name }}
              </p>
              <p>
                {{ t('classUi.focusPoints') }}:
                {{ character.classPackage.clericFocusPool.maximumFocusPoints }}
              </p>
              <p>
                {{ t('classUi.focusSource') }}:
                {{ character.classPackage.clericFocusPool.sourceGrantId }}
              </p>
            </template>
            <p><strong>{{ t('classUi.rules') }}</strong></p>
            <ul>
              <li v-for="rule in character.classPackage.rules" :key="rule.id">
                {{ rule.name }}
              </li>
            </ul>
          </v-card-text
        ></v-card
        ><v-card v-if="proficiencyGroups.length" elevation="0"
          ><v-card-item :title="t('classUi.initialProficiencies')" /><v-card-text
            ><section
              v-for="group in proficiencyGroups"
              :key="group.category"
              class="proficiency-group"
            >
              <strong>{{ getProficiencyCategoryLabel(group.category) }}</strong>
              <ul>
                <li v-for="proficiency in group.items" :key="proficiency.targetId">
                  {{ formatProficiency(proficiency, getProficiencyRankLabel) }}
                </li>
              </ul>
            </section></v-card-text
          ></v-card
        ><v-card v-if="character.finalFreeBoosts.length" elevation="0"
          ><v-card-item :title="t('characters.finalFreeBoosts')" /><v-card-text
            ><p>{{ character.finalFreeBoosts.map(getAbilityLabel).join(', ') }}</p></v-card-text
          ></v-card
        ><v-card elevation="0"
          ><v-card-item :title="t('characters.abilities')" /><v-card-text class="abilities"
            ><div v-for="(code, key) in abilityCodes" :key="key">
              <span>{{ getAbilityLabel(code) }}</span
              ><strong
                >{{ character.characteristics[key].value }}
                <small
                  >{{ character.characteristics[key].modifier >= 0 ? '+' : ''
                  }}{{ character.characteristics[key].modifier }}</small
                ></strong
              >
            </div></v-card-text
          ></v-card
        >
      </div></template
    ><v-dialog v-model="confirmDelete" max-width="440"
      ><v-card
        ><v-card-title>{{ t('characters.deleteTitle') }}</v-card-title
        ><v-card-text>{{ t('characters.deleteText') }}</v-card-text
        ><v-card-actions
          ><v-spacer /><v-btn variant="text" @click="confirmDelete = false">{{ t('common.cancel') }}</v-btn
          ><v-btn color="error" :loading="isDeleting" @click="remove"
            >{{ t('common.delete') }}</v-btn
          ></v-card-actions
        ></v-card
      ></v-dialog
    >
  </section>
</template>

<style scoped>
.details {
  display: grid;
  gap: 24px;
}
header {
  display: flex;
  justify-content: space-between;
  gap: 24px;
  align-items: start;
}
.character-heading {
  display: flex;
  align-items: center;
  gap: 24px;
  min-width: 0;
}
.eyebrow {
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
.stats {
  display: grid;
  grid-template-columns: minmax(0, 0.75fr) minmax(0, 1.25fr);
  gap: 16px;
}
.stats .v-card {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}
.stats .v-card-text ul {
  padding-inline-start: 1.25rem;
}
.hit-points-card__maximum {
  display: block;
  color: rgb(var(--v-theme-error));
  font-family: Georgia, 'Times New Roman', serif;
  font-size: 2.5rem;
  line-height: 1;
}
.hit-points-card__breakdown {
  display: grid;
  gap: 6px;
  margin: 18px 0 0;
}
.hit-points-card__breakdown div {
  display: flex;
  justify-content: space-between;
  gap: 12px;
}
.hit-points-card__breakdown dd {
  margin: 0;
  font-weight: 700;
}
.derived-statistics-card__grid {
  display: grid;
  gap: 12px;
}
.derived-statistics-card__item {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  border-bottom: 1px solid rgb(var(--v-theme-surface-variant));
  padding-bottom: 10px;
}
.derived-statistics-card__item > div {
  display: grid;
  gap: 2px;
}
.derived-statistics-card__value {
  text-align: right;
}
.derived-statistics-card__value strong {
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
  font-size: 1.5rem;
}
.skill-modifiers-card {
  grid-column: 1 / -1;
}
.skill-modifiers-card section + section {
  margin-top: 20px;
}
.skill-modifiers-card h3 {
  margin: 0 0 10px;
}
.skill-modifiers-card__grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 10px 20px;
}
.skill-modifiers-card__item {
  display: flex;
  justify-content: space-between;
  gap: 12px;
  border-bottom: 1px solid rgb(var(--v-theme-surface-variant));
  padding-bottom: 8px;
}
.skill-modifiers-card__item > div {
  display: grid;
  gap: 2px;
}
.skill-modifiers-card__value {
  text-align: right;
}
.skill-modifiers-card__value strong {
  color: rgb(var(--v-theme-primary));
  font-size: 1.125rem;
}
.proficiency-group + .proficiency-group {
  margin-top: 12px;
}
.proficiency-group ul {
  margin: 4px 0 0;
}
.abilities {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 14px;
}
.abilities div {
  display: flex;
  justify-content: space-between;
  border-bottom: 1px solid rgb(var(--v-theme-surface-variant));
  padding-bottom: 8px;
}
small {
  color: rgb(var(--v-theme-secondary));
}
@media (max-width: 600px) {
  header {
    flex-direction: column;
  }
  .character-heading {
    align-items: flex-start;
    flex-direction: column;
  }
  .stats {
    grid-template-columns: 1fr;
  }
  .skill-modifiers-card__grid {
    grid-template-columns: 1fr;
  }
}
</style>
