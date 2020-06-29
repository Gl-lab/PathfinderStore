import { Component } from 'vue-property-decorator';
import NucleusComponentBase from '@/shared/application/nucleus-component-base';

@Component
export default class ForgotPasswordComponent extends NucleusComponentBase {
  public refs = this.$refs as any;
  public forgotPasswordInput = {} as IForgotPasswordInput;
  public errors: INameValueDto[] = [];
  public isEmailSent = false;
  public resultMessage: string | undefined;

  public onSubmit() {
    if (this.refs.form.validate()) {
      this.nucleusService.post<IForgotPasswordOutput>('/api/forgotPassword', this.forgotPasswordInput)
                .then((response) => {
                  if (!response.isError) {
                    this.resultMessage = this.$t('EMailSentSuccessfully').toString();
                    this.isEmailSent = true;
                  } else {
                    this.errors = response.errors;
                  }
                });
    }
  }
}
