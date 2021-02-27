<template>
  <div>
    <v-row class="pt-5">
      <v-col xs="3" sm="4">Сила</v-col>
      <v-col>
        <v-slider
          v-model="characteristics.strength.value"
          thumb-color="red darken-2"
          thumb-label="always"
          :max="30"
          :min="0"
          @input="update"
        ></v-slider>
      </v-col>
    </v-row>
    <v-row>
      <v-col xs="3" sm="4">Ловкость</v-col>
      <v-col>
        <v-slider
          v-model="characteristics.dexterity.value"
          thumb-color="green darken-2"
          thumb-label="always"
          :max="30"
          :min="0"
          @input="update"
        ></v-slider>
      </v-col>
    </v-row>

    <v-row>
      <v-col xs="3" sm="4">Выносливость</v-col>
      <v-col>
        <v-slider
          v-model="characteristics.constitution.value"
          thumb-label="always"
          thumb-color="pink darken-2"
          :max="30"
          :min="0"
          @input="update"
        ></v-slider>
      </v-col>
    </v-row>
    <v-row>
      <v-col xs="3" sm="4">Интелект</v-col>
      <v-col>
        <v-slider
          v-model="characteristics.intelligence.value"
          thumb-label="always"
          thumb-color="blue darken-2"
          :max="30"
          :min="0"
          @input="update"
        ></v-slider>
      </v-col>
    </v-row>
    <v-row>
      <v-col xs="3" sm="4">Мудрость</v-col>
      <v-col>
        <v-slider
          v-model="characteristics.wisdom.value"
          thumb-label="always"
          thumb-color="purple darken-2"
          :max="30"
          :min="0"
          @input="update"
        ></v-slider>
      </v-col>
    </v-row>
    <v-row>
      <v-col xs="3" sm="4">Харизма</v-col>
      <v-col>
        <v-slider
          v-model="characteristics.charisma.value"
          thumb-label="always"
          thumb-color="yellow darken-2"
          :max="30"
          :min="0"
          @input="update"
        ></v-slider>
      </v-col>
    </v-row>
  </div>
</template>

<script>
import Characteristics from "@/models/Characteristics.js";
export default {
  props: {
    model: {
      type: Object,
      default: () => null
    }
  },
  data() {
    return {
      characteristics: new Characteristics()
    };
  },
  mounted: function() {
    if (this.model) {
      this.characteristics = this.model;
    }
  },
  methods: {
    update: function() {
      this.$emit("update", this.characteristics);
    }
  }
};
</script>

<style></style>
