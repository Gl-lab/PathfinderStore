<template>
  <div>
    <v-btn>
      <router-link to="/create">Добавить продукт</router-link> 
    </v-btn>
    <v-data-table
      :headers="headers"
      :items="products"
      :items-per-page="pageInfo.pageSize"
      :server-items-length="totalServerCount"
      :loading="loading"
      :options.sync="options"
      locale="ru-RU"
      class="elevation-1"
    ></v-data-table>
</div>
</template>
<script lang="ts">
import { Component, Vue, Watch } from 'vue-property-decorator';
import { IProduct } from '../shared/models/Interfaces/IProduct';
import { IPageArgs } from '../shared/models/Interfaces/IPageArgs';
import { SortingDirection } from '../shared/models/enums/SortingDirection';
import { PagingStrategy } from '../shared/models/enums/PagingStrategy';
import { FilteringOperator } from '../shared/models/enums/FilteringOperator';
import { Category } from '../shared/models/Category';
import axios from 'axios';
import { DataOptions } from 'vuetify';
@Component
export default class VuetifyProduct extends Vue {
  private pageInfo: IPageArgs  = {
    pageIndex: 1,
    pageSize: 5,
    pagingsStrategy: PagingStrategy.WithCount,
    sortingOptions: [{
      field: 'string',
      direction: SortingDirection.ASC,
      priority: 0,
    }],
    filteringOptions: [{
      field: 'string',
      operator: 0,
      value: {},
    }],
  };
  private headers = [{ text: 'Наименование', value: 'name' },
                     { text: 'Описание', value: 'description' },
                     { text: 'Цена', value: 'price' },
                     { text: 'Категория', value: 'categoryId' },
  ];
  private options: DataOptions = { page: this.pageInfo.pageIndex,
    itemsPerPage: this.pageInfo.pageSize,
    sortBy: [],
    sortDesc: [],
    groupBy: [],
    groupDesc: [],
    multiSort: false,
    mustSort: false,
  };
  private loading: boolean = false;
  private totalServerCount: number = 0;
  private emptyProduct: IProduct = {
    name: 'No data.',
    category: new Category(0, '', ''),
    price: 0, categoryId: 0,
  } as IProduct;
  private products: IProduct[] = [this.emptyProduct];
  public async mounted() {
    this.loadData();
  }
  @Watch('options')
  public optionsChanged() {
    this.pageInfo.pageIndex = this.options.page;
    this.pageInfo.pageSize = this.options.itemsPerPage;
    this.loadData();
  }
  private async loadData() {
    this.loading = true;
    axios.post('api/Products/SearchProducts', this.pageInfo)
      .then((response) => {
        this.products = response.data.items;
        this.totalServerCount = response.data.totalCount;
      })
      .catch(() => this.products = [this.emptyProduct]).then(() => this.loading = false);
  }
}
</script>