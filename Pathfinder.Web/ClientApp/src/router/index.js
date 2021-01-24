import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import VuetifyProduct from "../views/VuetifyProduct.vue";
import accountLayout from "@/account/account-layout.vue";
import store from "@/store";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/about",
    name: "About",
    component: () => import("../views/About.vue")
  },
  {
    path: "/create",
    meta: { requiresAuth: true },
    name: "create",
    component: () => import("../views/CreateProductForm.vue")
  },
  {
    path: "/vuetifyproduct",
    name: "VuetifyProduct",
    component: VuetifyProduct
  },
  {
    path: "/account",
    component: accountLayout,
    children: [
      {
        path: "login",
        component: require("@/account/views/login.vue").default
      },
      {
        path: "register",
        component: require("@/account/views/register.vue").default
      }
    ]
  }
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes
});

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!store.getters["auth/isAuthorized"]) {
      next({
        path: "/account/login",
        query: { redirect: to.fullPath }
      });
    }
  }
  next();
});

export default router;