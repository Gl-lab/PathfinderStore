<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { useAuthStore } from '@/features/auth/store'

const auth = useAuthStore()
const route = useRoute()
const router = useRouter()
const userNameOrEmail = ref('')
const password = ref('')
const errorMessages = ref<string[]>([])
const isSubmitting = ref(false)

async function submit(): Promise<void> {
  errorMessages.value = []
  isSubmitting.value = true

  try {
    await auth.signIn({ userNameOrEmail: userNameOrEmail.value.trim(), password: password.value })
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : '/'
    await router.replace(redirect)
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <main class="auth-page">
    <v-card class="auth-card" elevation="0">
      <v-card-item>
        <p class="eyebrow">Character Ledger</p>
        <v-card-title>С возвращением</v-card-title>
        <v-card-subtitle>Войдите, чтобы продолжить приключение.</v-card-subtitle>
      </v-card-item>
      <v-card-text>
        <v-alert
          v-for="message in errorMessages"
          :key="message"
          type="error"
          variant="tonal"
          class="mb-3"
          >{{ message }}</v-alert
        >
        <v-form @submit.prevent="submit">
          <v-text-field
            v-model="userNameOrEmail"
            label="Никнейм или эл. почта"
            autocomplete="username"
            required
          />
          <v-text-field
            v-model="password"
            label="Пароль"
            type="password"
            autocomplete="current-password"
            required
          />
          <v-btn type="submit" color="primary" block size="large" :loading="isSubmitting"
            >Войти</v-btn
          >
        </v-form>
      </v-card-text>
      <v-card-actions class="px-4 pb-5"
        >Нет аккаунта?
        <v-btn variant="text" color="primary" to="/register"
          >Зарегистрироваться</v-btn
        ></v-card-actions
      >
    </v-card>
  </main>
</template>

<style scoped>
.auth-page {
  display: grid;
  min-height: calc(100vh - 160px);
  place-items: center;
}
.auth-card {
  width: min(100%, 460px);
  border: 1px solid rgb(var(--v-theme-surface-variant));
}
.eyebrow {
  color: rgb(var(--v-theme-secondary));
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}
</style>
