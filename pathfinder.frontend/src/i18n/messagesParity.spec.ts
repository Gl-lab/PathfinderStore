import { describe, expect, it } from 'vitest'
import { messages } from './messages'

function messageKeys(value: unknown, prefix = ''): string[] {
  if (!value || typeof value !== 'object') return [prefix]

  return Object.entries(value).flatMap(([key, child]) =>
    messageKeys(child, prefix ? `${prefix}.${key}` : key),
  )
}

describe('message catalog parity', () => {
  it.each(['inventoryUi', 'commerceUi', 'tradeUi'] as const)(
    'keeps Russian and English %s keys aligned',
    (namespace) => {
      expect(messageKeys(messages.ru[namespace]).sort()).toEqual(
        messageKeys(messages.en[namespace]).sort(),
      )
    },
  )
})
