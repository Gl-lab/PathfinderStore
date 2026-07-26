import { computed, onMounted, onUnmounted, toValue, watch, type MaybeRefOrGetter } from 'vue'
import { storeToRefs } from 'pinia'
import { usePendingOperationsStore } from './pendingOperationsStore'

const PollIntervalMilliseconds = 45_000

export function usePendingOperations(
  campaignId: MaybeRefOrGetter<number>,
  characterIds: MaybeRefOrGetter<number[]>,
) {
  const store = usePendingOperationsStore()
  const state = storeToRefs(store)
  const normalizedCharacterIds = computed(() =>
    [...new Set(toValue(characterIds))].sort((left, right) => left - right),
  )
  let timer: ReturnType<typeof globalThis.setInterval> | null = null

  async function refresh(): Promise<void> {
    await store.refresh(toValue(campaignId), normalizedCharacterIds.value)
  }

  watch(
    [() => toValue(campaignId), () => normalizedCharacterIds.value.join(',')],
    () => void refresh(),
    { immediate: true },
  )
  onMounted(() => {
    timer = globalThis.setInterval(() => void refresh(), PollIntervalMilliseconds)
  })
  onUnmounted(() => {
    if (timer) globalThis.clearInterval(timer)
  })

  return { ...state, refresh }
}
