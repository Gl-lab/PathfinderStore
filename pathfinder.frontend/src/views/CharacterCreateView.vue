<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { ancestryNames } from '@/features/characters/api'
import { createCharacter, getAncestries, type Ancestry } from '@/features/character-creation/api'

const router = useRouter()
const step = ref(1)
const ancestries = ref<Ancestry[]>([])
const isLoadingAncestries = ref(true)
const isSubmitting = ref(false)
const errorMessages = ref<string[]>([])
const form = ref({
  name: '',
  concept: '',
  age: null as number | null,
  ancestryType: null as number | null,
  freeBoosts: [] as number[],
})
const abilityLabels: Record<number, string> = {
  1: 'Сила',
  2: 'Ловкость',
  3: 'Выносливость',
  4: 'Интеллект',
  5: 'Мудрость',
  6: 'Харизма',
}
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
      .filter((type): type is number => type !== null) ?? [],
)
const canContinue = computed(() => {
  if (step.value === 1)
    return (
      Boolean(form.value.name.trim()) &&
      (form.value.age === null || (Number.isInteger(form.value.age) && form.value.age > 0))
    )
  if (step.value === 2) return selectedAncestry.value !== null
  if (step.value === 3) return form.value.freeBoosts.length === freeBoostSlots.value
  return true
})

function selectAncestry(type: number | null): void {
  form.value.ancestryType = type
  form.value.freeBoosts = []
}
function isBoostDisabled(type: number): boolean {
  return (
    fixedBoosts.value.includes(type) ||
    (!form.value.freeBoosts.includes(type) && form.value.freeBoosts.length >= freeBoostSlots.value)
  )
}
function formatAbilities(types: number[]): string {
  return types.map((type) => abilityLabels[type]).join(', ') || 'нет'
}
function next(): void {
  if (canContinue.value && step.value < 4) step.value += 1
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
        <p class="eyebrow">Новый герой</p>
        <h1>Создание персонажа</h1>
        <p>Сервер применит правила родословной и рассчитает итоговые характеристики.</p>
      </div>
      <v-btn variant="text" to="/">Отмена</v-btn>
    </header>
    <v-progress-linear :model-value="step * 25" color="accent" height="8" rounded />
    <ol class="steps">
      <li
        v-for="(item, index) in ['Основное', 'Родословная', 'Бусты', 'Проверка']"
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
        ><v-progress-circular indeterminate color="accent" /> Загружаем родословные…</v-card-text
      ><template v-else
        ><section v-if="step === 1">
          <h2>Основное</h2>
          <p class="hint">Укажите имя героя. Концепция и возраст необязательны.</p>
          <v-text-field
            v-model="form.name"
            label="Имя персонажа"
            :rules="[(value) => Boolean(value?.trim()) || 'Укажите имя']"
            required
          /><v-textarea
            v-model="form.concept"
            label="Краткая концепция"
            counter="1000"
            maxlength="1000"
            hint="Кем является ваш персонаж?"
            persistent-hint
          /><v-text-field v-model.number="form.age" label="Возраст" type="number" min="1" />
        </section>
        <section v-else-if="step === 2">
          <h2>Родословная</h2>
          <p class="hint">Выберите происхождение героя. Его правила определит сервер.</p>
          <v-radio-group :model-value="form.ancestryType" @update:model-value="selectAncestry"
            ><v-radio v-for="ancestry in ancestries" :key="ancestry.type" :value="ancestry.type"
              ><template #label
                ><div>
                  <strong>{{ ancestry.name || ancestryNames[ancestry.type] }}</strong>
                  <p class="radio-detail">
                    {{ ancestry.baseHitPoints }} HP · скорость {{ ancestry.baseSpeed }} ·
                    {{ ancestry.abilityBoosts.filter((boost) => !boost.isFree).length }} фикс. буста
                  </p>
                </div></template
              ></v-radio
            ></v-radio-group
          >
        </section>
        <section v-else-if="step === 3 && selectedAncestry">
          <h2>Свободные бусты</h2>
          <p class="hint">
            Выберите {{ freeBoostSlots }}:
            {{ freeBoostSlots === 1 ? 'характеристику' : 'разные характеристики' }}. Фиксированные
            бусты: {{ formatAbilities(fixedBoosts) }}.
          </p>
          <v-checkbox
            v-for="(label, type) in abilityLabels"
            :key="type"
            v-model="form.freeBoosts"
            :value="Number(type)"
            :label="label"
            :disabled="isBoostDisabled(Number(type))"
            hide-details
          />
        </section>
        <section v-else-if="selectedAncestry">
          <h2>Проверка</h2>
          <v-list lines="two"
            ><v-list-item title="Имя" :subtitle="form.name" /><v-list-item
              title="Родословная"
              :subtitle="
                selectedAncestry.name || ancestryNames[selectedAncestry.type]
              " /><v-list-item
              title="Свободные бусты"
              :subtitle="formatAbilities(form.freeBoosts)" /><v-list-item
              v-if="form.concept"
              title="Концепция"
              :subtitle="form.concept" /></v-list
          ><v-alert type="info" variant="tonal"
            >Итоговые характеристики рассчитывает сервер после создания.</v-alert
          >
        </section></template
      ></v-card
    >
    <footer>
      <v-btn variant="text" :disabled="step === 1 || isSubmitting" @click="previous">Назад</v-btn
      ><v-spacer /><v-btn v-if="step < 4" color="primary" :disabled="!canContinue" @click="next"
        >Далее</v-btn
      ><v-btn v-else color="accent" :loading="isSubmitting" @click="submit"
        >Создать персонажа</v-btn
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
  grid-template-columns: repeat(4, 1fr);
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
