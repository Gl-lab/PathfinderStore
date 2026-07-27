<script setup lang="ts">
import { computed } from 'vue'
import { getOperationStatusLabel } from '@/i18n/domain'
import type { OperationStatus } from '@/features/inventory/api'
import type { ShopOfferStatus } from '@/features/commerce/api'

type DisplayStatus = OperationStatus | ShopOfferStatus

const props = defineProps<{ status: DisplayStatus }>()
const color = computed(
  () =>
    ({
      Pending: 'secondary',
      Reserved: 'info',
      Completed: 'success',
      Cancelled: 'default',
      Expired: 'error',
      Active: 'success',
      Withdrawn: 'default',
      SoldOut: 'error',
    })[props.status],
)
</script>

<template>
  <v-chip :color="color" size="small" variant="tonal">
    {{ getOperationStatusLabel(status) }}
  </v-chip>
</template>
