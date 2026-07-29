import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import {
  getItemCatalogCapabilities,
  login,
  type ItemCatalogCapabilities,
  type LoginRequest,
} from '@/features/auth/api'

const tokenStorageKey = 'pathfinder.auth-token'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem(tokenStorageKey))
  const isAuthenticated = computed(() => Boolean(token.value))
  const capabilities = ref<ItemCatalogCapabilities | null>(null)

  async function signIn(request: LoginRequest): Promise<void> {
    token.value = await login(request)
    localStorage.setItem(tokenStorageKey, token.value)
    capabilities.value = null
  }

  function signOut(): void {
    token.value = null
    capabilities.value = null
    localStorage.removeItem(tokenStorageKey)
  }

  async function loadCapabilities(): Promise<void> {
    if (!isAuthenticated.value || capabilities.value) return
    capabilities.value = await getItemCatalogCapabilities()
  }

  return { token, isAuthenticated, capabilities, signIn, signOut, loadCapabilities }
})
