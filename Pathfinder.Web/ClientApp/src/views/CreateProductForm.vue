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
<script src="./CreateProductForm.ts"></script>