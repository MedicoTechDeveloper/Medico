import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from 'src/app/core/services/config.service';
import { TobaccoHistoryModel } from '../models/tobacco-history.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';

@Injectable()
export class TobaccoHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getLastCreated(patientId: string): Promise<TobaccoHistoryModel> {
        return this.http.get<TobaccoHistoryModel>(`${this.config.apiUrl}tobaccohistory/last/patient/${patientId}`)
            .toPromise();
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<TobaccoHistoryModel[]>(`${this.config.apiUrl}tobaccohistory/patient/${patientId}`)
            .toPromise();
    }

    save(tobaccoHistory: TobaccoHistoryModel) {
        tobaccoHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(tobaccoHistory.createDate);

        return this.http.post<void>(`${this.config.apiUrl}tobaccohistory`, tobaccoHistory)
            .toPromise();
    }

    delete(tobaccoHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}tobaccohistory/${tobaccoHistoryId}`)
            .toPromise();
    }

    getById(tobaccoHistoryId: any) {
        return this.http.get<TobaccoHistoryModel>(`${this.config.apiUrl}tobaccohistory/${tobaccoHistoryId}`)
            .toPromise()
            .then(tobaccoHistory => {
                if (tobaccoHistory) {
                    tobaccoHistory.createDate =
                        DateHelper.sqlServerUtcDateToLocalJsDate(tobaccoHistory.createDate);
                }

                return tobaccoHistory;
            });
    }
}