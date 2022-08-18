import { Component, ViewChild } from '@angular/core';
import { DxFormComponent } from 'devextreme-angular';
import { AlertService } from 'src/app/core/services/alert.service';
import { UserService } from 'src/app/core/services/user.service';
import { Router } from '@angular/router';
import { UserIdentificationInfoModel } from 'src/app/core/models/user-identification-info.model';
import { LookupModel } from 'src/app/core/models/lookup.model';
import { AppRouteNames } from 'src/app/core/constants/app-route-names';


@Component({
  selector: 'root-login',
  templateUrl: './root-login.component.html'
})

export class RootLoginComponent {
  @ViewChild("patientIdentificationInfoForm", { static: false }) patientIdentificationInfoForm: DxFormComponent;

  userIdentificationInfo: UserIdentificationInfoModel =
    new UserIdentificationInfoModel();

  patientCompanies: LookupModel[] = [];

  constructor(private alertService: AlertService,
    private userService: UserService, private router: Router) {
  }

  get isPatientAbleToLogin(): boolean {
    return !!this.patientCompanies.length;
  }

  resetPatientCompanies(): void {
    this.patientCompanies = [];
  }

  findUsers(): void {
    const isPatientIdentificationInfoFormValid = this.patientIdentificationInfoForm
      .instance.validate()
      .isValid;

    if (!isPatientIdentificationInfoFormValid)
      return;

    this.userService.getUserCompanies(this.userIdentificationInfo)
      .then(companies => {
        if (companies.length)
          this.patientCompanies = companies;
        else
          this.alertService.warning("Unable to find patient. Please, check entered information");
      });

  }

  forgotPassword(): void {
    this.router.navigate([AppRouteNames.forgotPassword])
  }
}
