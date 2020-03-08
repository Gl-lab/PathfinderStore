<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png">
    <h1>Welcome to Your Vue.js App</h1>
    <table>
      <thead>
        <tr>
          <th v-for="(item, index) in productsCols" v-bind:key="index"> 
            {{ item.label | capitalize }}
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
    <button class="btn btn-primary" @click="prevPage()" :disabled = "hasPreviousPage">Назад</button> 
    <button class="btn btn-primary" @click="nextPage()" :disabled = "hasNextPage">Дальше</button>
  </div>
  
</template>

<script lang="ts">
import { Component, Vue} from 'vue-property-decorator';
import { IProduct } from '../models/IProduct';
import { IPageArgs } from '../models/IPageArgs'; 
import axios from 'axios';

@Component({
  filters: {
    capitalize: (value: string) => {
      if (!value) { return ''; }
      value = value.toString();
      return value.charAt(0).toUpperCase() + value.slice(1);
    },
  },
})
export default class Product extends Vue {
    private pageInfo: IPageArgs  = {
        pageIndex: 1, 
        pageSize: 5, 
        pagingsStrategy: 0, 
        sortingOptions: [{ 
            field: "string",
            direction: 0,
            priority: 0 
        }],
        filteringOptions: [{
            field: "string",
            operator: 0,
            value: {}
        }]
    };
    private hasPreviousPage: boolean;
    private hasNextPage: boolean;
    private products: IProduct[] = [{ name: 'No data.' } as IProduct];
    private productsCols: any[] = [
        { name: 'Name',         label: 'Наименование',  field: (row: IProduct) => row.name },
        { name: 'Description',  label: 'Описание',      field: (row: IProduct) => row.description },
        { name: 'Price',        label: 'Цена',          field: (row: IProduct) => row.price },
        { name: 'Category',     label: 'Категория',     field: (row: IProduct) => row.categoryId },
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
