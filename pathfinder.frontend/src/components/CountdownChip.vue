<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { formatCountdown, secondsUntil } from '@/features/inventory/countdown'

const props = defineProps<{ expiresAtUtc: string }>()
const emit = defineEmits<{ expired: [] }>()
const { t } = useI18n()
const seconds = ref(secondsUntil(props.expiresAtUtc))
const expired = computed(() => seconds.value === 0)
let timer: ReturnType<typeof globalThis.setInterval> | null = null

function tick(): void {
  const previous = seconds.value
  seconds.value = secondsUntil(props.expiresAtUtc)
  if (previous > 0 && seconds.value === 0) {
    emit('expired')
  }
}

onMounted(() => {
  timer = globalThis.setInterval(tick, 1000)
})
onUnmounted(() => {
  if (timer) globalThis.clearInterval(timer)
})
</script>

<template>
  <v-chip :color="expired ? 'error' : undefined" size="small" variant="outlined">
    {{ expired ? t('inventoryUi.expired') : formatCountdown(seconds) }}
  </v-chip>
</template>
