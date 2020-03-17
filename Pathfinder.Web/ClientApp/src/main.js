import Vue from 'vue';
import App from './App.vue';
import router from './router';
Vue.config.productionTip = false;
Vue.prototype.$sync = function (key, value) {
    this.$emit(`update:${key}`, value);
};
new Vue({
    router,
    render: (h) => h(App),
}).$mount('#app');
//# sourceMappingURL=main.js.map