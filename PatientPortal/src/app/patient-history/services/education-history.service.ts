import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from 'src/app/core/services/config.service';
import { EducationHistoryModel } from '../models/education-history.model';
import { DateHelper } from 'src/app/core/helpers/date.helper';

@Injectable()
export class EducationHistoryService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getAllByPatientId(patientId: string) {
        return this.http.get<EducationHistoryModel[]>(`${this.config.apiUrl}educationhistory/patient/${patientId}`)
            .toPromise();
    }

    isHistoryExist(patientId: string) {
        return this.http.get<boolean>(`${this.config.apiUrl}educationhistory/historyexistence/patient/${patientId}`)
            .toPromise();
    }

    save(educationHistory: EducationHistoryModel) {
        educationHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(educationHistory.createDate);

        return this.http.post<void>(`${this.config.apiUrl}educationhistory`, educationHistory)
            .toPromise();
    }

    delete(educationHistoryId: string) {
        return this.http.delete<void>(`${this.config.apiUrl}educationhistory/${educationHistoryId}`)
            .toPromise();
    }

    getById(educationHistoryId: any) {
        return this.http.get<EducationHistoryModel>(`${this.config.apiUrl}educationhistory/${educationHistoryId}`)
            .toPromise()
            .then(educationHistory => {
                if (educationHistory) {
                    educationHistory.createDate =
                        DateHelper.sqlServerUtcDateToLocalJsDate(educationHistory.createDate);
                }

                return educationHistory;
            });
    }
}