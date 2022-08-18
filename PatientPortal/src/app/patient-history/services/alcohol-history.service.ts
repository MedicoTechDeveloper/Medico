import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { DateHelper } from 'src/app/core/helpers/date.helper';
import { ConfigService } from 'src/app/core/services/config.service';
import { AlcoholHistoryModel } from '../models/alcohol-history.model';

@Injectable()
export class AlcoholHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {

    }

    getAllByPatientId(patientId: string) {
        return this.http.get<AlcoholHistoryModel[]>(`${this.config.apiUrl}alcoholhistory/patient/${patientId}`)
            .toPromise();
    }

    getLastCreated(patientId: string): Promise<AlcoholHistoryModel> {
        return this.http.get<AlcoholHistoryModel>(`${this.config.apiUrl}alcoholhistory/last/patient/${patientId}`)
            .toPromise();
    }

    save(alcoholHistory: AlcoholHistoryModel) {
        alcoholHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(alcoholHistory.createDate);

        return this.http.post<void>(`${this.config.apiUrl}alcoholhistory`, alcoholHistory)
            .toPromise();
    }

    delete(alcoholHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}alcoholhistory/${alcoholHistoryId}`)
            .toPromise();
    }

    getById(alcoholHistoryId: any) {
        return this.http.get<AlcoholHistoryModel>(`${this.config.apiUrl}alcoholhistory/${alcoholHistoryId}`)
            .toPromise()
            .then(alcoholHistory => {
                if (alcoholHistory) {
                    alcoholHistory.createDate = DateHelper.sqlServerUtcDateToLocalJsDate(alcoholHistory.createDate);
                }

                return alcoholHistory;
            });
    }
}