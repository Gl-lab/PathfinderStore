import { Module } from 'vuex';
// import { getters } from './getters';
import { actions } from './actions';
import { mutations } from './mutations';
// import { IAccount } from '@/shared/models/users/account';
import { RootState } from '@/shared/models/shared/root-state';

export const state: IAccount = {
  token: undefined,
  allRoles: [],
};

const namespaced: boolean = true;

export const AuthStore: Module<IAccount, RootState> = {
  namespaced,
  state,
  // getters,
  actions,
  mutations,
};
// export default class AuthStore {
//   public static storageKey: string = 'token';

//   public static getToken() {
//     return localStorage.getItem(AuthStore.storageKey);
//   }

//   public static setToken(token: string) {
//     localStorage.setItem(AuthStore.storageKey, token);
//   }

//   public static removeToken(): void {
//     localStorage.removeItem(AuthStore.storageKey);
//   }

//   public static isSignedIn(): boolean {
//     return !!AuthStore.getToken();
//   }


//   public static getTokenData() {
//     const token = AuthStore.getToken();
//     if (token) {
//       return JSON.parse(atob(token.split('.')[1]));
//     }

//     return {};
//   }
// }
