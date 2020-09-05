import { ActionTree } from 'vuex';
import axios from 'axios';
import { RootState } from '@/shared/models/shared/root-state';


export const actions: ActionTree<IAccount, RootState> = {
  login({ commit }, user: ILoginInput): any {
    axios({
      url: '/api/login',
      data: user,
    }).then((response) => {
      const token: string = response.data.token;
      commit('setToken', token);
    }, () => {
      commit('removeToken');
    });
  },
  logout({commit}): any {
    commit('removeToken');
  },
};

