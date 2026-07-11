<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/features/auth/store'

const drawer = ref(true)
const route = useRoute()
const pageTitle = computed(() => route.meta.title ?? 'Pathfinder')
const auth = useAuthStore()
</script>

<template>
  <v-app>
    <v-navigation-drawer v-model="drawer" color="primary" class="navigation" width="272">
      <div class="brand">
        <v-avatar color="accent" size="40" aria-hidden="true">
          <v-icon icon="mdi-compass-rose" />
        </v-avatar>
        <div>
          <p class="brand__title">Pathfinder</p>
          <p class="brand__subtitle">Character Ledger</p>
        </div>
      </div>

      <v-list nav density="comfortable">
        <v-list-item
          v-if="auth.isAuthenticated"
          to="/"
          prepend-icon="mdi-account-group-outline"
          title="Персонажи"
        />
        <v-list-item
          v-if="auth.isAuthenticated"
          to="/characters/create"
          prepend-icon="mdi-account-plus-outline"
          title="Создать персонажа"
        />
      </v-list>

      <template #append>
        <div class="navigation__footer">
          <v-btn
            v-if="auth.isAuthenticated"
            block
            variant="tonal"
            prepend-icon="mdi-logout"
            @click="auth.signOut()"
            >Выйти</v-btn
          >
          <v-btn v-else block variant="tonal" prepend-icon="mdi-login" to="/login">Войти</v-btn>
        </div>
      </template>
    </v-navigation-drawer>

    <v-app-bar class="app-bar" elevation="0">
      <v-app-bar-nav-icon class="d-md-none" @click="drawer = !drawer" />
      <v-app-bar-title>{{ pageTitle }}</v-app-bar-title>
    </v-app-bar>

    <v-main>
      <v-container class="content" max-width="1200">
        <router-view />
      </v-container>
    </v-main>
  </v-app>
</template>

<style scoped>
.navigation {
  color: white;
}

.brand {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 28px 24px;
}

.brand__title,
.brand__subtitle {
  margin: 0;
}

.brand__title {
  font-family: Georgia, 'Times New Roman', serif;
  font-size: 1.25rem;
  font-weight: 700;
}

.brand__subtitle {
  color: rgb(255 255 255 / 70%);
  font-size: 0.75rem;
  letter-spacing: 0.05em;
  text-transform: uppercase;
}

.navigation :deep(.v-list-item--active) {
  background: rgb(255 255 255 / 14%);
  color: white;
}

.navigation__footer {
  padding: 16px;
}

.app-bar {
  border-bottom: 1px solid rgb(var(--v-theme-surface-variant));
}

.content {
  padding: 32px 24px 56px;
}

@media (max-width: 600px) {
  .content {
    padding: 24px 16px 40px;
  }
}
</style>
