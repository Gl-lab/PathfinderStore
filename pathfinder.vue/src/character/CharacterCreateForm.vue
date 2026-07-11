<template>
  <v-card class="pa-5">
    <v-form v-model="valid" ref="form">
      <race-select
        :key="`race-${formResetKey}`"
        @response="selectRace"
      ></race-select>
      <v-text-field
        v-model="newCharacter.name"
        :rules="nameRules"
        label="Имя"
        required
      ></v-text-field>
      <v-textarea
        v-model="newCharacter.concept"
        :counter="1000"
        :rules="conceptRules"
        label="Краткая концепция"
        hint="Необязательно: кем является ваш персонаж?"
        persistent-hint
      ></v-textarea>
      <v-text-field
        v-model.number="newCharacter.age"
        :rules="ageRules"
        label="Возраст"
        type="number"
        min="1"
      ></v-text-field>
      <v-alert v-if="selectedAncestry" type="info" outlined dense class="mt-4">
        {{ ancestrySummary }}
      </v-alert>
      <free-boosts-select
        :key="`boosts-${formResetKey}-${newCharacter.ancestryType}`"
        :ancestry="selectedAncestry"
        @update="setFreeBoosts"
      ></free-boosts-select>
      <v-alert v-if="errorText" type="error" outlined dense class="mt-4">
        {{ errorText }}
      </v-alert>
      <v-btn
        :disabled="!canSubmit"
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
import FreeBoostsSelect from "@/character/FreeBoostsSelect.vue";

const ancestryLabels = {
  1: "Gnome",
  2: "Goblin",
  3: "Dwarf",
  4: "Halfling",
  5: "Human",
  6: "Elf"
};

const abilityLabels = {
  1: "Сила",
  2: "Ловкость",
  3: "Выносливость",
  4: "Интеллект",
  5: "Мудрость",
  6: "Харизма"
};

export default {
  components: {
    RaceSelect,
    FreeBoostsSelect
  },
  name: "CharacterCreateForm",
  data() {
    return {
      valid: true,
      newCharacter: {
        name: null,
        concept: null,
        age: null,
        ancestryType: 0,
        freeBoosts: []
      },
      selectedAncestry: null,
      resultCode: 0,
      loading: false,
      formResetKey: 0,
      nameRules: [
        v => !!v || "Name is required",
        v => (v && v.length <= 10) || "Name must be less than 10 characters"
      ],
      conceptRules: [
        v =>
          !v ||
          v.trim().length <= 1000 ||
          "Концепция не может быть длиннее 1000 символов"
      ],
      ageRules: [
        v =>
          !v ||
          (Number.isInteger(v) && v > 0) ||
          "Возраст должен быть положительным целым числом"
      ],
      errorText: ""
    };
  },
  computed: {
    ancestrySummary: function() {
      if (!this.selectedAncestry) {
        return "";
      }

      const fixedBoosts = Array.isArray(this.selectedAncestry.abilityBoosts)
        ? this.selectedAncestry.abilityBoosts
            .filter(boost => !boost.isFree && boost.abilityType)
            .map(boost => this.getAbilityName(boost.abilityType))
        : [];
      const flaws = Array.isArray(this.selectedAncestry.abilityFlaws)
        ? this.selectedAncestry.abilityFlaws.map(abilityType => {
            return this.getAbilityName(abilityType);
          })
        : [];
      const freeBoostsCount = Array.isArray(this.selectedAncestry.abilityBoosts)
        ? this.selectedAncestry.abilityBoosts.filter(boost => boost.isFree)
            .length
        : 0;

      const parts = [
        `Родословная: ${this.getAncestryName(this.selectedAncestry.type)}`
      ];

      if (fixedBoosts.length > 0) {
        parts.push(`фиксированные бусты: ${fixedBoosts.join(", ")}`);
      }

      if (freeBoostsCount > 0) {
        parts.push(`свободные бусты: ${freeBoostsCount}`);
      }

      if (flaws.length > 0) {
        parts.push(`слабости: ${flaws.join(", ")}`);
      }

      return parts.join(" | ");
    },
    requiredFreeBoostCount: function() {
      if (
        !this.selectedAncestry ||
        !Array.isArray(this.selectedAncestry.abilityBoosts)
      ) {
        return 0;
      }

      return this.selectedAncestry.abilityBoosts.filter(boost => boost.isFree)
        .length;
    },
    canSubmit: function() {
      if (!this.valid || this.loading) {
        return false;
      }

      if (!this.newCharacter.name || !this.selectedAncestry) {
        return false;
      }

      return (
        this.newCharacter.freeBoosts.length === this.requiredFreeBoostCount
      );
    }
  },
  methods: {
    reset: function() {
      this.$refs.form.reset();
      this.selectedAncestry = null;
      this.newCharacter.ancestryType = 0;
      this.newCharacter.concept = null;
      this.newCharacter.age = null;
      this.newCharacter.freeBoosts = [];
      this.errorText = "";
      this.resultCode = 0;
      this.formResetKey += 1;
    },
    createCharacter: async function() {
      this.errorText = "";
      this.loading = true;

      try {
        const response = await this.axios.post(
          "/api/character",
          this.newCharacter
        );

        this.resultCode = response.status;
        this.$emit("complite");
      } catch (error) {
        this.errorText = this.getErrorText(error);
      } finally {
        this.loading = false;
      }
    },
    selectRace: function(ancestry) {
      this.selectedAncestry = ancestry;
      this.newCharacter.ancestryType = ancestry ? ancestry.type : 0;
      this.newCharacter.freeBoosts = [];
      this.errorText = "";
    },
    setFreeBoosts: function(freeBoosts) {
      this.newCharacter.freeBoosts = freeBoosts;
    },
    getAncestryName: function(type) {
      return ancestryLabels[type] || "Unknown ancestry";
    },
    getAbilityName: function(abilityType) {
      return abilityLabels[abilityType] || abilityType;
    },
    getErrorText: function(error) {
      const responseData = error && error.response ? error.response.data : null;

      if (Array.isArray(responseData) && responseData.length > 0) {
        return responseData.join(", ");
      }

      if (responseData && typeof responseData === "string") {
        return responseData;
      }

      return "Не удалось создать персонажа.";
    }
  }
};
</script>
