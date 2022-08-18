import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgModule } from '@angular/core';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { NotAuthorizedInterceptor } from './core/interceptors/not-authorized.interceptor';
import { LoadingScreenInterceptor } from './core/interceptors/loading.interceptor';
import { AuthCookieInterceptor } from './core/interceptors/auth-cookie.interceptor';
import { DxSelectBoxModule } from 'devextreme-angular';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RegistrationModule } from './registration/registration.module';
import { CoreModule } from './core/core.module';
import { AppointmentsModule } from './appointments/appointments.module';
import { PatientHistoryModule } from './patient-history/patient-history.module';


@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    AppointmentsModule,
    CoreModule,
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    DxSelectBoxModule,
    BrowserAnimationsModule,
    RegistrationModule,
    PatientHistoryModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: NotAuthorizedInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingScreenInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthCookieInterceptor,
      multi: true,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
