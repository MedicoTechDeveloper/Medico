import { Component, OnInit, ViewChild, Input, EventEmitter, Output } from "@angular/core";
import { DxFormComponent } from "devextreme-angular";
import { Router, ActivatedRoute } from "@angular/router";
import { AlertService } from "src/app/core/services/alert.service";
import { LookupModel } from 'src/app/core/models/lookup.model';
import { UserIdentificationInfoModel } from 'src/app/core/models/user-identification-info.model';
import { PatientLoginModel } from 'src/app/core/models/patient-login.model';
import { AuthenticationService } from 'src/app/core/services/authentication.service';

@Component({
    selector: "login",
    templateUrl: "./login.component.html"
})
export class LoginComponent implements OnInit {
    @Input() companies: LookupModel[];
    @Input() userIdentificationInfo: UserIdentificationInfoModel;

    @Output() onBackToEmailForm: EventEmitter<void> = new EventEmitter();

    @ViewChild("patientLoginForm", { static: false }) patientLoginForm: DxFormComponent;

    patientLoginModel: PatientLoginModel = new PatientLoginModel();

    loginErrors: string[] = [];

    returnUrl: string;

    constructor(private router: Router,
        private route: ActivatedRoute,
        private authenticationService: AuthenticationService,
        private alertService: AlertService) {
    }

    ngOnInit() {
        this.initPatientLoginModel();
        this.returnUrl = this.route.snapshot.queryParams["returnUrl"] || "/";
    }

    backToEmailForm(): void {
        this.onBackToEmailForm.next();
    }

    login(): void {
        const isLoginFormValid = this.patientLoginForm
            .instance.validate()
            .isValid;

        if (!isLoginFormValid)
            return;

        this.loginErrors = [];

        this.authenticationService.login(this.patientLoginModel)
            .then(loginErrors => {
                if (loginErrors.length) {
                    this.loginErrors = loginErrors;
                }
                else {
                    this.router.navigate([this.returnUrl]);
                }
            }).catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private initPatientLoginModel(): void {
        this.patientLoginModel.companyId =
            this.companies[0].id;

        this.patientLoginModel.dateOfBirth =
            this.userIdentificationInfo.dateOfBirth;

        this.patientLoginModel.firstName =
            this.userIdentificationInfo.firstName;

        this.patientLoginModel.lastName =
            this.userIdentificationInfo.lastName;
    }
}
