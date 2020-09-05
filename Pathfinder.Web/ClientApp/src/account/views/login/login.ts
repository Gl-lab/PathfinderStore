import { Vue, Component } from 'vue-property-decorator';
import { State, Action, Getter } from 'vuex-class';

const namespace: string = 'AuthStore';
@Component
export default class LoginComponent extends Vue {
  public refs = this.$refs as any;
  public loginInput = {} as ILoginInput;
  public errors: INameValueDto[] = [];
  public isHaveError: boolean = false;

  @Action('login', { namespace })
  public login: any;

  public onSubmit() {
    if (this.refs.form.validate()) {
      this.login(this.loginInput);
      // this.$http.post('/api/login', this.loginInput)
      //   .then((response) => {
      //     localStorage.setItem('token', response.data.token);
      //     this.$router.push('/');
      //     this.isHaveError = false;
      //   }).catch((errors) => {
      //     this.errors = errors.response.data;
      //     this.isHaveError = true;
      //   });
    }
  }
  protected requiredError = (v: any) => !!v || 'RequiredField';
}
