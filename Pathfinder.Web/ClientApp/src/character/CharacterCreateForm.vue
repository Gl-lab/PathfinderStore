<template>
  <v-card class="pa-5">
    <v-form v-model="valid" ref="form">
      <race-select @response="selectRace"></race-select>
      <v-text-field
        v-model="newCharacter.name"
        :rules="nameRules"
        label="Имя"
        required
      ></v-text-field>
      <characteristics-form @update="setCharacteristics"></characteristics-form>
      <v-btn
        :disabled="!valid"
        color="success"
        class="mr-4"
        @click="createCharacter"
      >
        Создать
      </v-btn>
      <v-btn color="error" class="mr-4" @click="reset">
        Сброс
      </v-btn>
    </v-form>
  </v-card>
</template>
<script>
import RaceSelect from "@/components/select/RaceSelect.vue";
import CharacteristicsForm from "@/character/CharacteristicsForm.vue";

export default {
  components: {
    RaceSelect,
    CharacteristicsForm
  },
  name: "CharacterCreateForm",
  data() {
    return {
      valid: true,
      newCharacter: {
        name: null,
        balance: 0,
        raceId: null,
        characteristics: null
      },
      resultCode: 0,
      nameRules: [
        v => !!v || "Name is required",
        v => (v && v.length <= 10) || "Name must be less than 10 characters"
      ],
      errorText: ""
    };
  },
  methods: {
    reset: function() {
      this.$refs.form.reset();
    },
    createCharacter: function() {
      try {
        this.axios
          .post("/api/Character", this.newCharacter)
          .then(response => {
            this.resultCode = response.status;
            this.$emit("complite");
          })
          .then(() => {
            if (this.resultCode !== 200) {
              this.errorText = "Неудачно";
            }
          });
      } catch {
        this.errorText = "Неудачно";
      }
    },
    selectRace: function(race) {
      this.newCharacter.raceId = race.id;
    },
    setCharacteristics: function(characteristics) {
      this.newCharacter.characteristics = characteristics;
    }
  }
};
</script>
