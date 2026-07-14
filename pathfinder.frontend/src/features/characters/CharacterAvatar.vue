<script setup lang="ts">
import { applyUnknownAvatarFallback, resolveAvatarPath } from '@/features/characters/avatar'

defineProps<{
  path: string | null | undefined
  alt: string
  size: number | string
}>()
</script>

<template>
  <v-avatar :size="size" class="character-avatar">
    <img
      class="character-avatar__image"
      :src="resolveAvatarPath(path)"
      :alt="alt"
      @error="applyUnknownAvatarFallback"
    />
  </v-avatar>
</template>

<style scoped>
.character-avatar {
  background: #17191d;
  flex: 0 0 auto;
}
.character-avatar__image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
}
</style>
