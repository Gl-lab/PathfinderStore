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
    throw new Error('Сервер не вернул токен авторизации.')
  }

  return response.data.token
}

export async function register(request: RegisterRequest): Promise<void> {
  await http.post('/api/Register', request)
}
