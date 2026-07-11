import { createVuetify } from 'vuetify'
import { ru } from 'vuetify/locale'

export const vuetify = createVuetify({
  locale: {
    locale: 'ru',
    messages: { ru },
  },
  theme: {
    defaultTheme: 'characterLedger',
    themes: {
      characterLedger: {
        dark: false,
        colors: {
          background: '#F6F1E8',
          surface: '#FFFDF8',
          'surface-variant': '#EAE1D4',
          primary: '#162B42',
          secondary: '#236B68',
          accent: '#B66B22',
          error: '#A3342B',
          warning: '#9A6515',
          success: '#2D6A4F',
          info: '#365F8B',
        },
      },
    },
  },
  defaults: {
    VBtn: {
      rounded: 'lg',
    },
    VCard: {
      rounded: 'xl',
    },
  },
})
