import Vue from 'vue';
import App from './App.vue';
import router from './router';
import vuetify from './plugins/vuetify';
import '@babel/polyfill';
import 'roboto-fontface/css/roboto/roboto-fontface.css';
import '@mdi/font/css/materialdesignicons.css';
import VueI18n from 'vue-i18n';
import LanguageStore from '@/stores/language-store';

Vue.use(VueI18n);
Vue.config.productionTip = false;

const locales = {
  en: require('@/assets/localizations/en.json'),
};

const i18n = new VueI18n({
  locale: LanguageStore.getLanguage().languageCode,
  fallbackLocale: 'en',
  messages: locales,
});

Vue.prototype.$sync = function(key: string, value: any) {
  this.$emit(`update:${key}`, value);
};

new Vue({
  i18n,
  router,
  vuetify,
  render: (h) => h(App),
}).$mount('#app');
