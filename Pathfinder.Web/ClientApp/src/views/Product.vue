
<template>
  <v-content>
    <v-container fluid>
      <v-btn><router-link to="/create">Добавить продукт</router-link> </v-btn>
      <table class="table table-bordered table-dark">
        <thead>
          <tr>
            <th v-for="(item, index) in productsCols" v-bind:key="index"> 
              {{ item.label }}
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in products" v-bind:key="index">
            <td v-for="(col, index) in productsCols" v-bind:key="index">
              {{ col.field(item) }}
            </td>
          </tr>
        </tbody>
      </table>
      <v-btn @click="prevPage()" :disabled = "hasPreviousPage">Назад</v-btn> 
      <v-btn @click="nextPage()" :disabled = "hasNextPage">Дальше</v-btn>
    </v-container>
  </v-content>
  
</template>

<script lang="ts">
import { Component, Vue} from 'vue-property-decorator';
import { IProduct } from '../models/Interfaces/IProduct';
import { IPageArgs } from '../models/Interfaces/IPageArgs'; 
import { SortingDirection } from '../models/enums/SortingDirection';
import { PagingStrategy } from '../models/enums/PagingStrategy';
import { FilteringOperator } from '../models/enums/FilteringOperator';
import { Category } from '../models/Category'
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.css'

@Component
export default class Product extends Vue {
    private pageInfo: IPageArgs  = {
        pageIndex: 1, 
        pageSize: 5, 
        pagingsStrategy: PagingStrategy.WithCount, 
        sortingOptions: [{ 
            field: "string",
            direction: SortingDirection.ASC,
            priority: 0 
        }],
        filteringOptions: [{
            field: "string",
            operator: 0,
            value: {}
        }]
    };
    private hasPreviousPage: boolean = false;
    private hasNextPage: boolean = false;
    private products: IProduct[] = [{ name: 'No data.', category: new Category(0,'','') } as IProduct];
    private productsCols: any[] = [
        { name: 'Name',         label: 'Наименование',  field: (row: IProduct) => row.name },
        { name: 'Description',  label: 'Описание',      field: (row: IProduct) => row.description },
        { name: 'Price',        label: 'Цена',          field: (row: IProduct) => row.price },
        { name: 'Category',     label: 'Категория',     field: (row: IProduct) => row.category.name },
    ];
 
    public async loadData(){
        axios.post('api/Products/SearchProducts',this.pageInfo)
            .then(response => {
                this.products = response.data.items;
                this.hasPreviousPage = !(response.data.pageIndex > 1);
                this.hasNextPage = !(response.data.pageIndex < response.data.totalPages);
        });
    }

    private prevPage(){
        this.pageInfo.pageIndex--;
        this.loadData();
    }
    private nextPage(){
        this.pageInfo.pageIndex++;
        this.loadData();
    }

    public async mounted() {
        try {
          this.loadData();
        } catch {
          this.products = [{ name: 'No data.' } as IProduct];
        }
    }
}
</script>
