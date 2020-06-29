import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';
import VuetifyProduct from './views/VuetifyProduct.vue';
import accountLayout from '@/account/account-layout.vue';
import AuthStore from '@/stores/auth-store';


Vue.use(Router);

const router = new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home,
    },
    {
      path: '/vuetifyproduct',
      meta: { requiresAuth: true },
      name: 'VuetifyProduct',
      component: VuetifyProduct,
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('./views/About.vue'),
    },
    {
      path: '/create',
      name: 'create',
      component: () => import('./views/CreateProductForm.vue'),
    },
    {
      path: '/account',
      component: accountLayout,
      children: [
          { path: 'login', component: require('@/account/views/login/login.vue').default },
          { path: 'register', component: require('@/account/views/register/register.vue').default },
          { path: 'forgot-password', component: require('@/account/views/manage/forgot-password.vue').default },
          { path: 'reset-password', component: require('@/account/views/manage/reset-password.vue').default },
      ],
    },

  ],
});

router.beforeEach((to: any, from: any, next: any) => {
  if (to.matched.some((record: any) => record.meta.requiresAuth)) {
    if (!AuthStore.isSignedIn()) {
      next({
        path: '/account/login',
        query: { redirect: to.fullPath },
      });
    }
  }
  next(); // make sure to always call next()!
});

export default router;
