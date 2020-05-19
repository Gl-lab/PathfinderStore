<template>
    <div class="container">
        <div class="col-sm-2"> 
            <label> Наименование </label>
            <input  type="text" v-model="product.name"/>
        </div>
        <div class="col-sm-2"> 
            <label> Описание </label>
            <textarea v-model="product.description"></textarea>
        </div>
        <div class="col-sm-2"> 
            <label> Цена </label>
            <input type="number" v-model.number="product.price"/>
        </div>
        <div class="col-sm-2">
            <label> Категория </label>
            <cb :response.sync="product.category"/>
        </div>
        <div class="col-sm-2">
           <button class="btn btn-primary" @click="createProduct">Создать</button>
        </div>
        <div class="danger">{{errorText}}</div>
    </div>
</template>
<script lang="ts">
import { Component, Vue} from 'vue-property-decorator';
import axios from 'axios';
import { IProduct } from '../models/Interfaces/IProduct';
import CategoryComboBox from '../components/comboBox/CategoryComboBox.vue';
import { Product } from '../models/Product';
 
@Component({
    components: {
        ['cb'] :CategoryComboBox
    },
})

export default class CreateProductForm extends Vue {
    private product: Product = new Product();
    private resultCode: number;
    private errorText: string = '';
    private createProduct(): void {
        let productModel: Product = new Product();
        productModel.copyModel(this.product);
        productModel.category = null;
        if ( this.product.name != '' 
            && this.product.description != '' 
            && this.product.price >= 0) {
            try {
                axios.post('/api/Products/CreateProduct', productModel)
                        .then(response => this.resultCode = response.status)
                        .then(()=>{
                            if (this.resultCode != 200) {
                                this.errorText = 'Неудачно' ;
                            } else {
                                this.$router.push('product');
                            }
                        });
            } catch
            {
                this.errorText = 'Неудачно';
            }
        }
            
        
    }
}


</script>