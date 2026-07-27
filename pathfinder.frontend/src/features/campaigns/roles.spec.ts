import { describe, expect, it } from 'vitest'
import { isOnlyCampaignGameMaster } from './roles'

describe('campaign roles', () => {
  it('identifies the only Game Master', () => {
    const members = [
      { userId: 1, roles: ['GameMaster' as const, 'Player' as const] },
      { userId: 2, roles: ['Player' as const] },
    ]

    expect(isOnlyCampaignGameMaster(members, 1)).toBe(true)
    expect(isOnlyCampaignGameMaster(members, 2)).toBe(false)
  })

  it('allows a Game Master role to be revoked when another remains', () => {
    const members = [
      { userId: 1, roles: ['GameMaster' as const] },
      { userId: 2, roles: ['GameMaster' as const] },
    ]

    expect(isOnlyCampaignGameMaster(members, 1)).toBe(false)
  })
})
