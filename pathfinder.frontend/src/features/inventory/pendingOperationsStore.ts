import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { getApiErrorMessages } from '@/api/errors'
import { getPurchaseReservations, type PurchaseReservation } from '@/features/commerce/api'
import { getPartyExchanges, getPartyGifts, type PartyExchange, type PartyGift } from './api'
import {
  uniquePendingExchanges,
  uniquePendingGifts,
  uniquePendingReservations,
} from './pendingOperations'

export const usePendingOperationsStore = defineStore('pendingOperations', () => {
  const gifts = ref<PartyGift[]>([])
  const exchanges = ref<PartyExchange[]>([])
  const reservations = ref<PurchaseReservation[]>([])
  const errors = ref<string[]>([])
  const isLoading = ref(false)
  const requestSequence = ref(0)
  const count = computed(
    () => gifts.value.length + exchanges.value.length + reservations.value.length,
  )

  async function refresh(campaignId: number, characterIds: number[]): Promise<void> {
    const currentRequest = ++requestSequence.value
    if (!characterIds.length) {
      gifts.value = []
      exchanges.value = []
      reservations.value = []
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
        getPurchaseReservations(campaignId, characterId, 'Active'),
      ]),
    )
    if (currentRequest !== requestSequence.value) return

    const giftItems: PartyGift[] = []
    const exchangeItems: PartyExchange[] = []
    const reservationItems: PurchaseReservation[] = []
    const resultErrors: string[] = []
    results.forEach((result, index) => {
      if (result.status === 'rejected') {
        resultErrors.push(...getApiErrorMessages(result.reason))
      } else if (index % 3 === 0) {
        giftItems.push(...(result.value as PartyGift[]))
      } else if (index % 3 === 1) {
        exchangeItems.push(...(result.value as PartyExchange[]))
      } else {
        reservationItems.push(...(result.value as PurchaseReservation[]))
      }
    })

    gifts.value = uniquePendingGifts(giftItems)
    exchanges.value = uniquePendingExchanges(exchangeItems)
    reservations.value = uniquePendingReservations(reservationItems)
    errors.value = [...new Set(resultErrors)]
    isLoading.value = false
  }

  return { gifts, exchanges, reservations, errors, isLoading, count, refresh }
})
