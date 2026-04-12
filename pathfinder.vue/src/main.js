import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import vuetify from "./plugins/vuetify";
import VueAxios from "vue-axios";
import axios from "axios";

import { appConst } from "@/settings";

Vue.config.productionTip = false;
axios.defaults.baseURL = appConst.webApiUrl;
Vue.use(VueAxios, axios);
new Vue({
  router,
  store,
  vuetify,
  render: h => h(App)
}).$mount("#app");
