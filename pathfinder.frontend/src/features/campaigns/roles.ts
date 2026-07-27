import type { Campaign } from './api'

export function isOnlyCampaignGameMaster(
  members: Campaign['members'],
  userId: number,
): boolean {
  const gameMasters = members.filter((member) => member.roles.includes('GameMaster'))
  return gameMasters.length === 1 && gameMasters[0]?.userId === userId
}
