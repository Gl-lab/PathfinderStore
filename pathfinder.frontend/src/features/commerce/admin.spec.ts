import { describe, expect, it } from 'vitest'
import {
  canForceMove,
  catalogConfigurationOptions,
  containerTitle,
  signedAdjustment,
} from './admin'
import type { AdminInventoryContainer, PublishedItemRevision } from './adminApi'

describe('commerce administration helpers', () => {
  it('flattens only usable catalog configurations', () => {
    const revisions = [
      {
        name: 'Healing Potion',
        configurations: [
          {
            itemConfigurationId: 12,
            size: 'Medium',
            materialType: 'Standard',
            materialGrade: 'Standard',
          },
        ],
      },
      { name: 'Unconfigured', configurations: [] },
    ] as PublishedItemRevision[]

    expect(catalogConfigurationOptions(revisions)).toEqual([
      { value: 12, title: 'Healing Potion · Medium · Standard' },
    ])
  })

  it('normalizes the adjustment sign', () => {
    expect(signedAdjustment(250, 'credit')).toBe(250)
    expect(signedAdjustment(250, 'debit')).toBe(-250)
    expect(signedAdjustment(-10, 'credit')).toBe(0)
  })

  it('requires a complete force move to a different container', () => {
    expect(canForceMove('source', 'item', 'target', 'Correction')).toBe(true)
    expect(canForceMove('source', 'item', 'source', 'Correction')).toBe(false)
    expect(canForceMove('source', 'item', 'target', ' ')).toBe(false)
  })

  it('formats unnamed container owners without losing identity', () => {
    expect(
      containerTitle({
        ownerKind: 'World',
        ownerId: 9,
        ownerName: null,
      } as AdminInventoryContainer),
    ).toBe('World: #9')
  })
})
