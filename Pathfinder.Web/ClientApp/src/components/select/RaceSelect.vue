<template>
  <div>
    <v-select
      v-model="select"
      label="Раса"
      :items="list"
      item-text="name"
      :hint="hintText"
      return-object
      @change="response"
    ></v-select>
  </div>
</template>

<script>
export default {
  data() {
    return {
      list: [{ id: 0, name: "No data." }],
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
      this.axios.get("/api/Character/Races").then(response => {
        this.list = response.data;
      });
    },
    response() {
      this.$emit("response", this.select);
    }
  },
  computed: {
    hintText: function() {
      return this.select ? `Размер ${this.select.size.name}` : "";
    }
  }
};
</script>
