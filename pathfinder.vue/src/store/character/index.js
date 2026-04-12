import axios from "axios";
import { appConst } from "@/settings";
import Wallet from "@/models/Wallet";

function mapCharacter(character) {
  if (!character) {
    return null;
  }

  const backpack = character.backpack || null;
  const walletBalance =
    backpack && backpack.wallet ? backpack.wallet.balance : 0;

  return {
    ...character,
    balance: walletBalance,
    wallet: new Wallet(walletBalance)
  };
}

const Character = {
  namespaced: true,
  state: {
    characters: [],
    character: null,
    loading: false
  },
  getters: {
    characters: state => {
      return state.characters;
    },
    character: state => {
      return state.character;
    },
    loading: state => {
      return state.loading;
    }
  },
  actions: {
    loadCharacters(context) {
      if (!context.getters.loading) {
        context.commit("startLoading");
        return axios
          .get(appConst.webApiUrl + "/api/Character")
          .then(response => {
            context.commit("setCharacters", response.data);
          })
          .finally(() => context.commit("stopLoading"));
      }
    },
    loadCharacter(context, characterId) {
      if (!context.getters.loading) {
        context.commit("clearCharacter");
        context.commit("startLoading");
        return axios
          .get(appConst.webApiUrl + "/api/Character/" + characterId)
          .then(response => {
            context.commit("setCharacter", response.data);
          })
          .catch(error => {
            context.commit("clearCharacter");
            throw error;
          })
          .finally(() => context.commit("stopLoading"));
      }
    },
    deleteCharacter(context, characterId) {
      context.commit("startLoading");
      return axios
        .delete(appConst.webApiUrl + "/api/Character/" + characterId)
        .then(() => {
          context.commit("removeCharacter", characterId);
        })
        .finally(() => context.commit("stopLoading"));
    }
  },
  mutations: {
    setCharacters(state, characters) {
      state.characters = Array.isArray(characters)
        ? characters.map(mapCharacter)
        : [];
    },
    setCharacter(state, character) {
      state.character = mapCharacter(character);
    },
    clearCharacter(state) {
      state.character = null;
    },
    removeCharacter(state, characterId) {
      state.characters = state.characters.filter(
        character => character.id !== characterId
      );

      if (state.character && state.character.id === characterId) {
        state.character = null;
      }
    },
    startLoading(state) {
      state.loading = true;
    },
    stopLoading(state) {
      state.loading = false;
    }
  }
};
export default Character;
