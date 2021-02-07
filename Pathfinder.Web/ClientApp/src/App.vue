<template>
  <div id="app">
    <v-app>
      <v-navigation-drawer app clipped>
        <v-list>
          <v-list-item
            v-for="route in $router.options.routes.filter(
              item => item.meta && item.meta.mainMenu
            )"
            :key="route.name"
            :to="{ path: route.path }"
          >
            <v-list-item-action>
              <v-icon>{{ route.meta.mainMenu.icon }}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>
                {{ route.meta.mainMenu.title }}
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>
        <template v-slot:append>
          <div v-if="isAuthorized" class="pa-2">
            <v-btn @click="logout" block>Выйти</v-btn>
          </div>
          <div v-else class="pa-2">
            <v-btn @click="login">Login</v-btn>
          </div>
        </template>
      </v-navigation-drawer>
      <v-app-bar app clipped-left>
        <v-avatar class="mr-5" color="primary white--text" size="50">
          <div v-if="isLoadedAccount">
            {{ getUserName }}
          </div>
        </v-avatar>
      </v-app-bar>
      <v-main>
        <v-container fluid>
          <v-fade-transition mode="out-in">
            <router-view></router-view>
          </v-fade-transition>
        </v-container>
      </v-main>
    </v-app>
  </div>
</template>

<script>
import { createNamespacedHelpers } from "vuex";
const { mapActions, mapGetters } = createNamespacedHelpers("auth");

export default {
  name: "Home",
  computed: {
    ...mapGetters([
      "getToken",
      "isAuthorized",
      "isLoadedAccount",
      "getUserName"
    ])
  },
  methods: {
    ...mapActions(["logout", "loadAccount"]),
    login() {
      this.$router.push("/account/login");
    }
  },
  created: function() {
    const text = "Bearer " + this.getToken;
    this.$http.defaults.headers.common["Authorization"] = text;
    this.$http.interceptors.response.use(undefined, function(err) {
      return new Promise(function() {
        if (err.status === 401 && err.config && !err.config.__isRetryRequest) {
          this.logout();
        }
        throw err;
      });
    });
    if (this.isAuthorized) this.loadAccount();
  }
};
</script>
