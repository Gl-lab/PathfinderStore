import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import { State, Action, Getter } from 'vuex-class';

const namespace: string = 'AuthStore';


@Component
export default class ComponentBase extends Vue {

  // @Getter('isSignedIn', { namespace })
  // public isSignedIn: boolean | undefined;

  @Action('logout', { namespace })
  public logout: any;



  public created(): void {
    this.$http.interceptors.response.use((responce) => (responce), ((error) => {
      if (error.response.status === 401) {
        this.logout();
      }
    }));
  }
}
