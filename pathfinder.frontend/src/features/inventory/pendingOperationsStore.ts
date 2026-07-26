import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { getApiErrorMessages } from '@/api/errors'
import { getPartyExchanges, getPartyGifts, type PartyExchange, type PartyGift } from './api'
import { uniquePendingExchanges, uniquePendingGifts } from './pendingOperations'

export const usePendingOperationsStore = defineStore('pendingOperations', () => {
  const gifts = ref<PartyGift[]>([])
  const exchanges = ref<PartyExchange[]>([])
  const errors = ref<string[]>([])
  const isLoading = ref(false)
  const requestSequence = ref(0)
  const count = computed(() => gifts.value.length + exchanges.value.length)

  async function refresh(campaignId: number, characterIds: number[]): Promise<void> {
    const currentRequest = ++requestSequence.value
    if (!characterIds.length) {
      gifts.value = []
      exchanges.value = []
      errors.value = []
      isLoading.value = false
      return
    }

    isLoading.value = true
    errors.value = []
    const results = await Promise.allSettled(
      characterIds.flatMap((characterId) => [
        getPartyGifts(campaignId, characterId, 'Incoming'),
        getPartyExchanges(campaignId, characterId),
      ]),
    )
    if (currentRequest !== requestSequence.value) return

    const giftItems: PartyGift[] = []
    const exchangeItems: PartyExchange[] = []
    const resultErrors: string[] = []
    results.forEach((result, index) => {
      if (result.status === 'rejected') {
        resultErrors.push(...getApiErrorMessages(result.reason))
      } else if (index % 2 === 0) {
        giftItems.push(...(result.value as PartyGift[]))
      } else {
        exchangeItems.push(...(result.value as PartyExchange[]))
      }
    })

    gifts.value = uniquePendingGifts(giftItems)
    exchanges.value = uniquePendingExchanges(exchangeItems)
    errors.value = [...new Set(resultErrors)]
    isLoading.value = false
  }

  return { gifts, exchanges, errors, isLoading, count, refresh }
})
