<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { combineMoneyParts } from '@/features/commerce/money'

const model = defineModel<number>({ required: true })
const { t } = useI18n()

function update(gold: number, silver: number, copper: number): void {
  model.value = combineMoneyParts(gold, silver, copper)
}

const gold = computed({
  get: () => Math.floor(Math.max(0, Math.trunc(model.value)) / 100),
  set: (value: number) => update(value, silver.value, copper.value),
})
const silver = computed({
  get: () => Math.floor((Math.max(0, Math.trunc(model.value)) % 100) / 10),
  set: (value: number) => update(gold.value, value, copper.value),
})
const copper = computed({
  get: () => Math.max(0, Math.trunc(model.value)) % 10,
  set: (value: number) => update(gold.value, silver.value, value),
})
</script>

<template>
  <div class="money-input">
    <v-number-input
      v-model="gold"
      control-variant="stacked"
      :label="t('inventoryUi.money.gold')"
      :min="0"
    />
    <v-number-input
      v-model="silver"
      control-variant="stacked"
      :label="t('inventoryUi.money.silver')"
      :min="0"
    />
    <v-number-input
      v-model="copper"
      control-variant="stacked"
      :label="t('inventoryUi.money.copper')"
      :min="0"
    />
  </div>
</template>

<style scoped>
.money-input {
  display: grid;
  grid-template-columns: repeat(3, minmax(90px, 1fr));
  gap: 8px;
}
</style>
