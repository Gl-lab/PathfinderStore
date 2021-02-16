<template>
  <div>
    <v-slider
      v-model="characteristics.strength.value"
      :label="Сила"
      :thumb-color="ex3.color"
      thumb-label="always"
      :max=30
      :min=0
    ></v-slider>
    <v-slider
      v-model="characteristics.dexterity.value"
      :label="Ловкость"
      :thumb-color="ex3.color"
      thumb-label="always"
      :max=30
      :min=0
    ></v-slider>
    <v-slider
      v-model="characteristics.constitution.value"
      :label="Выносливость"
      :thumb-color="ex3.color"
      thumb-label="always"
      :max=30
      :min=0
    ></v-slider>
    <v-slider
      v-model="characteristics.intelligence.value"
      :label="Интелект"
      :thumb-color="ex3.color"
      thumb-label="always"
      :max=30
      :min=0
    ></v-slider>
    <v-slider
      v-model="characteristics.wisdom.value"
      :label="Мудрость"
      :thumb-color="ex3.color"
      thumb-label="always"
      :max=30
      :min=0
    ></v-slider>
    <v-slider
      v-model="characteristics.charisma.value"
      :label="Харизма"
      :thumb-color="ex3.color"
      thumb-label="always"
      :max=30
      :min=0
    ></v-slider>
  </div>
</template>

<script>
import Characteristics from "@/models/Characteristics.js";
export default {
  data() {
    return {
      characteristics: new Characteristics()
    };
  }
};
</script>

<style></style>
