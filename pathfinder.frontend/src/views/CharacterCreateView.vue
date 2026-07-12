<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { getAbilityLabel, getAncestryChoiceLabel, getAncestryLabel } from '@/i18n/domain'
import type { AbilityCode, AncestryCode } from '@/features/characters/api'
import { createCharacter, getAncestries, type Ancestry } from '@/features/character-creation/api'

const router = useRouter()
const { t } = useI18n()
const step = ref(1)
const ancestries = ref<Ancestry[]>([])
const isLoadingAncestries = ref(true)
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
const canContinue = computed(() => {
  if (step.value === 1)
    return (
      Boolean(form.value.name.trim()) &&
      (form.value.age === null || (Number.isInteger(form.value.age) && form.value.age > 0))
  )
  if (step.value === 2) return selectedAncestry.value !== null
  if (step.value === 3) return selectedHeritage.value !== null && selectedAncestryFeat.value !== null
  if (step.value === 4) return form.value.freeBoosts.length === freeBoostSlots.value
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
function formatAbilities(types: AbilityCode[]): string {
  return types.map(getAbilityLabel).join(', ') || t('wizard.none')
}
function next(): void {
  if (canContinue.value && step.value < 5) step.value += 1
}
function previous(): void {
  if (step.value > 1) step.value -= 1
}
async function submit(): Promise<void> {
  if (!selectedAncestry.value) return
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
    })
    await router.replace('/')
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isSubmitting.value = false
  }
}
async function loadAncestries(): Promise<void> {
  isLoadingAncestries.value = true
  errorMessages.value = []
  try {
    ancestries.value = await getAncestries()
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isLoadingAncestries.value = false
  }
}
onMounted(loadAncestries)
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
    <v-progress-linear :model-value="step * 20" color="accent" height="8" rounded />
    <ol class="steps">
      <li
        v-for="(item, index) in [t('wizard.basic'), t('wizard.ancestry'), t('wizard.choices'), t('wizard.boosts'), t('wizard.review')]"
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
      ><v-card-text v-if="isLoadingAncestries"
        ><v-progress-circular indeterminate color="accent" /> {{ t('wizard.loading') }}</v-card-text
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
        <section v-else-if="selectedAncestry">
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
              v-if="form.concept"
              :title="t('wizard.selectedConcept')"
              :subtitle="form.concept" /></v-list
          ><v-alert type="info" variant="tonal"
            >{{ t('wizard.resultHint') }}</v-alert
          >
        </section></template
      ></v-card
    >
    <footer>
      <v-btn variant="text" :disabled="step === 1 || isSubmitting" @click="previous">{{ t('common.back') }}</v-btn
      ><v-spacer /><v-btn v-if="step < 5" color="primary" :disabled="!canContinue" @click="next"
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
  grid-template-columns: repeat(5, 1fr);
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
    font-size: 0.75rem;
    gap: 4px;
  }
}
</style>
