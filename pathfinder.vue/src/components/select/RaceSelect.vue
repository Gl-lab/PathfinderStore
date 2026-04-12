<template>
  <div>
    <v-select
      v-model="select"
      label="Родословная"
      :items="list"
      item-text="displayName"
      :hint="hintText"
      return-object
      @change="response"
    ></v-select>
  </div>
</template>

<script>
export default {
  name: "RaceSelect",
  data() {
    return {
      list: [],
      select: null
    };
  },
  mounted: function() {
    try {
      this.loadData();
    } catch {
      this.list = [];
    }
  },
  methods: {
    loadData() {
      this.axios.get("/api/ancestries").then(response => {
        this.list = Array.isArray(response.data)
          ? response.data.map(ancestry => this.mapAncestry(ancestry))
          : [];
      });
    },
    mapAncestry(ancestry) {
      return {
        ...ancestry,
        displayName: ancestry.name || this.getAncestryName(ancestry.type)
      };
    },
    getAncestryName(type) {
      const ancestryMap = {
        1: "Gnome",
        2: "Goblin",
        3: "Dwarf",
        4: "Halfling",
        5: "Human",
        6: "Elf"
      };

      return ancestryMap[type] || "Unknown ancestry";
    },
    response() {
      this.$emit("response", this.select);
    }
  },
  computed: {
    hintText: function() {
      if (!this.select) {
        return "";
      }

      return `Размер ${this.select.size}, HP ${this.select.baseHitPoints}, скорость ${this.select.baseSpeed}`;
    }
  }
};
</script>
