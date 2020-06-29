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
        <cb :response.sync="product.category"/>
      </v-col>
        <div class="col-sm-2">
           <v-btn class="mr-4" @click="createProduct">Создать</v-btn>
        </div>
        <div class="danger">{{errorText}}</div>
    </div>
</template>
<script src="./CreateProductForm.ts"></script>