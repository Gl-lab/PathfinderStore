<template>
  <div v-if="!registerComplete">
    <v-card class="elevation-12">
      <v-toolbar dark color="primary">
        <v-toolbar-title>Регистрация</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <div v-for="error in errors" :key="error.name">
          <v-alert :value="true" type="error">
            {{ error.name }}
          </v-alert>
        </div>
        <v-form ref="form" @keyup.native.enter="onSubmit">
          <v-text-field
              name="userName"
              label="Никнейм"
              type="text"
              v-model="registerInput.userName"
              :rules="[requiredError]"
          ></v-text-field>
          <v-text-field
              name="email"
              label="Электронная почта"
              type="text"
              v-model="registerInput.email"
              :rules="[requiredError]"
          ></v-text-field>
          <v-text-field
              name="password"
              label="Пароль"
              type="password"
              v-model="registerInput.password"
              :rules="[requiredError]"
          ></v-text-field>
          <v-text-field
              name="passwordRepeat"
              label="Повтор пароля"
              v-model="registerInput.passwordRepeat"
              type="password"
              :rules="[requiredError]"
              :error-messages="
              passwordMatchError(
                registerInput.password,
                registerInput.passwordRepeat
              )
            "
          ></v-text-field>
        </v-form>
      </v-card-text>
      <v-card-actions class="pa-5">
        <v-spacer></v-spacer>
        <v-btn color="primary" text to="/account/login">Войти</v-btn>
        <v-btn color="primary" @click="onSubmit">Регистрация</v-btn>
      </v-card-actions>
    </v-card>
  </div>
  <div v-else>
    <v-card class="elevation-12">
      <v-toolbar dark color="primary">
        <v-toolbar-title>Register</v-toolbar-title>
      </v-toolbar>
      <v-card-text>
        <v-alert :value="true" type="success">
          {{ resultMessage }}
        </v-alert>
      </v-card-text>
      <v-card-actions class="pa-5">
        <v-spacer></v-spacer>
        <v-btn color="primary" text to="/account/login">Login</v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>

<script>
export default {
  data() {
    return {
      registerInput: {},
      errors: [],
      resultMessage: "",
      registerComplete: false,
      isHaveError: false
    };
  },
  methods: {
    onSubmit() {
      if (this.$refs.form.validate()) {
        this.axios
          .post("/api/register", this.registerInput)
          .then(() => {
            this.resultMessage = "Регистрация прошла успешно";
            this.registerComplete = true;
          })
          .catch(err => {
            this.errors = err.response.data;
            this.isHaveError = true;
          });
      }
    },
    passwordMatchError(password, passwordRepeat) {
      return password === passwordRepeat ? "" : "Пароли не совпадают";
    },
    requiredError: v => !!v || "Обязательное поле"
  }
};
</script>
