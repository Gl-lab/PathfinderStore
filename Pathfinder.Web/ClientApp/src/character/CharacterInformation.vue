<template>
  <v-card v-if="character" max-width="600">
    <template slot="default">
      <v-btn icon @click="loadCharacter">
        <v-icon>mdi-cached</v-icon>
      </v-btn>
    </template>
    <v-card-title>{{ character.name }}</v-card-title>
    <v-list-item two-line v-if="character.race">
      <v-list-item-content>
        <v-list-item-title>
          {{ character.race.name }}
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
    <v-card class="mx-auto" max-width="400">
      <v-card-text>
        <v-text-field
            class="mr-4"
            v-model="balance"
            label="Медные монеты"
            min="0"
            step="1"
            type="number"
        ></v-text-field>
      </v-card-text>
      <v-card-actions>
        <v-btn text @click="topUpBalance" color="deep-purple accent-1">
          Пополнить кошелек
        </v-btn>
      </v-card-actions>
      <v-card-title class="text-h5">
        {{ errorText }}
      </v-card-title>
    </v-card>

    <characteristics-form
        v-if="character.characteristics"
        :model="character.characteristics"
        class="pt-5 px-15"
    ></characteristics-form>
    <v-data-table
        dense
        :headers="getHeaders"
        :items="getWeapons"
        item-key="id"
        class="elevation-1"
    ></v-data-table>
  </v-card>
</template>

<script>
import {createNamespacedHelpers} from "vuex";
import CharacteristicsForm from "@/character/CharacteristicsForm.vue";

const {mapActions, mapGetters} = createNamespacedHelpers("character");
export default {
  components: {
    CharacteristicsForm
  },
  data() {
    return {
      balance: 0,
      errorText: "",
      snackbar: {
        isActive: false,
        text: null,
        timeout: 2000
      },
      items: []
    };
  },
  computed: {
    ...mapGetters(["character"]),
    getHeaders: function () {
      return [
        {
          text: "Наименование",
          align: "start",
          sortable: false,
          value: "name"
        },
        {text: "Размер", value: "size"},
        {text: "Урон", value: "damage"},
        {text: "Категория", value: "category"},
        {text: "Тип", value: "type"}
      ];
    },
    getWeapons: function () {
      if (this.items == null) return null;
      const itemTable = [];
      this.items.forEach(value =>
          itemTable.push({
            id: value.item.id,
            name: value.item.product.name,
            category: value.item.product.category.name,
            damage: value.damage.count + "D" + value.damage.d,
            size: value.size,
            type: value.weaponType.name
          })
      );
      return itemTable;
    }
  },
  methods: {
    ...mapActions(["loadCharacter"]),
    topUpBalance: function () {
      if (this.balance > 0) {
        let resultCode = null;
        this.axios
            .put("/api/Character/IncreaseBalance", {value: this.balance})
            .catch(error => (resultCode = error.response))
            .finally(() => {
              if (resultCode != null && resultCode.status !== 200) {
                this.errorText = resultCode.data;
              } else {
                this.balance = 0;
                this.loadCharacter();
              }
            });
      }
    },
    loadWeapons: function () {
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
    this.loadCharacter();
    this.loadWeapons();
  }
};
</script>

<style></style>
