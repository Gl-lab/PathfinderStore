<template>
  <div>
    <v-select
      v-model="select"
      label="Расса"
      :items="list"
      item-text="name"
      :hint="`Размер ${select.size.name}`"
      @input="response"
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
  }
};
</script>
