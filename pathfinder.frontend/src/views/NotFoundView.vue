<template>
  <v-card class="not-found" elevation="0">
    <v-card-item>
      <v-card-title>Эта страница не найдена</v-card-title>
      <v-card-subtitle>Вернитесь к списку персонажей.</v-card-subtitle>
    </v-card-item>
    <v-card-actions>
      <v-btn to="/" color="primary" variant="flat">К персонажам</v-btn>
    </v-card-actions>
  </v-card>
</template>

<style scoped>
.not-found {
  max-width: 560px;
}
</style>
