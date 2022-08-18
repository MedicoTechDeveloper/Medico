import { Injectable } from "@angular/core";
import { ConfigService } from './config.service';
import { HttpClient } from '@angular/common/http';
import { LookupModel } from '../models/lookup.model';
import { MedicationItemInfoViewModel } from '../models/medication-item-info-view.model';

@Injectable({ providedIn: 'root' })
export class MedicationService {
    constructor(private http: HttpClient,
        private config: ConfigService) { }

    getMedicationInfo(medicationNameId: string): Promise<MedicationItemInfoViewModel> {
        return this.http.get<MedicationItemInfoViewModel>(`${this.config.apiUrl}medication/info/${medicationNameId}`)
            .toPromise();
    }

    getById(id: string): Promise<LookupModel> {
        return this.http.get<LookupModel>(`${this.config.apiUrl}medication/${id}`)
            .toPromise();
    }

    getNameByMedicationNameId(medicationNameId: string): Promise<LookupModel> {
        return this.http.get<LookupModel>(`${this.config.apiUrl}medication/name/${medicationNameId}`)
            .toPromise();
    }
}