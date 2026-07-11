<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<template>
  <v-card class="not-found" elevation="0">
    <v-card-item>
      <v-card-title>{{ t('notFound.title') }}</v-card-title>
      <v-card-subtitle>{{ t('notFound.subtitle') }}</v-card-subtitle>
    </v-card-item>
    <v-card-actions>
      <v-btn to="/" color="primary" variant="flat">{{ t('app.navigation.characters') }}</v-btn>
    </v-card-actions>
  </v-card>
</template>

<style scoped>
.not-found {
  max-width: 560px;
}
</style>
