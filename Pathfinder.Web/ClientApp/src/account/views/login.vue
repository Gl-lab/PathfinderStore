﻿<template>
  <v-card class="elevation-12">
    <v-toolbar dark color="primary">
      <v-toolbar-title>Login</v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <div v-for="error in errors" :key="error.name">
        <v-alert :value="isHaveError" type="error">
          {{ error.name }}
        </v-alert>
      </div>
      <v-form ref="form" @keyup.native.enter="onSubmit">
        <v-text-field
          prepend-icon="mdi-account"
          name="userNameOrEmail"
          type="text"
          label="UserNameOrEmailAddress"
          v-model="loginInput.userNameOrEmail"
          :rules="[requiredError]"
        ></v-text-field>
        <v-text-field
          prepend-icon="mdi-lock"
          name="password"
          type="password"
          label="Password"
          v-model="loginInput.password"
          :rules="[requiredError]"
        ></v-text-field>
      </v-form>
    </v-card-text>
    <v-card-actions class="pa-5">
      <v-spacer></v-spacer>
      <v-btn color="primary" text to="/account/forgot-password"
        >ForgotPassword</v-btn
      >
      <v-btn color="primary" text to="/account/register">Register</v-btn>
      <v-btn color="primary" @click="onSubmit">Login</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import { createNamespacedHelpers } from "vuex";
const { mapActions } = createNamespacedHelpers("auth");
export default {
  data() {
    return {
      loginInput: {},
      errors: [],
      isHaveError: false
    };
  },
  methods: {
    ...mapActions(["login"]),
    onSubmit() {
      if (this.$refs.form.validate()) {
        this.login(this.loginInput);
        this.$router.push("/");
      }
    },
    requiredError: v => !!v || "RequiredField"
  }
};
</script>
