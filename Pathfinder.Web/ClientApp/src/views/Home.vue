<template>
  <div class="home">
    <h1>Все хорошо! Вы на главной</h1>
    <div v-if="isAuthorized">
      <characters-list/>
      <v-dialog
        v-model="dialog"
        width="500"
      >
      <template v-slot:activator="{ on, attrs }">
        <v-btn
          class="mx-2"
          fab
          dark
          color="indigo"
          v-bind="attrs"
          v-on="on"
        >
          <v-icon dark>
            mdi-plus
          </v-icon>
        </v-btn>
      </template>
      <CharacterCreateForm @complite="dialog=false"/>
    </v-dialog>
    </div>
  </div>
</template>
<script>
import CharacterCreateForm from '@/character/CharacterCreateForm';
import { mapGetters } from 'vuex';
import CharactersList from "@/components/CharactersList";
export default {
  name: "Home",
  data() {
    return {
      dialog: false
    }
  },
  components: {
    CharactersList,
    CharacterCreateForm
  },
  computed: {
    ...mapGetters({
      isAuthorized: 'auth/isAuthorized'
    })
  }
};
</script>
