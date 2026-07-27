import { describe, expect, it } from 'vitest'
import type { ShopOffer, Wallet } from './api'
import { availableOfferQuantity, maxAffordableQuantity, purchaseShortfall } from './shop'

const offer: ShopOffer = {
  offerKey: 'offer',
  campaignId: 1,
  shopId: 2,
  kind: 'Catalog',
  itemConfigurationId: 3,
  itemInstanceKey: null,
  itemName: 'Potion',
  itemLevel: 1,
  availableQuantity: 8,
  reservedQuantity: 2,
  unitPriceCopper: 125,
  status: 'Active',
  version: 0,
}

const wallet: Wallet = {
  campaignId: 1,
  characterId: 4,
  balanceCopper: 500,
  reservedCopper: 125,
  availableCopper: 375,
  version: 1,
  entries: [],
}

describe('shop purchase limits', () => {
  it('uses unreserved stock as the available quantity', () => {
    expect(availableOfferQuantity(offer)).toBe(6)
  })

  it('limits quantity by available wallet funds', () => {
    expect(maxAffordableQuantity(offer, wallet)).toBe(3)
  })

  it('calculates the exact copper shortfall', () => {
    expect(purchaseShortfall(offer, 4, wallet)).toBe(125)
  })
})
