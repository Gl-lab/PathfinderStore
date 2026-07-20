import { describe, expect, it } from 'vitest'
import { formatSignedModifier } from '@/features/characters/hitPoints'
import { formatStatisticBreakdown } from '@/features/characters/statistics'

describe('derived statistic presentation', () => {
  it('formats the server-provided ability and proficiency breakdown without recalculating total', () => {
    expect(
      formatStatisticBreakdown(
        {
          abilityModifier: 3,
          proficiencyBonus: 5,
        },
        formatSignedModifier,
      ),
    ).toEqual({ ability: '+3', proficiency: '+5' })
  })
})
