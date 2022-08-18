import { Routes, RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { RootLoginComponent } from './registration/components/root-login/root-login.component';
import { ForgotPasswordComponent } from './registration/components/forgot-password/forgot-password.component';
import { ChangePasswordComponent } from './registration/components/changepassword/changepassword.component';
import { ConfirmEmailComponent } from "./registration/components/confirm-email/confirm-email.component";
import { ResetPasswordComponent } from "./registration/components/reset-password/reset-password.component";
import { AppointmentsComponent } from './appointments/appointments.component';
import { AuthGuard } from './core/guards/auth.guard';
import { PatientHistoryComponent } from './patient-history/patient-history.component';
import { AppRouteNames } from './core/constants/app-route-names';

const routes: Routes = [
  {
    path: "",
    redirectTo: AppRouteNames.appointments,
    pathMatch: "full"
  },
  {
    path: AppRouteNames.appointments,
    component: AppointmentsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: AppRouteNames.patientHistory,
    component: PatientHistoryComponent,
    canActivate: [AuthGuard]
  },
  {
    path: AppRouteNames.login,
    component: RootLoginComponent
  },
  /* {
    path: AppRouteNames.confirmEmail,
    component: ConfirmEmailComponent
  },
  {
    path: AppRouteNames.resetPassword,
    component: ResetPasswordComponent
  },
  {
    path: AppRouteNames.changePassword,
    component: ChangePasswordComponent
  },
  {
    path: AppRouteNames.forgotPassword,
    component: ForgotPasswordComponent
  } */
];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
