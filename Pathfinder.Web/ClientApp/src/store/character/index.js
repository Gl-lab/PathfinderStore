import axios from "axios";
import {appConst} from "@/settings";
import Wallet from "@/models/Wallet";

const Character = {
  namespaced: true,
  state: {
    character: null,
    loading: false
  },
  getters: {
    character: state => {
      return state.character;
    }
  },
  actions: {
    loadCharacter(context) {
      if (!context.getters.loading) {
        context.commit("startLoading");
        return axios
            .get(appConst.webApiUrl + "/api/Character")
            .then(response => {
              context.commit("setCharacter", response.data);
            })
            .finally(() => context.commit("stopLoading"));
      }
    }
  },
  mutations: {
    setCharacter(state, character) {
      character.wallet = new Wallet(character.backpack.wallet.balance);
      state.character = character;
    },
    startLoading(state) {
      state.loading = true;
    },
    stopLoading(state) {
      state.loading = true;
    }
  }
};
export default Character;
