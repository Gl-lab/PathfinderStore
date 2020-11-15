<template>
  <div id="app">
    <v-app>
      <v-navigation-drawer app clipped>
        <v-list>
          <v-list-item :to="{ path: '/' }">
            <v-list-item-action>
              <v-icon>mdi-widgets</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>
                Домашняя
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item :to="{ path: '/vuetifyproduct' }">
            <v-list-item-action>
              <v-icon>mdi-home</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>
                Товары
              </v-list-item-title>
            </v-list-item-content>
          </v-list-item>
          <v-list-item :to="{ path: '/about' }">
            <v-list-item-action>
              <v-icon>mdi-heart</v-icon>
            </v-list-item-action>
            <v-list-item-content>
              <v-list-item-title>
                About
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
        <v-toolbar-title>App Bar</v-toolbar-title>
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
import { createNamespacedHelpers } from 'vuex';
const { mapActions, mapGetters } = createNamespacedHelpers('auth');

export default {
  name: "Home",
  computed: {
    ...mapGetters(['getToken','isAuthorized'])
  },
  methods: {
    ...mapActions(['logout']),
    login(){
      this.$router.push('/account/login');
    }
  },
  created: function () {
    console.log(123)
    const text = 'Bearer '+this.getToken
    this.$http.defaults.headers.common['Authorization'] = text;
    this.$http.interceptors.response.use(undefined, function (err) {
      return new Promise(function () {
        if (err.status === 401 && err.config && !err.config.__isRetryRequest) {
          this.logout();
        }
        throw err;
      });
    });
  }
};
</script>
