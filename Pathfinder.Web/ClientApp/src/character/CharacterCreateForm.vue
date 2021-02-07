<template>
  <v-card>
  <v-form 
    v-model="valid"
    ref="form"
  >
    <v-text-field
        v-model="newCharacter.name"
        :rules="nameRules"
        label="Name"
        required
    ></v-text-field>
    <v-btn
      :disabled="!valid"
      color="success"
      class="mr-4"
      @click="сreateCharacter"
    >
      Создать
    </v-btn>
    <v-btn
      color="error"
      class="mr-4"
      @click="reset"
    >
      Сброс
    </v-btn>
  </v-form>
  </v-card>
</template>
<script>

export default {
  name: "CharacterCreateForm",
  data() {
    return {
      valid: true,
      newCharacter: { name: null, balance: 0},
      resultCode: 0,
       nameRules: [
        v => !!v || 'Name is required',
        v => (v && v.length <= 10) || 'Name must be less than 10 characters',
      ],
      errorText: ""
    };
  },
  methods: {
    methods: {
      reset () {
        this.$refs.form.reset()
      },
    },
    сreateCharacter() {
      if (this.$refs.form.validate())
      {
       try {
        this.axios
          .post("/api/Character", this.newCharacter)
          .then(response => (this.resultCode = response.status))
          .then(() => {
            if (this.resultCode !== 200) {
              this.errorText = "Неудачно";
            } else {
              this.$emit('complite')
            }
          });
      } catch {
        this.errorText = "Неудачно";
      }
      }
    }
  }
};
</script>
