import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import CharactersDashboardView from '@/views/CharactersDashboardView.vue'
import NotFoundView from '@/views/NotFoundView.vue'
import LoginView from '@/views/LoginView.vue'
import RegisterView from '@/views/RegisterView.vue'
import CharacterDetailsView from '@/views/CharacterDetailsView.vue'
import CharacterCreateView from '@/views/CharacterCreateView.vue'
import CampaignsView from '@/views/CampaignsView.vue'
import { useAuthStore } from '@/features/auth/store'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'characters',
    component: CharactersDashboardView,
    meta: { title: 'routes.characters', requiresAuth: true },
  },
  {
    path: '/login',
    name: 'login',
    component: LoginView,
    meta: { title: 'routes.login' },
  },
  {
    path: '/register',
    name: 'register',
    component: RegisterView,
    meta: { title: 'routes.register' },
  },
  {
    path: '/characters',
    redirect: '/',
  },
  {
    path: '/characters/:id(\\d+)',
    name: 'character-details',
    component: CharacterDetailsView,
    meta: { title: 'routes.characterDetails', requiresAuth: true },
  },
  {
    path: '/characters/create',
    name: 'character-create',
    component: CharacterCreateView,
    meta: { title: 'routes.characterCreate', requiresAuth: true },
  },
  {
    path: '/campaigns',
    name: 'campaigns',
    component: CampaignsView,
    meta: { title: 'routes.campaigns', requiresAuth: true },
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: NotFoundView,
    meta: { title: 'routes.notFound' },
  },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior: () => ({ top: 0 }),
})

router.beforeEach((to) => {
  const auth = useAuthStore()

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if ((to.name === 'login' || to.name === 'register') && auth.isAuthenticated) {
    return { name: 'characters' }
  }

  return true
})
