import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from 'src/app/core/services/config.service';
import { MedicationHistoryModel } from '../models/medication-history.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';

@Injectable()
export class MedicationHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<MedicationHistoryModel[]>(`${this.config.apiUrl}medicationhistory/patient/${patientId}`)
            .toPromise();
    }

    isHistoryExist(patientId: string) {
        return this.http.get<boolean>(`${this.config.apiUrl}medicationhistory/historyexistence/patient/${patientId}`)
            .toPromise();
    }

    save(medicationHistory: MedicationHistoryModel) {
        medicationHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(medicationHistory.createDate);

        return this.http.post<void>(`${this.config.apiUrl}medicationhistory`, medicationHistory)
            .toPromise();
    }

    delete(medicationHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}medicationhistory/${medicationHistoryId}`)
            .toPromise();
    }

    getById(medicationHistoryId: any) {
        return this.http.get<MedicationHistoryModel>(`${this.config.apiUrl}medicationhistory/${medicationHistoryId}`)
            .toPromise()
            .then(medicationHistory => {
                if (medicationHistory) {
                    medicationHistory.createDate =
                        DateHelper.sqlServerUtcDateToLocalJsDate(medicationHistory.createDate);
                }

                return medicationHistory;
            });
    }
}