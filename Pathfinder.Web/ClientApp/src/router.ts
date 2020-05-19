import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';
import CategoryComboBox from './views/comboBox/CategoryComboBox.vue'


Vue.use(Router);

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
    },
    {
      path: '/product',
      name: 'product',
      component: () => import(/* webpackChunkName: "about" */ './views/Product.vue'),
    },
    {
      path: '/cb',
      name: 'category-combo-box',
      component: CategoryComboBox,
    },
    {
      path: '/about',
      name: 'about',
      component: () => import(/* webpackChunkName: "about" */ './views/About.vue'),
    },
    {
      path: '/create',
      name: 'create',
      component: () => import(/* webpackChunkName: "about" */ './views/CreateProductForm.vue'),
    },

  ],
});
