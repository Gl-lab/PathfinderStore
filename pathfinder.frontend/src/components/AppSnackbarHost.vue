<script setup lang="ts">
import { computed } from 'vue'
import { useSnackbar } from '@/composables/useSnackbar'

const snackbar = useSnackbar()
const current = computed(() => snackbar.state.queue[0] ?? null)
const visible = computed({
  get: () => current.value !== null,
  set: (value: boolean) => {
    if (!value) snackbar.dismiss()
  },
})
</script>

<template>
  <v-snackbar v-model="visible" :color="current?.kind" location="bottom end">
    {{ current?.text }}
  </v-snackbar>
</template>
