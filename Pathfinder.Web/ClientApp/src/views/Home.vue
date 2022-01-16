<template>
  <div>
    <div v-if="isAuthorized">
      <characters-list/>
    </div>
    <h1 v-else>Авторизуйтесь</h1>
  </div>
</template>
<script>
import {mapGetters} from "vuex";
import CharactersList from "@/character/CharactersList";

export default {
  name: "Characters",
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
