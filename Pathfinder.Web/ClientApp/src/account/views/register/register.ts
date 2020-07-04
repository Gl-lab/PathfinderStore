import { Vue, Component } from 'vue-property-decorator';

@Component
export default class RegisterComponent extends Vue {
  public refs = this.$refs as any;
  public registerInput = {} as IRegisterInput;
  public errors: INameValueDto[] = [];
  public resultMessage: string | undefined;
  public registerComplete = false;
  public isHaveError = false;

  public onSubmit() {
    if (this.refs.form.validate()) {
      this.axios.post('/api/register', this.registerInput)
                .then(() => {
                    this.resultMessage = 'AccountCreationSuccessful';
                    this.registerComplete = true;
                }).catch((err) => {
                  this.errors = err.response.data;
                  this.isHaveError = true;
                });
    }
  }
  protected requiredError = (v: any) => !!v || 'RequiredField';
  protected passwordMatchError(password: string, passwordRepeat: string) {
    // tslint:disable-next-line: triple-equals
    return (password == passwordRepeat)
            ? ''
            : 'Несовпадение паролей';
  }
}
