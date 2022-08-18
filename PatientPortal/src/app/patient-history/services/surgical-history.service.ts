import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from 'src/app/core/services/config.service';
import { SurgicalHistoryModel } from '../models/surgical-history.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';

@Injectable()
export class SurgicalHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<SurgicalHistoryModel[]>(`${this.config.apiUrl}surgicalhistory/patient/${patientId}`)
            .toPromise();
    }

    isHistoryExist(patientId: string) {
        return this.http.get<boolean>(`${this.config.apiUrl}surgicalhistory/historyexistence/patient/${patientId}`)
            .toPromise();
    }

    save(surgicalHistory: SurgicalHistoryModel) {
        return this.http.post<void>(`${this.config.apiUrl}surgicalhistory`, surgicalHistory)
            .toPromise();
    }

    delete(surgicalHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}surgicalhistory/${surgicalHistoryId}`)
            .toPromise();
    }

    getById(surgicalHistoryId: any) {
        return this.http.get<SurgicalHistoryModel>(`${this.config.apiUrl}surgicalhistory/${surgicalHistoryId}`)
            .toPromise()
            .then(surgicalHistory => {
                if (surgicalHistory) {
                    surgicalHistory.createDate =
                        DateHelper.sqlServerUtcDateToLocalJsDate(surgicalHistory.createDate);
                }

                return surgicalHistory;
            });
    }
}