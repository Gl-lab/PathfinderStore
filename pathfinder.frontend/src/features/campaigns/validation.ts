export const campaignNameMaxLength = 200

export function normalizeCampaignName(name: string): string {
  return name.trim()
}

export function isCampaignNameValid(name: string): boolean {
  const normalizedName = normalizeCampaignName(name)
  return normalizedName.length > 0 && normalizedName.length <= campaignNameMaxLength
}
