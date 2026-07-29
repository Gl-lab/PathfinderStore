import { http } from '@/api/http'

export interface LoginRequest {
  userNameOrEmail: string
  password: string
}

export interface RegisterRequest {
  userName: string
  email: string
  name: string
  surname: string
  password: string
}

interface LoginResponse {
  token: string | null
}

export async function login(request: LoginRequest): Promise<string> {
  const response = await http.post<LoginResponse>('/api/Login', request)

  if (!response.data.token) {
    throw new Error('Authorization token is missing.')
  }

  return response.data.token
}

export async function register(request: RegisterRequest): Promise<void> {
  await http.post('/api/Register', request)
}

export interface ItemCatalogCapabilities {
  canManageGlobalCatalog: boolean
  campaignId: number | null
  canManageCampaignCatalog: boolean
}

export async function getItemCatalogCapabilities(
  campaignId?: number,
): Promise<ItemCatalogCapabilities> {
  return (
    await http.get<ItemCatalogCapabilities>('/api/item-catalog-admin/capabilities', {
      params: campaignId ? { campaignId } : {},
    })
  ).data
}
