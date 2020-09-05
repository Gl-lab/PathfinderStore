// profile/getters.ts
import { GetterTree } from 'vuex';
import { RootState } from '@/shared/models/shared/root-state';

export const getters: GetterTree<IAccount, RootState> = {
    getToken(state) {
        return state.token;
    },

    isSignedIn(state): boolean {
        return !!state.token;
    },

};
