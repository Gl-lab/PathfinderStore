<template>
    <div class="container">
       <select v-model="selected">
        <option v-for="option in categories" v-bind:key="option.id">
            {{ option.name }}
        </option>
        </select>
    </div>
</template>

<script lang="ts">
import Vue from 'vue';
import Component from 'vue-class-component'
import axios from 'axios';
import { ICategory } from '../../models/ICategory';

    @Component
    
export default class CategoryComboBox extends Vue {
    private categories: ICategory[] = [{ name: 'No data.', id: 0 , description: ''} as ICategory];
    public selected: string = '';
    private loadData(){
        axios.get('api/Categories').then(response => {
            this.categories = response.data;
            if (this.selected == '')
                this.selected = response.data[0].name;
        })
    }
    public async mounted() {
        try {
        this.loadData();
        } catch {
        this.categories = [{ name: 'No data.', id: 0 , description: ''} as ICategory];
        }
    }
}
</script>