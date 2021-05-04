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
    {{ items }}
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
  data() {
    return {
      snackbar: {
        isActive: false,
        text: null,
        timeout: 2000
      },
      items: []
    };
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
  },
  methods: {
    loadWeapons: function() {
      try {
        this.axios
          .get("/api/Character/items/Weapons")
          .then(response => {
            this.items = response.data;
          })
          .catch(error => {
            console.log(error.message);
          });
      } catch {
        console.log(1);
      }
    }
  },
  mounted() {
    this.loadWeapons();
  }
};
</script>

<style></style>
