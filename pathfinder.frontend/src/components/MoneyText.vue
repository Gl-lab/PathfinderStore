<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { splitCopper } from '@/features/commerce/money'

const props = withDefaults(defineProps<{ copper: number; compact?: boolean }>(), {
  compact: false,
})
const { t } = useI18n()
const parts = computed(() => splitCopper(Math.abs(props.copper)))
const sign = computed(() => (props.copper < 0 ? '−' : ''))
</script>

<template>
  <span class="money" :class="{ 'money--compact': compact }">
    <span v-if="sign">{{ sign }}</span>
    <span v-for="part in parts" :key="part.unit">
      {{ part.value }} {{ t(`inventoryUi.money.units.${part.unit}`) }}
    </span>
  </span>
</template>

<style scoped>
.money {
  display: inline-flex;
  flex-wrap: wrap;
  gap: 0.35em;
  font-variant-numeric: tabular-nums;
}

.money--compact {
  gap: 0.2em;
}
</style>
