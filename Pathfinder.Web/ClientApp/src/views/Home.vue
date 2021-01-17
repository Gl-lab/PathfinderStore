<template>
  <div class="home">
    <h1>Все хорошо! Вы на главной</h1>
    <characters-list/>
  </div>
</template>
s
<script>
import CharactersList from "@/components/CharactersList";
export default {
  name: "Home",
  components: {
    CharactersList
  }
};
</script>
