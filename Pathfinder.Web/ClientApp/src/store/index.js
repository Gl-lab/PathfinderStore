import Vue from "vue";
import Vuex from "vuex";
import Auth from "./auth";
import Character from "@/store/character";

Vue.use(Vuex);

export default new Vuex.Store({
  namespaced: true,
  modules: {
    auth: Auth,
    character: Character
  }
});
