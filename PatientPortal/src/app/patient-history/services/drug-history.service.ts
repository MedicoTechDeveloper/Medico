import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from 'src/app/core/services/config.service';
import { DrugHistoryModel } from '../models/drug-history.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';

@Injectable()
export class DrugHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<DrugHistoryModel[]>(`${this.config.apiUrl}drughistory/patient/${patientId}`)
            .toPromise();
    }

    getLastCreated(patientId: string): Promise<DrugHistoryModel> {
        return this.http.get<DrugHistoryModel>(`${this.config.apiUrl}drughistory/last/patient/${patientId}`)
            .toPromise();
    }

    save(drugHistory: DrugHistoryModel) {
        drugHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(drugHistory.createDate);

        return this.http.post<void>(`${this.config.apiUrl}drughistory`, drugHistory)
            .toPromise();
    }

    delete(drugHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}drughistory/${drugHistoryId}`)
            .toPromise();
    }

    getById(drugHistoryId: any) {
        return this.http.get<DrugHistoryModel>(`${this.config.apiUrl}drughistory/${drugHistoryId}`)
            .toPromise()
            .then(drugHistory => {
                if (drugHistory) {
                    drugHistory.createDate = DateHelper.sqlServerUtcDateToLocalJsDate(drugHistory.createDate);
                }

                return drugHistory;
            });
    }
}