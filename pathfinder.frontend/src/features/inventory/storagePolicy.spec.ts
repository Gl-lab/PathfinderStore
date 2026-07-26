import { describe, expect, it } from 'vitest'
import { canWithdrawFromStorage } from './storagePolicy'

describe('canWithdrawFromStorage', () => {
  it('does not preemptively block an unconfigured policy', () => {
    expect(canWithdrawFromStorage('Unconfigured', false)).toBe(true)
  })

  it('allows free member withdrawals', () => {
    expect(canWithdrawFromStorage('FreeForMembers', false)).toBe(true)
  })

  it('restricts Game Master policy withdrawals', () => {
    expect(canWithdrawFromStorage('GameMasterOnly', false)).toBe(false)
    expect(canWithdrawFromStorage('GameMasterOnly', true)).toBe(true)
  })
})
