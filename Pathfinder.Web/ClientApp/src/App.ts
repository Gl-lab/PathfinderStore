
import { Vue, Component } from 'vue-property-decorator';
import ComponentBase from './shared/application/component-base';
import { State, Action, Getter } from 'vuex-class';

const namespace: string = 'AuthStore';
@Component
export default class AppComponent extends ComponentBase {
    @Action('logout', { namespace })
    public logout: any;

}
