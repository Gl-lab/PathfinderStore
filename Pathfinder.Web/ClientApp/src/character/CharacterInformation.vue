<template>
  <v-card v-if="currentCharacter">
    <v-card-title>{{ currentCharacter.name }}</v-card-title>
    <v-list-item two-line v-if="currentCharacter.race">
      <v-list-item-content>
        <v-list-item-title>
          {{ currentCharacter.race.name }}
        </v-list-item-title>
        <v-list-item-subtitle>Раса</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-card-text>
      <v-row align="center">
        <v-col>
          <v-icon large color="amber darken-2">
            mdi-gold
          </v-icon>
          {{ gold }}
        </v-col>
        <v-col>
          <v-icon large color="blue-grey lighten-4">
            mdi-gold
          </v-icon>
          {{ silver }}
        </v-col>
        <v-col>
          <v-icon large color="orange accent-4">
            mdi-gold
          </v-icon>
          {{ copper }}
        </v-col>
      </v-row>
    </v-card-text>
    <characteristics-form
      v-if="currentCharacter.characteristics"
      :model="currentCharacter.characteristics"
      class="pt-5 px-15"
    ></characteristics-form>
  </v-card>
</template>

<script>
import { createNamespacedHelpers } from "vuex";
import CharacteristicsForm from "@/character/CharacteristicsForm.vue";
const { mapGetters } = createNamespacedHelpers("auth");
export default {
  components: {
    CharacteristicsForm
  },
  computed: {
    ...mapGetters(["currentCharacter"]),
    gold: function() {
      return Math.floor(this.currentCharacter.balance / 100);
    },
    silver: function() {
      return Math.floor((this.currentCharacter.balance % 100) / 10);
    },
    copper: function() {
      return Math.floor(this.currentCharacter.balance % 10);
    }
  }
};
</script>

<style></style>
