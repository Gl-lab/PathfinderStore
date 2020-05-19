<template>
    <v-data-table
      :headers="headers"
      :items="desserts"
      :items-per-page="5"
      class="elevation-1"
    ></v-data-table>
</template>
<script lang="ts">
import { Component, Vue} from 'vue-property-decorator';
import axios from 'axios'
@Component
export default class VuetifyProduct extends Vue {
    
}
</script>