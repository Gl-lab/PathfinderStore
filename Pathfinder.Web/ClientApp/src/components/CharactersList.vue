<template>
  <v-data-table
    :headers="headers"
    :items="list"
    :items-per-page="5"
    class="elevation-1"
    :loading="loading"
  ></v-data-table>
</template>

<script>
export default {
  data() {
    return {
      loading: false,
      list: [],
      headers: [
        {
          text: "Персонаж",
          align: "start",
          sortable: false,
          value: "name"
        },
        { text: "Баланс", value: "balance" }
      ]
    };
  },
  methods: {
    loadData() {
      this.loading = true;
      this.axios
        .get("api/GameAccount", this.pageInfo)
        .then(response => {
          this.list = response.data.characters;
        })
        .catch(() => (this.list = []))
        .then(() => (this.loading = false));
    }
  },
  mounted: function() {
    this.loadData();
  }
};
</script>
