import { describe, expect, it } from 'vitest'
import type { InventoryOperationItem, PartyExchange } from './api'
import {
  exchangeItemsForViewer,
  reconcileExchangeLines,
  toCreateExchangeLines,
  type ExchangeDraftLine,
} from './exchange'

function item(key: string, version: number, name = key): InventoryOperationItem {
  return {
    itemInstanceKey: key,
    version,
    quantity: 1,
    name,
    primaryCategory: 'Weapon',
    bulkTenths: 10,
  }
}

describe('exchange draft', () => {
  it('keeps reservation operation ids when creating the request', () => {
    const lines: ExchangeDraftLine[] = [
      { fromCharacterId: 1, item: item('sword', 3), reservationOperationId: 'reserve-1' },
    ]

    expect(toCreateExchangeLines(lines)).toEqual([
      {
        fromCharacterId: 1,
        itemInstanceKey: 'sword',
        expectedItemVersion: 3,
        reservationOperationId: 'reserve-1',
      },
    ])
  })

  it('removes only missing or changed lines after a conflict', () => {
    const lines: ExchangeDraftLine[] = [
      { fromCharacterId: 1, item: item('sword', 3), reservationOperationId: 'reserve-1' },
      {
        fromCharacterId: 2,
        item: item('shield', 2, 'Shield'),
        reservationOperationId: 'reserve-2',
      },
    ]

    const result = reconcileExchangeLines(
      lines,
      new Map([
        [1, [item('sword', 3)]],
        [2, [item('shield', 4, 'Shield')]],
      ]),
    )

    expect(result.lines.map((line) => line.item.itemInstanceKey)).toEqual(['sword'])
    expect(result.removedNames).toEqual(['Shield'])
    expect(result.lines[0]?.reservationOperationId).toBe('reserve-1')
  })

  it('uses the viewer perspective for both columns', () => {
    const exchange = {
      items: [
        { fromCharacterId: 1, item: item('sword', 3) },
        { fromCharacterId: 2, item: item('shield', 2) },
      ],
    } as PartyExchange

    const perspective = exchangeItemsForViewer(exchange, 2)

    expect(perspective.giving[0]?.item.itemInstanceKey).toBe('shield')
    expect(perspective.receiving[0]?.item.itemInstanceKey).toBe('sword')
  })
})
