import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { PatientChartNodeType } from '../enums/patient-chart-node-types.enum';
import { DefaultValueModel } from '../models/default-value.model';

@Injectable({ providedIn: 'root' })
export class DefaultValueService {
    constructor(private http: HttpClient,
        private config: ConfigService) {
    }

    getByPatientChartNodeType(patientChartNodeType: PatientChartNodeType) {
        return this.http.get<DefaultValueModel>(`${this.config.apiUrl}defaultvalue/key/${patientChartNodeType}`)
            .toPromise();
    }
}