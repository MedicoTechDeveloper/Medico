import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { PatientUserModel } from '../models/patient-user.model';
import { ConfigService } from './config.service';
import { PatientLoginModel } from '../models/patient-login.model';
import { PatientLoginResponseModel } from '../models/patient-login-response.model';
import { ValidationResultModel } from '../models/validation-result.model';


@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private userLocalStorageName: string = "PatientPortal.CurrentUser";

    private currentUserSubject: BehaviorSubject<PatientUserModel>;

    currentUser: Observable<PatientUserModel>;

    constructor(private http: HttpClient,
        private config: ConfigService) {
        const localStorageUser = JSON.parse(localStorage.getItem(this.userLocalStorageName)) as PatientUserModel;

        const patientUser = localStorageUser
            ? localStorageUser
            : new PatientUserModel();

        this.currentUserSubject = new BehaviorSubject<PatientUserModel>(patientUser);
        this.currentUser = this.currentUserSubject.asObservable();
    }

    get currentUserValue(): PatientUserModel {
        return this.currentUserSubject.value;
    }

    login(patientLoginModel: PatientLoginModel): Promise<string[]> {
        console.log(patientLoginModel);
        return this.http.post<PatientLoginResponseModel>(`${this.config.apiUrl}account/patient/login`, patientLoginModel)
            .toPromise()
            .then(patientLoginResponse => {
                const loginErrors = patientLoginResponse.errors;
                const doLoginErrorsExist = loginErrors && loginErrors.length;

                if (doLoginErrorsExist)
                    return loginErrors;

                const patientUser = patientLoginResponse.patientUser;
                localStorage.setItem(this.userLocalStorageName, JSON.stringify(patientUser));

                this.currentUserSubject.next(patientUser);

                return [];
            });
    }

    confirmEmail(userId: string, code: string): Promise<ValidationResultModel> {
        return this.http.get<ValidationResultModel>(`${this.config.apiUrl}account/confirm-email?userId=${userId}&code=${code}`)
            .toPromise();
    }

    logout() {
        this.clearPreviouslySavedUser();
        this.currentUserSubject.next(null);

        return this.http.post(`${this.config.apiUrl}account/logout`, {})
            .toPromise();
    }

    clearPreviouslySavedUser() {
        localStorage.removeItem(this.userLocalStorageName);
    }
}
