import { MutationTree } from 'vuex';

export const mutations: MutationTree<IAccount> = {
    setToken(state, token: string) {
        state.token = token;
        localStorage.setItem('AuthStore', JSON.stringify(state));
    },

    removeToken(state) {
        state.token = undefined;
        localStorage.setItem('AuthStore', JSON.stringify(state));
    },
};
