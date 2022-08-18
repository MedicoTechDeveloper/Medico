import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from 'src/app/core/services/config.service';
import { AllergyOnMedicationModel } from '../models/allergy-on-medication.model';
import { AllergyModel } from '../models/allergy.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';

@Injectable()
export class AllergyService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getPatientAllergyOnMedication(patientId: string, medicationNameId: string): Promise<AllergyOnMedicationModel> {
        return this.http.get<AllergyOnMedicationModel>(`${this.config.apiUrl}allergy/patient/${patientId}/medication/${medicationNameId}`)
            .toPromise();
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<AllergyModel[]>(`${this.config.apiUrl}allergy/patient/${patientId}`)
            .toPromise();
    }

    isHistoryExist(patientId: string) {
        return this.http.get<boolean>(`${this.config.apiUrl}allergy/allergyexistence/patient/${patientId}`)
            .toPromise();
    }

    save(allergy: AllergyModel) {
        allergy.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(allergy.createDate);

        return this.http.post<void>(`${this.config.apiUrl}allergy`, allergy)
            .toPromise();
    }

    delete(allergyId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}allergy/${allergyId}`)
            .toPromise();
    }

    getByPatientIdAndDate(patientId: string, currentDate: any): Promise<AllergyModel[]> {
        var utcDate = DateHelper.jsLocalDateToSqlServerUtc(currentDate);
        return this.http.get<AllergyModel[]>(`${this.config.apiUrl}allergy/patient/${patientId}/date/${utcDate}`)
            .toPromise()
            .then(allergies => {
                if (allergies.length) {
                    allergies.forEach(allergy => {
                        allergy.createDate = DateHelper.sqlServerUtcDateToLocalJsDate(allergy.createDate);
                    });
                }

                return allergies;
            });
    }

    getById(allergyId: any) {
        return this.http.get<AllergyModel>(`${this.config.apiUrl}allergy/${allergyId}`)
            .toPromise()
            .then(allergy => {
                if (allergy) {
                    allergy.createDate = DateHelper.sqlServerUtcDateToLocalJsDate(allergy.createDate);
                }

                return allergy;
            });
    }
}