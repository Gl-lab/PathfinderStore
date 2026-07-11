<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import { ancestryNames, getCharacters, type Character } from '@/features/characters/api'

const router = useRouter()
const characters = ref<Character[]>([])
const errorMessages = ref<string[]>([])
const isLoading = ref(true)

async function loadCharacters(): Promise<void> {
  isLoading.value = true
  errorMessages.value = []
  try {
    characters.value = await getCharacters()
  } catch (error) {
    errorMessages.value = getApiErrorMessages(error)
  } finally {
    isLoading.value = false
  }
}

onMounted(loadCharacters)
</script>

<template>
  <section class="dashboard">
    <header class="dashboard__header">
      <div>
        <p class="eyebrow">Ваше приключение</p>
        <h1>Мои персонажи</h1>
        <p class="dashboard__lead">
          Соберите историю героя, сохраните его выборы и возвращайтесь к игре без лишних таблиц.
        </p>
      </div>
      <v-btn color="accent" size="large" prepend-icon="mdi-account-plus" to="/characters/create"
        >Создать персонажа</v-btn
      >
    </header>
    <v-progress-linear v-if="isLoading" color="accent" indeterminate rounded />
    <v-alert v-for="message in errorMessages" :key="message" type="error" variant="tonal"
      >{{ message
      }}<template #append
        ><v-btn variant="text" @click="loadCharacters">Повторить</v-btn></template
      ></v-alert
    >
    <v-card
      v-if="!isLoading && !errorMessages.length && !characters.length"
      class="empty-state"
      elevation="0"
      to="/characters/create"
      ><v-card-item prepend-icon="mdi-account-plus-outline"
        ><v-card-title>Первый герой ждёт вас</v-card-title
        ><v-card-subtitle
          >Создайте персонажа, чтобы начать вести его историю.</v-card-subtitle
        ></v-card-item
      ></v-card
    >
    <div v-if="!isLoading" class="character-grid">
      <v-card
        v-for="character in characters"
        :key="character.id"
        class="character-card"
        elevation="0"
        @click="router.push({ name: 'character-details', params: { id: character.id } })"
        ><v-card-item
          ><template #prepend
            ><v-avatar color="primary" size="48"
              ><span class="character-card__initials">{{
                character.name.slice(0, 1).toUpperCase()
              }}</span></v-avatar
            ></template
          ><v-card-title>{{ character.name }}</v-card-title
          ><v-card-subtitle>{{
            ancestryNames[character.ancestryType] ?? 'Родословная'
          }}</v-card-subtitle></v-card-item
        ><v-card-text>{{ character.concept || 'История ещё не написана.' }}</v-card-text
        ><v-card-actions
          ><v-btn variant="text" color="primary">Открыть</v-btn></v-card-actions
        ></v-card
      >
    </div>
  </section>
</template>

<style scoped>
.dashboard {
  display: grid;
  gap: 24px;
}
.dashboard__header {
  display: flex;
  align-items: end;
  justify-content: space-between;
  gap: 24px;
}
.eyebrow {
  margin: 0 0 8px;
  color: rgb(var(--v-theme-secondary));
  font-size: 0.875rem;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}
h1 {
  margin: 0;
  color: rgb(var(--v-theme-primary));
  font-family: Georgia, 'Times New Roman', serif;
  font-size: clamp(2rem, 5vw, 3rem);
  line-height: 1.05;
}
.dashboard__lead {
  max-width: 620px;
  margin: 12px 0 0;
  color: #52606d;
  font-size: 1.0625rem;
}
.character-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 16px;
}
.character-card,
.empty-state {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}
.character-card {
  cursor: pointer;
}
.character-card__initials {
  color: white;
  font-size: 1.25rem;
  font-weight: 700;
}
@media (max-width: 600px) {
  .dashboard__header {
    align-items: stretch;
    flex-direction: column;
  }
}
</style>
