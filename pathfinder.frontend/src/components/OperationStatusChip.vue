<script setup lang="ts">
import { computed } from 'vue'
import { getOperationStatusLabel } from '@/i18n/domain'
import type { OperationStatus } from '@/features/inventory/api'

const props = defineProps<{ status: OperationStatus }>()
const color = computed(
  () =>
    ({
      Pending: 'secondary',
      Reserved: 'info',
      Completed: 'success',
      Cancelled: 'default',
      Expired: 'error',
    })[props.status],
)
</script>

<template>
  <v-chip :color="color" size="small" variant="tonal">
    {{ getOperationStatusLabel(status) }}
  </v-chip>
</template>
