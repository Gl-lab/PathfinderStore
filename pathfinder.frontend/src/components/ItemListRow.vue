<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { ItemCategory } from '@/features/inventory/api'
import { getItemCategoryIcon } from '@/features/inventory/itemPresentation'
import { formatBulk } from '@/features/inventory/bulk'

defineProps<{
  category: ItemCategory
  name: string
  quantity: number
  bulkTenths: number
  subtitle?: string
  selected?: boolean
}>()
const { t } = useI18n()
</script>

<template>
  <v-list-item
    :active="selected"
    :prepend-icon="getItemCategoryIcon(category)"
    color="primary"
    rounded="lg"
  >
    <v-list-item-title
      >{{ name }} <span v-if="quantity > 1">×{{ quantity }}</span></v-list-item-title
    >
    <v-list-item-subtitle>
      {{ subtitle ? `${subtitle} · ` : ''
      }}{{ t('inventoryUi.bulk.value', { value: formatBulk(bulkTenths) }) }}
    </v-list-item-subtitle>
    <template #append>
      <slot name="append" />
    </template>
  </v-list-item>
</template>
