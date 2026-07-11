<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { getApiErrorMessages } from '@/api/errors'
import {
  ancestryNames,
  deleteCharacter,
  getCharacter,
  type Character,
} from '@/features/characters/api'

const route = useRoute()
const router = useRouter()
const character = ref<Character | null>(null)
const errors = ref<string[]>([])
const isLoading = ref(true)
const isDeleting = ref(false)
const confirmDelete = ref(false)
const abilityLabels = {
  strength: 'Сила',
  dexterity: 'Ловкость',
  constitution: 'Выносливость',
  intelligence: 'Интеллект',
  wisdom: 'Мудрость',
  charisma: 'Харизма',
} as const
async function load(): Promise<void> {
  isLoading.value = true
  errors.value = []
  try {
    character.value = await getCharacter(Number(route.params.id))
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isLoading.value = false
  }
}
async function remove(): Promise<void> {
  if (!character.value) return
  isDeleting.value = true
  try {
    await deleteCharacter(character.value.id)
    await router.replace('/')
  } catch (error) {
    errors.value = getApiErrorMessages(error)
  } finally {
    isDeleting.value = false
    confirmDelete.value = false
  }
}
onMounted(load)
</script>

<template>
  <section class="details">
    <v-btn prepend-icon="mdi-arrow-left" variant="text" to="/">К персонажам</v-btn
    ><v-progress-linear v-if="isLoading" color="accent" indeterminate rounded /><v-alert
      v-for="error in errors"
      :key="error"
      type="error"
      variant="tonal"
      >{{ error }}</v-alert
    ><template v-if="character"
      ><header>
        <div>
          <p class="eyebrow">{{ ancestryNames[character.ancestryType] ?? 'Персонаж' }}</p>
          <h1>{{ character.name }}</h1>
          <p v-if="character.concept">{{ character.concept }}</p>
        </div>
        <v-btn
          color="error"
          variant="tonal"
          prepend-icon="mdi-delete-outline"
          @click="confirmDelete = true"
          >Удалить</v-btn
        >
      </header>
      <div class="stats">
        <v-card elevation="0"
          ><v-card-item title="Детали" /><v-card-text
            ><p v-if="character.age">Возраст: {{ character.age }}</p>
            <p v-else>Возраст не указан</p></v-card-text
          ></v-card
        ><v-card elevation="0"
          ><v-card-item title="Характеристики" /><v-card-text class="abilities"
            ><div v-for="(label, key) in abilityLabels" :key="key">
              <span>{{ label }}</span
              ><strong
                >{{ character.characteristics[key].value }}
                <small
                  >{{ character.characteristics[key].modifier >= 0 ? '+' : ''
                  }}{{ character.characteristics[key].modifier }}</small
                ></strong
              >
            </div></v-card-text
          ></v-card
        >
      </div></template
    ><v-dialog v-model="confirmDelete" max-width="440"
      ><v-card
        ><v-card-title>Удалить персонажа?</v-card-title
        ><v-card-text>Это действие нельзя отменить.</v-card-text
        ><v-card-actions
          ><v-spacer /><v-btn variant="text" @click="confirmDelete = false">Отмена</v-btn
          ><v-btn color="error" :loading="isDeleting" @click="remove"
            >Удалить</v-btn
          ></v-card-actions
        ></v-card
      ></v-dialog
    >
  </section>
</template>

<style scoped>
.details {
  display: grid;
  gap: 24px;
}
header {
  display: flex;
  justify-content: space-between;
  gap: 24px;
  align-items: start;
}
.eyebrow {
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
}
.stats {
  display: grid;
  grid-template-columns: minmax(0, 0.75fr) minmax(0, 1.25fr);
  gap: 16px;
}
.stats .v-card {
  border: 1px solid rgb(var(--v-theme-surface-variant));
}
.abilities {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 14px;
}
.abilities div {
  display: flex;
  justify-content: space-between;
  border-bottom: 1px solid rgb(var(--v-theme-surface-variant));
  padding-bottom: 8px;
}
small {
  color: rgb(var(--v-theme-secondary));
}
@media (max-width: 600px) {
  header {
    flex-direction: column;
  }
  .stats {
    grid-template-columns: 1fr;
  }
}
</style>
