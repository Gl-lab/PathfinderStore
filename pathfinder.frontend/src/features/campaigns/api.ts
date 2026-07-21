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
  members: { userId: number; roles: CampaignMembershipRole[] }[]
}

export interface CampaignInvitation {
  id: number
  campaignId: number
  campaignName: string
  invitedByUserId: number
  createdAtUtc: string
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

export async function getCampaignInvitations(): Promise<CampaignInvitation[]> {
  return (await http.get<CampaignInvitation[]>('/api/campaign-membership/invitations')).data
}

export async function inviteCampaignMember(id: number, userName: string): Promise<void> {
  await http.post(`/api/campaign-membership/campaigns/${id}/invitations`, { userName })
}

export async function respondToCampaignInvitation(
  invitationId: number,
  accept: boolean,
): Promise<Campaign | null> {
  const action = accept ? 'accept' : 'decline'
  return (
    await http.post<Campaign | null>(
      `/api/campaign-membership/invitations/${invitationId}/${action}`,
    )
  ).data
}

export async function leaveCampaign(id: number): Promise<void> {
  await http.post(`/api/campaign-membership/campaigns/${id}/leave`)
}

export async function changeCampaignRole(
  campaignId: number,
  memberUserId: number,
  role: CampaignMembershipRole,
  assign: boolean,
): Promise<Campaign> {
  const url = `/api/campaign-membership/campaigns/${campaignId}/members/${memberUserId}/roles/${role}`
  return (assign ? await http.put<Campaign>(url) : await http.delete<Campaign>(url)).data
}
