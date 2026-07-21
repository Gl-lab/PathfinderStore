import { http } from '@/api/http'

export type CampaignStatus = 'Active' | 'Archived'
export type CampaignMembershipRole = 'GameMaster' | 'Player'

export interface Campaign {
  id: number
  name: string
  status: CampaignStatus
  createdByUserId: number
  createdAtUtc: string
  archivedAtUtc: string | null
  roles: CampaignMembershipRole[]
}

export async function getCampaigns(): Promise<Campaign[]> {
  return (await http.get<Campaign[]>('/api/campaigns')).data
}

export async function createCampaign(name: string): Promise<Campaign> {
  return (await http.post<Campaign>('/api/campaigns', { name })).data
}

export async function archiveCampaign(id: number): Promise<Campaign> {
  return (await http.post<Campaign>(`/api/campaigns/${id}/archive`)).data
}
