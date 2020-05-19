<template>
    <div class="container">
        <select v-model="categoryName" @change="respond">
            <option v-for="option in categories" :key="option.id">
                {{ option.name }}
            </option>
        </select>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component'
import { Prop } from 'vue-property-decorator'
import axios from 'axios';
import { ICategory } from '../../models/Interfaces/ICategory';
import { Category } from '../../models/Category';

@Component
    
export default class CategoryComboBox extends Vue {
    private categories: ICategory[] = [new Category(0,'No data.','')];
   
    @Prop ({required: true})
    public response!: ICategory;
    private categoryName: string = '';
    
    private respond(){
        this.$sync<Category>('response', this.categories.find(item => item.name == this.categoryName) as Category);
    }

    private loadData(){
        axios.get('api/Categories').then(response => {
            this.categories = response.data;
            this.categoryName = response.data[0].name;
        }).then(()=>this.respond());
    }
    public async mounted() {
        try {
            this.loadData();
        } catch {
            this.categories = [new Category(0,'No data.','')];
        }
    }
}
</script>