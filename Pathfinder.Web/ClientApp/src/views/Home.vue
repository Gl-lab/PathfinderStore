<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png">
    <h1>Welcome to Your Vue.js App</h1>
     
  </div>
</template>

<script lang="ts">
import { Component, Vue} from 'vue-property-decorator';

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
export default class Home extends Vue {
  
}
</script>
