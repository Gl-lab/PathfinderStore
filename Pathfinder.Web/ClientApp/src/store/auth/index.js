import axios from 'axios'
import { appConst } from '../../settings'

const Auth = {
  namespaced: true,
  state: {
    token: null
  },
  getters: {
    isAuthorized: state => {
      if (!state.token){
        const temp = localStorage.getItem('AuthStore');
        if (temp) {
          state = JSON.parse(temp);
        }
      }
      return !!state.token;
    },
    getToken: state => {
      if (!state.token){
        const temp = localStorage.getItem('AuthStore');
        if (temp) {
          state = JSON.parse(temp);
        }
      }
      return state.token;
    },
  },
  actions: {
    login (context, user) {
      axios.post(
            appConst.webApiUrl + '/api/login',
            user,
          )
          .then((response) => {
            const token = response.data.token;
            axios.defaults.headers.common['Authorization'] = 'Bearer '+token;
            context.commit('setToken', token);
          }, () => {
            context.commit('removeToken');
          });
    },
    logout(context) {
      delete axios.defaults.headers.common['Authorization'];
      context.commit('removeToken');
    },
  },
  mutations: {
    setToken(state, token) {
      state.token = token;
      localStorage.setItem('AuthStore', JSON.stringify(state));
    },
    removeToken(state) {
      state.token = undefined;
      localStorage.removeItem('AuthStore');
    },
  }
}
export default Auth
