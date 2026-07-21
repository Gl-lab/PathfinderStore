export const campaignNameMaxLength = 200
export const campaignUserNameMaxLength = 256

export function normalizeCampaignName(name: string): string {
  return name.trim()
}

export function isCampaignNameValid(name: string): boolean {
  const normalizedName = normalizeCampaignName(name)
  return normalizedName.length > 0 && normalizedName.length <= campaignNameMaxLength
}

export function isCampaignUserNameValid(userName: string): boolean {
  const normalizedUserName = userName.trim()
  return normalizedUserName.length > 0 && normalizedUserName.length <= campaignUserNameMaxLength
}
