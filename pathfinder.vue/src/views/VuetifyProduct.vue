<template>
  <div>
    <v-btn color="primary" text to="/create">
      Добавить продукт
    </v-btn>
    <v-data-table
      :headers="headers"
      :items="products"
      :items-per-page="pageInfo.pageSize"
      :server-items-length="totalServerCount"
      :loading="loading"
      @update:options="changeOptions"
      locale="ru-RU"
      class="elevation-1"
    ></v-data-table>
  </div>
</template>
<script>
export default {
  name: "VuetifyProduct",
  data() {
    return {
      pageInfo: {
        pageIndex: 1,
        pageSize: 5,
        pagingsStrategy: 0,
        sortingOptions: [
          {
            field: "string",
            direction: 0,
            priority: 0
          }
        ],
        filteringOptions: [
          {
            field: "string",
            operator: 0,
            value: {}
          }
        ]
      },
      headers: [
        { text: "Наименование", value: "name" },
        { text: "Описание", value: "description" },
        { text: "Цена, мм", value: "price" },
        { text: "Категория", value: "category.name" }
      ],
      options: {
        page: 1,
        itemsPerPage: 5,
        sortBy: [],
        sortDesc: [],
        groupBy: [],
        groupDesc: [],
        multiSort: false,
        mustSort: false
      },
      loading: false,
      totalServerCount: 0,
      products: []
    };
  },
  methods: {
    loadData() {
      this.loading = true;
      this.axios
        .post("api/Products/SearchProducts", this.pageInfo)
        .then(response => {
          this.products = response.data.items;
          this.totalServerCount = response.data.totalCount;
        })
        .catch(() => (this.products = []))
        .then(() => (this.loading = false));
    },
    changeOptions(newOption) {
      this.pageInfo.pageIndex = newOption.page;
      this.pageInfo.pageSize = newOption.itemsPerPage;
      this.loadData();
    }
  }
};
</script>
