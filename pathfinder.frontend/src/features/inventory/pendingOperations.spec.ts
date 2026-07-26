import { describe, expect, it } from 'vitest'
import type { PartyExchange, PartyGift } from './api'
import { uniquePendingExchanges, uniquePendingGifts } from './pendingOperations'

describe('pending operation aggregation', () => {
  it('deduplicates gifts by idempotency key', () => {
    const gift = { gift: { giftKey: 'gift-1' } } as PartyGift
    expect(uniquePendingGifts([gift, gift])).toHaveLength(1)
  })

  it('deduplicates exchanges visible to multiple controlled characters', () => {
    const exchange = { exchange: { exchangeKey: 'exchange-1' } } as PartyExchange
    expect(uniquePendingExchanges([exchange, exchange])).toHaveLength(1)
  })
})
