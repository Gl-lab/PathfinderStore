import Vue from 'vue';
import App from './App.vue';
import router from './router';
import vuetify from './plugins/vuetify';
import '@babel/polyfill';
import 'roboto-fontface/css/roboto/roboto-fontface.css';
import '@mdi/font/css/materialdesignicons.css';
import VueAxios from 'vue-axios';
import axios from 'axios';

Vue.config.productionTip = false;
Vue.use(VueAxios, axios);

Vue.prototype.$sync = function(key: string, value: any) {
  this.$emit(`update:${key}`, value);
};

const token = localStorage.getItem('token');
if (token != null) {
  Vue.prototype.$http.defaults.headers.common.Authorization = `Bearer ${token}`;
}

new Vue({
  router,
  vuetify,
  render: (h) => h(App),
}).$mount('#app');
