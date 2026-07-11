<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { register } from '@/features/auth/api'

const router = useRouter()
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

async function submit(): Promise<void> {
  errorMessages.value = []
  if (form.value.password !== form.value.passwordRepeat) {
    errorMessages.value = ['Пароли не совпадают.']
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
        ><p class="eyebrow">Character Ledger</p>
        <v-card-title>Создайте аккаунт</v-card-title
        ><v-card-subtitle>Он понадобится для сохранения персонажей.</v-card-subtitle></v-card-item
      ><v-card-text
        ><v-alert
          v-for="message in errorMessages"
          :key="message"
          type="error"
          variant="tonal"
          class="mb-3"
          >{{ message }}</v-alert
        ><v-form @submit.prevent="submit"
          ><v-text-field v-model="form.userName" label="Никнейм" required /><v-text-field
            v-model="form.email"
            label="Электронная почта"
            type="email"
            required
          /><v-row
            ><v-col><v-text-field v-model="form.name" label="Имя" /></v-col
            ><v-col><v-text-field v-model="form.surname" label="Фамилия" /></v-col></v-row
          ><v-text-field
            v-model="form.password"
            label="Пароль"
            type="password"
            required
          /><v-text-field
            v-model="form.passwordRepeat"
            label="Повтор пароля"
            type="password"
            required
          /><v-btn type="submit" color="primary" block size="large" :loading="isSubmitting"
            >Зарегистрироваться</v-btn
          ></v-form
        ></v-card-text
      ><v-card-actions class="px-4 pb-5"
        >Уже есть аккаунт?
        <v-btn variant="text" color="primary" to="/login">Войти</v-btn></v-card-actions
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
