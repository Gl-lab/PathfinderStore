<template>
  <div class="home">
    <h1>Все хорошо! Вы на главной</h1>
    <div v-if="isAuthorized">
      <characters-list />
    </div>
  </div>
</template>
<script>
import { mapGetters } from "vuex";
import CharactersList from "@/character/CharactersList";
export default {
  name: "Home",
  data() {
    return {
      dialog: false
    };
  },
  components: {
    CharactersList
  },
  computed: {
    ...mapGetters({
      isAuthorized: "auth/isAuthorized"
    })
  }
};
</script>
