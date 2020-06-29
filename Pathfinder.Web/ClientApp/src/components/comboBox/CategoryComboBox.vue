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

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component';
import { Prop } from 'vue-property-decorator';
import axios from 'axios';
import { ICategory } from '../../shared/models/Interfaces/ICategory';
import { Category } from '../../shared/models/Category';

@Component

export default class CategoryComboBox extends Vue {

  @Prop({ required: true })
    public response!: ICategory;
  private categories: ICategory[] = [new Category(0, 'No data.', '')];
  private select: ICategory = null;
  public async mounted() {
    try {
      this.loadData();
    } catch {
      this.categories = [new Category(0, 'No data.', '')];
    }
  }

  private respond() {
    this.$sync<Category>('response', this.select);
  }

  private loadData() {
    axios.get('api/Categories').then((response) => {
      this.categories = response.data;
    });
  }
}
</script>