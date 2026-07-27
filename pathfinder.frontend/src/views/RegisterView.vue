<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { register } from '@/features/auth/api'
import { hasRequiredValue, passwordsMatch } from '@/features/auth/validation'

const router = useRouter()
const { t } = useI18n()
const form = ref({
  userName: '',
  email: '',
  name: '',
  surname: '',
  password: '',
  passwordRepeat: '',
})
const errorMessages = ref<string[]>([])
const isSubmitting = ref(false)
const authForm = ref<{ validate: () => Promise<{ valid: boolean }> } | null>(null)
const requiredRule = (value: unknown): boolean | string =>
  hasRequiredValue(value) || t('auth.required')
const passwordsMatchRule = (value: string): boolean | string =>
  passwordsMatch(form.value.password, value) || t('errors.passwordsMismatch')

async function submit(): Promise<void> {
  errorMessages.value = []
  const validation = await authForm.value?.validate()
  if (!validation?.valid) {
    return
  }
  isSubmitting.value = true
  try {
    await register({
      userName: form.value.userName.trim(),
      email: form.value.email.trim(),
      name: form.value.name.trim(),
      surname: form.value.surname.trim(),
      password: form.value.password,
    })
    await router.replace('/login')
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <main class="auth-page">
    <v-card class="auth-card" elevation="0"
      ><v-card-item
        ><p class="eyebrow">{{ t('auth.ledger') }}</p>
        <v-card-title>{{ t('auth.createAccount') }}</v-card-title
        ><v-card-subtitle>{{ t('auth.registrationPrompt') }}</v-card-subtitle></v-card-item
      ><v-card-text
        ><v-alert
          v-for="message in errorMessages"
          :key="message"
          type="error"
          variant="tonal"
          class="mb-3"
          >{{ message }}</v-alert
        ><v-form ref="authForm" validate-on="blur lazy" @submit.prevent="submit"
          ><v-text-field
            v-model="form.userName"
            :label="t('common.requiredField', { field: t('auth.userName') })"
            autocomplete="username"
            :rules="[requiredRule]"
            required
          /><v-text-field
            v-model="form.email"
            :label="t('common.requiredField', { field: t('auth.email') })"
            type="email"
            autocomplete="email"
            :rules="[requiredRule]"
            required
          /><v-row
            ><v-col
              ><v-text-field
                v-model="form.name"
                :label="t('common.optionalField', { field: t('auth.firstName') })" /></v-col
            ><v-col
              ><v-text-field
                v-model="form.surname"
                :label="t('common.optionalField', { field: t('auth.surname') })" /></v-col></v-row
          ><v-text-field
            v-model="form.password"
            :label="t('common.requiredField', { field: t('auth.password') })"
            type="password"
            autocomplete="new-password"
            :rules="[requiredRule]"
            required
          /><v-text-field
            v-model="form.passwordRepeat"
            :label="t('common.requiredField', { field: t('auth.passwordRepeat') })"
            type="password"
            autocomplete="new-password"
            :rules="[requiredRule, passwordsMatchRule]"
            required
          /><v-btn type="submit" color="primary" block size="large" :loading="isSubmitting"
            >{{ t('auth.register') }}</v-btn
          ></v-form
        ></v-card-text
      ><v-card-actions class="px-4 pb-5"
        >{{ t('auth.hasAccount') }}
        <v-btn variant="text" color="primary" to="/login">{{ t('app.auth.signIn') }}</v-btn></v-card-actions
      ></v-card
    >
  </main>
</template>

<style scoped>
.auth-page {
  display: grid;
  min-height: calc(100vh - 160px);
  place-items: center;
}
.auth-card {
  width: min(100%, 580px);
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
