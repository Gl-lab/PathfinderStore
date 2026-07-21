import { describe, expect, it } from 'vitest'
import {
  campaignNameMaxLength,
  campaignUserNameMaxLength,
  isCampaignNameValid,
  isCampaignUserNameValid,
  normalizeCampaignName,
} from './validation'

describe('campaign name validation', () => {
  it('trims a valid name', () => {
    expect(normalizeCampaignName('  Abomination Vaults  ')).toBe('Abomination Vaults')
    expect(isCampaignNameValid('  Abomination Vaults  ')).toBe(true)
  })

  it('rejects empty and oversized names', () => {
    expect(isCampaignNameValid('   ')).toBe(false)
    expect(isCampaignNameValid('x'.repeat(campaignNameMaxLength + 1))).toBe(false)
  })

  it('validates an invited user name', () => {
    expect(isCampaignUserNameValid(' memberuser ')).toBe(true)
    expect(isCampaignUserNameValid(' ')).toBe(false)
    expect(isCampaignUserNameValid('x'.repeat(campaignUserNameMaxLength + 1))).toBe(false)
  })
})
