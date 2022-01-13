<template>
  <v-card class="elevation-12">
    <v-toolbar dark color="primary">
      <v-toolbar-title>Login</v-toolbar-title>
    </v-toolbar>
    <v-card-text>
      <div v-for="error in errors" :key="error.id">
        <v-alert type="error">
          {{ error.value }}
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
      <v-btn color="primary" text to="/account/forgot-password">
        ForgotPassword
      </v-btn>
      <v-btn color="primary" text to="/account/register">Register</v-btn>
      <v-btn color="primary" @click="onSubmit">Login</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import {createNamespacedHelpers} from "vuex";
import axios from "axios";
import {appConst} from "@/settings";

const { mapActions, mapMutations } = createNamespacedHelpers("auth");
export default {
  data() {
    return {
      loginInput: {},
      errors: [],
      isHaveError: false
    };
  },
  methods: {
    ...mapMutations(["removeToken", "setToken"]),
    ...mapActions(["loadAccount"]),
    onSubmit() {
      if (this.$refs.form.validate()) {
        axios.post(appConst.webApiUrl + "/api/login", this.loginInput).then(
          response => {
            const token = response.data.token;
            axios.defaults.headers.common["Authorization"] = "Bearer " + token;
            this.setToken(token);
            this.loadAccount();
            this.$router.push("/");
          },
          err => {
            err.response.data.forEach(x =>
                this.errors.push({value: x.value, id: this.errors.length})
            );
            this.removeToken();
          }
        );
      }
    },
    requiredError: v => !!v || "RequiredField"
  }
};
</script>
