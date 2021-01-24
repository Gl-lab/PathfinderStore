<template>
  <div class="home">
    <h1>Все хорошо! Вы на главной</h1>
    <div v-if="isAuthorized">
      <characters-list/>
    </div>
  </div>
</template>
s
<script>
import { mapGetters } from 'vuex'
import CharactersList from "@/components/CharactersList";
export default {
  name: "Home",
  components: {
    CharactersList
  },
  computed: {
    ...mapGetters({
      isAuthorized: 'auth/isAuthorized'
    })
  }
};
</script>
