import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from '@/App.vue'
import { router } from '@/app/router'
import { vuetify } from '@/app/vuetify'
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
import '@/styles/base.css'

createApp(App).use(createPinia()).use(router).use(vuetify).mount('#app')
