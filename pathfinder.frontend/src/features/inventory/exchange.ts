import type { InventoryOperationItem, PartyExchange } from './api'

export interface ExchangeDraftLine {
  fromCharacterId: number
  item: InventoryOperationItem
  reservationOperationId: string
}

export function reconcileExchangeLines(
  lines: ExchangeDraftLine[],
  availableItems: Map<number, InventoryOperationItem[]>,
): { lines: ExchangeDraftLine[]; removedNames: string[] } {
  const removedNames: string[] = []
  const retained = lines.flatMap((line) => {
    const current = availableItems
      .get(line.fromCharacterId)
      ?.find((item) => item.itemInstanceKey === line.item.itemInstanceKey)
    if (!current || current.version !== line.item.version) {
      removedNames.push(line.item.name)
      return []
    }

    return [{ ...line, item: current }]
  })
  return { lines: retained, removedNames }
}

export function exchangeItemsForViewer(
  exchange: PartyExchange,
  viewerCharacterId: number,
): { giving: PartyExchange['items']; receiving: PartyExchange['items'] } {
  return {
    giving: exchange.items.filter((line) => line.fromCharacterId === viewerCharacterId),
    receiving: exchange.items.filter((line) => line.fromCharacterId !== viewerCharacterId),
  }
}

export function toCreateExchangeLines(lines: ExchangeDraftLine[]): {
  fromCharacterId: number
  itemInstanceKey: string
  expectedItemVersion: number
  reservationOperationId: string
}[] {
  return lines.map((line) => ({
    fromCharacterId: line.fromCharacterId,
    itemInstanceKey: line.item.itemInstanceKey,
    expectedItemVersion: line.item.version,
    reservationOperationId: line.reservationOperationId,
  }))
}
