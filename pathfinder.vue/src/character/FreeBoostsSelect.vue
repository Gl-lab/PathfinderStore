<template>
  <v-card outlined class="mt-4">
    <v-card-title class="text-subtitle-1">
      Свободные бусты
    </v-card-title>
    <v-card-text>
      <div v-if="!ancestry">
        Сначала выберите родословную.
      </div>
      <div v-else>
        <div class="mb-2">Выберите {{ freeBoostSlotsText }}.</div>
        <div v-if="freeBoostSlotCount === 0" class="text--secondary">
          Для этой родословной свободные бусты не требуются.
        </div>
        <v-checkbox
          v-for="ability in abilities"
          :key="ability.value"
          v-model="selectedBoosts"
          :value="ability.value"
          :label="ability.label"
          :disabled="isAbilityDisabled(ability.value)"
          hide-details
          class="mt-0"
          @change="emitUpdate"
        ></v-checkbox>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
const abilityLabels = {
  1: "Сила",
  2: "Ловкость",
  3: "Выносливость",
  4: "Интеллект",
  5: "Мудрость",
  6: "Харизма"
};

const abilities = Object.keys(abilityLabels).map(value => {
  return {
    value: Number(value),
    label: abilityLabels[value]
  };
});

export default {
  name: "FreeBoostsSelect",
  props: {
    ancestry: {
      type: Object,
      default: () => null
    }
  },
  data() {
    return {
      abilities,
      selectedBoosts: []
    };
  },
  computed: {
    freeBoostSlotCount: function() {
      if (!this.ancestry || !Array.isArray(this.ancestry.abilityBoosts)) {
        return 0;
      }

      return this.ancestry.abilityBoosts.filter(boost => boost.isFree).length;
    },
    fixedBoostAbilityTypes: function() {
      if (!this.ancestry || !Array.isArray(this.ancestry.abilityBoosts)) {
        return [];
      }

      return this.ancestry.abilityBoosts
        .filter(boost => !boost.isFree && boost.abilityType)
        .map(boost => boost.abilityType);
    },
    freeBoostSlotsText: function() {
      if (this.freeBoostSlotCount === 1) {
        return "1 характеристику";
      }

      return `${this.freeBoostSlotCount} характеристики`;
    }
  },
  watch: {
    ancestry: {
      immediate: true,
      handler: function() {
        this.selectedBoosts = [];
        this.emitUpdate();
      }
    }
  },
  methods: {
    isAbilityDisabled: function(abilityType) {
      if (!this.ancestry) {
        return true;
      }

      if (this.fixedBoostAbilityTypes.includes(abilityType)) {
        return true;
      }

      if (this.selectedBoosts.includes(abilityType)) {
        return false;
      }

      return this.selectedBoosts.length >= this.freeBoostSlotCount;
    },
    emitUpdate: function() {
      this.$emit("update", this.selectedBoosts.slice());
    }
  }
};
</script>
