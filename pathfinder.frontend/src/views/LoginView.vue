<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { useAuthStore } from '@/features/auth/store'

const auth = useAuthStore()
const { t } = useI18n()
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
        <p class="eyebrow">{{ t('auth.ledger') }}</p>
        <v-card-title>{{ t('auth.welcome') }}</v-card-title>
        <v-card-subtitle>{{ t('auth.signInPrompt') }}</v-card-subtitle>
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
            :label="t('auth.userNameOrEmail')"
            autocomplete="username"
            required
          />
          <v-text-field
            v-model="password"
            :label="t('auth.password')"
            type="password"
            autocomplete="current-password"
            required
          />
          <v-btn type="submit" color="primary" block size="large" :loading="isSubmitting"
            >{{ t('app.auth.signIn') }}</v-btn
          >
        </v-form>
      </v-card-text>
      <v-card-actions class="px-4 pb-5"
        >{{ t('auth.noAccount') }}
        <v-btn variant="text" color="primary" to="/register"
          >{{ t('auth.register') }}</v-btn
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
