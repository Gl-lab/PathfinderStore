<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { bulkProgress, formatBulk } from '@/features/inventory/bulk'

const props = defineProps<{
  totalTenths: number
  encumberedTenths: number
  maximumTenths: number
}>()
const { t } = useI18n()
const progress = computed(() => bulkProgress(props.totalTenths, props.maximumTenths))
const threshold = computed(() => bulkProgress(props.encumberedTenths, props.maximumTenths))
const overloaded = computed(() => props.totalTenths > props.encumberedTenths)
</script>

<template>
  <div class="bulk-meter">
    <div class="bulk-meter__track">
      <v-progress-linear
        :color="overloaded ? 'error' : 'secondary'"
        height="12"
        :model-value="progress"
        rounded
      />
      <span class="bulk-meter__threshold" :style="{ left: `${threshold}%` }" />
    </div>
    <p>
      {{
        t('inventoryUi.bulk.summary', {
          total: formatBulk(totalTenths),
          encumbered: formatBulk(encumberedTenths),
          maximum: formatBulk(maximumTenths),
        })
      }}
    </p>
  </div>
</template>

<style scoped>
.bulk-meter {
  display: grid;
  gap: 8px;
}

.bulk-meter__track {
  position: relative;
}

.bulk-meter__threshold {
  position: absolute;
  top: -3px;
  width: 2px;
  height: 18px;
  background: rgb(var(--v-theme-on-surface));
  transform: translateX(-1px);
}

.bulk-meter p {
  margin: 0;
  color: rgb(var(--v-theme-on-surface-variant));
  font-size: 0.875rem;
}
</style>
