import { NgModule, Optional, SkipSelf } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { ProductDataService } from './services/product-data.service';
import { CategoryDataService } from './services/category-data.service';
import { ValidationService } from './services/validation.service';
import { EnsureModuleLoadedOnceGuard } from './ensure-module-loaded-once.guard';
import { HttpErrorInterceptor } from './interceptors/http-error.interceptor';
import { HttpAuthInterceptor } from './interceptors/http-auth.interceptor';
import { NgxUiLoaderModule, NgxUiLoaderHttpModule } from 'ngx-ui-loader';
import { SpinnerService } from './services/spinner.service';
import { AuthService } from './services/auth.service';
import { AuthGuardService } from './services/auth-guard.service';
import { LayoutComponent } from './layout/layout.component';
import { AppAsideModule, AppBreadcrumbModule, AppHeaderModule, AppFooterModule, AppSidebarModule } from '@coreui/angular';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { RouterModule } from '@angular/router';
import { CustomerDataService } from './services/customer-data.services';
import { PageService } from './services/page.service';
import { ToastrModule } from 'ngx-toastr';
import { NgWizardModule, NgWizardConfig, THEME } from 'ng-wizard';

const APP_CONTAINERS = [LayoutComponent];

const ngWizardConfig: NgWizardConfig = {
  theme: THEME.default
};

@NgModule({
  declarations: [
    ...APP_CONTAINERS,
  ],
  imports: [
    RouterModule,
    NgxUiLoaderModule,
    //NgxUiLoaderRouterModule, // import this module for showing loader automatically when navigating between app routes
    NgxUiLoaderHttpModule.forRoot({ showForeground: false }),
    ToastrModule.forRoot({ positionClass: 'toast-top-full-width', closeButton: true }),
    AppAsideModule,
    AppBreadcrumbModule.forRoot(),
    AppFooterModule,
    AppHeaderModule,
    AppSidebarModule,
    PerfectScrollbarModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot(),
    NgWizardModule.forRoot(ngWizardConfig),
  ],
  exports: [
    RouterModule,
    HttpClientModule,
    NgxUiLoaderModule,
    NgWizardModule,
    LayoutComponent,
  ],
  providers: [
    ProductDataService,
    CategoryDataService,
    CustomerDataService,
    AuthService,
    AuthGuardService,
    ValidationService,
    PageService,
    SpinnerService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpAuthInterceptor,
      multi: true
    },
  ] // these should be singleton
})
export class CoreModule extends EnsureModuleLoadedOnceGuard {    // Ensure that CoreModule is only loaded into AppModule

  // Looks for the module in the parent injector to see if it's already been loaded (only want it loaded once)
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    super(parentModule);
  }
}
