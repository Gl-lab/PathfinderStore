<template>
  <div>
    <v-col cols="12" sm="6">
      <v-text-field
        v-model="product.name"
        label="Наименование"
        required
      ></v-text-field>
    </v-col>
    <v-col cols="12" sm="6">
      <v-textarea
        v-model="product.description"
        label="Описание"
        required
        filled
        auto-grow
      ></v-textarea>
    </v-col>
    <v-col cols="12" sm="6">
      <v-text-field
        v-model="product.price"
        label="Цена"
        min="0"
        step="1"
        type="number"
      ></v-text-field>
    </v-col>
    <v-col cols="12" sm="6">
      <category-combo-box @response="selectCategory" />
    </v-col>
    <div class="col-sm-2">
      <v-btn class="mr-4" @click="createProduct">Создать</v-btn>
    </div>
    <div class="danger">{{ errorText }}</div>
  </div>
</template>
<script>
import CategoryComboBox from "../components/comboBox/CategoryComboBox.vue";
import Product from "../models/Product";

export default {
  name: "CreateProductForm",
  components: {
    CategoryComboBox
  },
  data() {
    return {
      product: new Product(),
      resultCode: 0,
      errorText: ""
    };
  },
  methods: {
    selectCategory(category) {
      this.product.categoryId = category.id;
    },
    createProduct() {
      if (
        this.product.name !== "" &&
        this.product.description !== "" &&
        this.product.price >= 0
      ) {
        this.product.category = null;
        this.product.price = Number.parseFloat(this.product.price);
        try {
          this.axios
            .post("/api/Products/CreateProduct", this.product)
            .then(response => (this.resultCode = response.status))
            .then(() => {
              if (this.resultCode !== 200) {
                this.errorText = "Неудачно";
              } else {
                this.$router.push("vuetifyproduct");
              }
            });
        } catch {
          this.errorText = "Неудачно";
        }
      }
    }
  }
};
</script>
