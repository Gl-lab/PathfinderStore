import axios from "axios";

function getPersistedAuthState() {
  const rawState = localStorage.getItem("AuthStore");

  if (!rawState) {
    return null;
  }

  try {
    return JSON.parse(rawState);
  } catch (error) {
    return null;
  }
}

function getTokenFromState(state) {
  if (state && state.token) {
    return state.token;
  }

  const persistedState = getPersistedAuthState();

  return persistedState && persistedState.token ? persistedState.token : null;
}

function decodeTokenPayload(token) {
  if (!token) {
    return null;
  }

  const tokenParts = token.split(".");

  if (tokenParts.length < 2) {
    return null;
  }

  const base64 = tokenParts[1].replace(/-/g, "+").replace(/_/g, "/");
  const padding = "=".repeat((4 - (base64.length % 4)) % 4);

  try {
    return JSON.parse(atob(base64 + padding));
  } catch (error) {
    return null;
  }
}

function getUserNameFromToken(token) {
  const payload = decodeTokenPayload(token);

  if (!payload) {
    return null;
  }

  return typeof payload.sub === "string" ? payload.sub : null;
}

const Auth = {
  namespaced: true,
  state: {
    token: null
  },
  getters: {
    isAuthorized: state => {
      return !!getTokenFromState(state);
    },
    getToken: state => {
      return getTokenFromState(state);
    },
    getUserName: state => {
      const userName = getUserNameFromToken(getTokenFromState(state));

      if (userName) {
        return userName.toUpperCase()[0];
      }

      return null;
    }
  },
  actions: {
    logout(context) {
      delete axios.defaults.headers.common["Authorization"];
      context.commit("removeToken");
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
    }
  }
};
export default Auth;
