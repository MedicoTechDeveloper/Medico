import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from 'src/app/core/services/config.service';
import { OccupationalHistoryModel } from '../models/occupational-history.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';

@Injectable()
export class OccupationalHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<OccupationalHistoryModel[]>(`${this.config.apiUrl}occupationalhistory/patient/${patientId}`)
            .toPromise();
    }

    isHistoryExist(patientId: string) {
        return this.http.get<boolean>(`${this.config.apiUrl}occupationalhistory/historyexistence/patient/${patientId}`)
            .toPromise();
    }

    save(occupationalHistory: OccupationalHistoryModel) {
        occupationalHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(occupationalHistory.createDate);

        occupationalHistory.start = DateHelper
            .jsLocalDateToSqlServerUtc(occupationalHistory.start);

        if (occupationalHistory.end)
            occupationalHistory.end = DateHelper
                .jsLocalDateToSqlServerUtc(occupationalHistory.end);

        return this.http.post<void>(`${this.config.apiUrl}occupationalhistory`, occupationalHistory)
            .toPromise();
    }

    delete(occupationalHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}occupationalhistory/${occupationalHistoryId}`)
            .toPromise();
    }

    getById(occupationalHistoryId: any) {
        return this.http.get<OccupationalHistoryModel>(`${this.config.apiUrl}occupationalhistory/${occupationalHistoryId}`)
            .toPromise()
            .then(occupationalHistory => {
                if (occupationalHistory) {
                    occupationalHistory.createDate
                        = DateHelper.sqlServerUtcDateToLocalJsDate(occupationalHistory.createDate);

                    occupationalHistory.start
                        = DateHelper.sqlServerUtcDateToLocalJsDate(occupationalHistory.start);

                    if (occupationalHistory.end)
                        occupationalHistory.end
                            = DateHelper.sqlServerUtcDateToLocalJsDate(occupationalHistory.end);
                }

                return occupationalHistory;
            });
    }
}