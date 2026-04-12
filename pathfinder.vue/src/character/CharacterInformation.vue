<template>
  <v-card v-if="character" max-width="600">
    <template slot="default">
      <v-btn icon @click="loadCurrentCharacter">
        <v-icon>mdi-cached</v-icon>
      </v-btn>
    </template>
    <v-card-title>{{ character.name }}</v-card-title>
    <v-list-item two-line>
      <v-list-item-content>
        <v-list-item-title>
          {{ ancestryName }}
        </v-list-item-title>
        <v-list-item-subtitle>Родословная</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>
    <v-card-text>
      <v-row align="center">
        <v-col>
          <v-icon large color="amber darken-2">
            mdi-gold
          </v-icon>
          {{ character.wallet.gold }}
        </v-col>
        <v-col>
          <v-icon large color="blue-grey lighten-4">
            mdi-gold
          </v-icon>
          {{ character.wallet.silver }}
        </v-col>
        <v-col>
          <v-icon large color="orange accent-4">
            mdi-gold
          </v-icon>
          {{ character.wallet.copper }}
        </v-col>
      </v-row>
    </v-card-text>

    <characteristics-form
      v-if="character.characteristics"
      :model="character.characteristics"
      class="pt-5 px-15"
    ></characteristics-form>
  </v-card>
  <h1 v-else>
    {{ emptyStateText }} (
    <v-btn icon :to="{ name: 'Characters' }">
      <v-icon>mdi-account-group</v-icon>
    </v-btn>
    ) чтобы перейти к списку персонажей.
  </h1>
</template>

<script>
import { createNamespacedHelpers } from "vuex";
import CharacteristicsForm from "@/character/CharacteristicsForm.vue";

const { mapActions, mapGetters } = createNamespacedHelpers("character");

const ancestryMap = {
  0: "Не выбрана",
  1: "Gnome",
  2: "Goblin",
  3: "Dwarf",
  4: "Halfling",
  5: "Human",
  6: "Elf"
};

export default {
  components: {
    CharacteristicsForm
  },
  data() {
    return {
      loadError: false
    };
  },
  computed: {
    ...mapGetters(["character"]),
    emptyStateText: function() {
      if (this.loadError) {
        return "Персонаж не найден или недоступен";
      }

      return "Выберите персонажа из списка";
    },
    ancestryName: function() {
      if (!this.character) {
        return "";
      }

      return (
        ancestryMap[this.character.ancestryType] || this.character.ancestryType
      );
    }
  },
  methods: {
    ...mapActions(["loadCharacter"]),
    loadCurrentCharacter: function() {
      const characterId = parseInt(this.$route.params.id, 10);

      this.loadError = false;

      if (Number.isNaN(characterId)) {
        this.loadError = true;
        return;
      }

      this.loadCharacter(characterId).catch(() => {
        this.loadError = true;
      });
    }
  },
  mounted() {
    this.loadCurrentCharacter();
  },
  watch: {
    "$route.params.id": function() {
      this.loadCurrentCharacter();
    }
  }
};
</script>

<style></style>
