import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from 'src/app/core/services/config.service';
import { FamilyHistoryModel } from '../models/family-history.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';

@Injectable()
export class FamilyHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<FamilyHistoryModel[]>(`${this.config.apiUrl}familyhistory/patient/${patientId}`)
            .toPromise();
    }

    isHistoryExist(patientId: string) {
        return this.http.get<boolean>(`${this.config.apiUrl}familyhistory/historyexistence/patient/${patientId}`)
            .toPromise();
    }

    save(familyHistory: FamilyHistoryModel) {
        familyHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(familyHistory.createDate);

        return this.http.post<void>(`${this.config.apiUrl}familyhistory`, familyHistory)
            .toPromise();
    }

    delete(familyHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}familyhistory/${familyHistoryId}`)
            .toPromise();
    }

    getById(familyHistoryId: string) {
        return this.http.get<FamilyHistoryModel>(`${this.config.apiUrl}familyhistory/${familyHistoryId}`)
            .toPromise()
            .then(familyHistory => {
                if (familyHistory) {
                    familyHistory.createDate = DateHelper.sqlServerUtcDateToLocalJsDate(familyHistory.createDate);
                }

                return familyHistory;
            });
    }
}