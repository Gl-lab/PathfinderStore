<template>
  <div id="app">
    <div id="nav">
      <router-link to="/">Домашняя</router-link> |
      <router-link to="/product">Товары</router-link> |
      <router-link to="/cb">Тест</router-link> |
      <router-link to="/about">About</router-link> 
    </div>
    <router-view/>
  </div>
</template>

<script lang="ts">
import { Component, Vue} from 'vue-property-decorator';
@Component
export default class App extends Vue {

}
</script>