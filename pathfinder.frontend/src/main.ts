import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from '@/App.vue'
import { router } from '@/app/router'
import { vuetify } from '@/app/vuetify'
import { i18n } from '@/i18n'
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
import '@/styles/base.css'

createApp(App).use(createPinia()).use(router).use(vuetify).use(i18n).mount('#app')
