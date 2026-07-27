import { describe, expect, it } from 'vitest'
import {
  campaignNameMaxLength,
  campaignPartyNameMaxLength,
  campaignUserNameMaxLength,
  isCampaignCharacterIdValid,
  isCampaignNameValid,
  isCampaignPartyNameValid,
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

  it('validates a party name with the backend length limit', () => {
    expect(isCampaignPartyNameValid('  Heroes  ')).toBe(true)
    expect(isCampaignPartyNameValid(' ')).toBe(false)
    expect(isCampaignPartyNameValid('x'.repeat(campaignPartyNameMaxLength + 1))).toBe(false)
  })

  it('accepts only positive integer character identifiers', () => {
    expect(isCampaignCharacterIdValid(42)).toBe(true)
    expect(isCampaignCharacterIdValid(null)).toBe(false)
    expect(isCampaignCharacterIdValid(0)).toBe(false)
    expect(isCampaignCharacterIdValid(1.5)).toBe(false)
  })
})
