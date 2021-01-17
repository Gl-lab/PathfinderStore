<template>
  <div>
    <v-combobox
      v-model="select"
      label="Категория"
      :items="categories"
      item-text="name"
      @input="respond"
    ></v-combobox>
  </div>
</template>

<script>
export default {
  name: "CategoryComboBox",
  props: {
    model: {}
  },
  data() {
    return {
      categories: [{ id: 0, name: "No data.", description: "" }],
      select: null
    };
  },
  mounted: function() {
    try {
      this.loadData();
    } catch {
      this.categories = [];
    }
  },
  methods: {
    loadData() {
      this.axios.get("api/Categories").then(response => {
        this.categories = response.data;
      });
    },
    respond() {
      this.$emit("response", this.select);
    }
  }
};
</script>
