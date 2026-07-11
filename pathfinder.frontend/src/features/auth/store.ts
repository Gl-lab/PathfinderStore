import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import { login, type LoginRequest } from '@/features/auth/api'

const tokenStorageKey = 'pathfinder.auth-token'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem(tokenStorageKey))
  const isAuthenticated = computed(() => Boolean(token.value))

  async function signIn(request: LoginRequest): Promise<void> {
    token.value = await login(request)
    localStorage.setItem(tokenStorageKey, token.value)
  }

  function signOut(): void {
    token.value = null
    localStorage.removeItem(tokenStorageKey)
  }

  return { token, isAuthenticated, signIn, signOut }
})
