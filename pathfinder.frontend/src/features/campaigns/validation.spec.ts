import { describe, expect, it } from 'vitest'
import { campaignNameMaxLength, isCampaignNameValid, normalizeCampaignName } from './validation'

describe('campaign name validation', () => {
  it('trims a valid name', () => {
    expect(normalizeCampaignName('  Abomination Vaults  ')).toBe('Abomination Vaults')
    expect(isCampaignNameValid('  Abomination Vaults  ')).toBe(true)
  })

  it('rejects empty and oversized names', () => {
    expect(isCampaignNameValid('   ')).toBe(false)
    expect(isCampaignNameValid('x'.repeat(campaignNameMaxLength + 1))).toBe(false)
  })
})
