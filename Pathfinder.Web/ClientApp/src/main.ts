import Vue from 'vue';
import App from './App.vue';
import router from './router';

Vue.config.productionTip = false;

Vue.prototype.$sync = function(key: string, value: any) {
  this.$emit(`update:${key}`, value);
};

new Vue({
  router,
  render: (h) => h(App),
}).$mount('#app');
