import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { DateHelper } from 'src/app/core/helpers/date.helper';
import { LookupModel } from '../models/lookup.model';
import { UserIdentificationInfoModel } from '../models/user-identification-info.model';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient,
        private config: ConfigService) {
    }

    getUserCompanies(userIdentificationInfo: UserIdentificationInfoModel): Promise<LookupModel[]> {
        userIdentificationInfo.dateOfBirth =
            DateHelper.jsLocalDateToSqlServerUtc(userIdentificationInfo.dateOfBirth);

        const queryParams = new HttpParams({
            fromObject: userIdentificationInfo.toQueryParams()
        });

        return this.http.get<LookupModel[]>(`${this.config.apiUrl}user/companies`, { params: queryParams })
            .toPromise();
    }
}
