import { Component } from 'vue-property-decorator';
import NucleusComponentBase from '@/shared/application/nucleus-component-base';

@Component
export default class ResetPasswordComponent extends NucleusComponentBase {
  public refs = this.$refs as any;
  public resetPasswordInput = {} as IResetPasswordInput;
  public errors: INameValueDto[] = [];
  public isPasswordReset = false;
  public resultMessage: string | undefined;

  public onSubmit() {
    if (this.refs.form.validate()) {
      this.resetPasswordInput.token = this.$route.query.token.toString();
      this.nucleusService.post<IResetPasswordOutput>('/api/resetPassword', this.resetPasswordInput)
                .then((response) => {
                  if (!response.isError) {
                    this.resultMessage = 'PasswordResetSuccessful';
                    this.isPasswordReset = true;
                  } else {
                    this.errors = response.errors;
                  }
                });
    }
  }
}
