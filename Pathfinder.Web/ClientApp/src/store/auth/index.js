import axios from "axios";
import { appConst } from "../../settings";

const Auth = {
  namespaced: true,
  state: {
    token: null,
    account: null
  },
  getters: {
    isAuthorized: state => {
      if (!state.token) {
        const temp = localStorage.getItem("AuthStore");
        if (temp) {
          state = JSON.parse(temp);
        }
      }
      return !!state.token;
    },
    getToken: state => {
      if (!state.token) {
        const temp = localStorage.getItem("AuthStore");
        if (temp) {
          state = JSON.parse(temp);
        }
      }
      return state.token;
    },
    isLoadedAccount: state => !!state.account,
    currentCharacter: state => {
      if (state.account && state.account.currentCharacter) {
        return state.account.currentCharacter;
      } else return null;
    },
    characterlist: state => {
      if (state.account && state.account.characters) {
        return state.account.characters;
      } else return [];
    },
    getUserName: state => {
      if (state.account && state.account.user) {
        return state.account.user.userName;
      }
      return null;
    }
  },
  actions: {
    login(context, user) {
      axios.post(appConst.webApiUrl + "/api/login", user).then(
        response => {
          const token = response.data.token;
          axios.defaults.headers.common["Authorization"] = "Bearer " + token;
          context.commit("setToken", token);
          context.dispath("loadAccount");
        },
        () => {
          context.commit("removeToken");
        }
      );
    },
    loadAccount(context) {
      if (context.getters.isAuthorized) {
        return axios.get(appConst.webApiUrl + "/api/Account").then(
          response => {
            context.commit("setAccount", response.data);
          },
          () => {
            context.commit("removeAccount");
          }
        );
      }
    },
    logout(context) {
      delete axios.defaults.headers.common["Authorization"];
      context.commit("removeToken");
      context.commit("removeAccount");
    }
  },
  mutations: {
    setToken(state, token) {
      state.token = token;
      localStorage.setItem("AuthStore", JSON.stringify(state));
    },
    removeToken(state) {
      state.token = undefined;
      localStorage.removeItem("AuthStore");
    },
    setAccount(state, account) {
      state.account = account;
    },
    removeAccount(state) {
      state.account = null;
    }
  }
};
export default Auth;
