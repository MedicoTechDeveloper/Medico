import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MedicalHistoryModel } from '../models/medical-history.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';
import { ConfigService } from 'src/app/core/services/config.service';

@Injectable()
export class MedicalHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<MedicalHistoryModel[]>(`${this.config.apiUrl}medicalhistory/patient/${patientId}`)
            .toPromise();
    }

    isHistoryExist(patientId: string) {
        return this.http.get<boolean>(`${this.config.apiUrl}medicalhistory/historyexistence/patient/${patientId}`)
            .toPromise();
    }

    save(medicalHistory: MedicalHistoryModel) {
        medicalHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(medicalHistory.createDate);

        return this.http.post<void>(`${this.config.apiUrl}medicalhistory`, medicalHistory)
            .toPromise();
    }

    delete(medicalHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}medicalhistory/${medicalHistoryId}`)
            .toPromise();
    }

    getById(medicalHistoryId: any) {
        return this.http.get<MedicalHistoryModel>(`${this.config.apiUrl}medicalhistory/${medicalHistoryId}`)
            .toPromise()
            .then(medicalHistory => {
                if (medicalHistory) {
                    medicalHistory.createDate = DateHelper.sqlServerUtcDateToLocalJsDate(medicalHistory.createDate);
                }

                return medicalHistory;
            });
    }
}