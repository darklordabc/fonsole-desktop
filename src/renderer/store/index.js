import Vue from 'vue';
import Vuex from 'vuex';
import * as actions from './actions';
import * as getters from './getters';
import gui from './modules/gui';
import networking from './modules/networking';
import gameLibrary from './modules/gameLibrary';

Vue.use(Vuex);

export default new Vuex.Store({
  actions,
  getters,
  modules: {
    gui,
    networking,
    gameLibrary,
  },
});