<template>
  <div>
    <v-data-table
      :headers="[{ text: 'Наименование', value: 'name',},
                 { text: 'Описание', value: 'description' },
                 { text: 'Цена', value: 'price' },
                 { text: 'Категория', value: 'categoryId' },
      ]"
      :items="products"
      :items-per-page="5"
      class="elevation-1"
    ></v-data-table>
</div>
</template>
<script lang="ts">
import { Component, Vue} from 'vue-property-decorator';
import { IProduct } from '../models/Interfaces/IProduct';
import { IPageArgs } from '../models/Interfaces/IPageArgs'; 
import { SortingDirection } from '../models/enums/SortingDirection';
import { PagingStrategy } from '../models/enums/PagingStrategy';
import { FilteringOperator } from '../models/enums/FilteringOperator';
import { Category } from '../models/Category'
import axios from 'axios'
@Component
export default class VuetifyProduct extends Vue {
  private pageInfo: IPageArgs  = {
        pageIndex: 1, 
        pageSize: 10, 
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
  private products: IProduct[] = [{ name: 'No data.', category: new Category(0,'','') ,price: 0,categoryId: 0} as IProduct];
  private async loadData(){
        axios.post('api/Products/SearchProducts',this.pageInfo)
            .then(response => {
                this.products = response.data.items;
        });
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