import Vue from 'vue';
import Vuex, { StoreOptions } from 'vuex';
import { RootState } from '@/shared/models/shared/root-state';
import { AuthStore } from './auth/auth-store';

Vue.use(Vuex);

const store: StoreOptions<RootState> = {
    state: {
        version: '1.0.0', // a simple property
      },
  modules: {
    AuthStore,
  },
};
export default new Vuex.Store<RootState>(store);
