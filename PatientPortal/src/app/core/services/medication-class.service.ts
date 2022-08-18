import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ConfigService } from './config.service';
import { LookupModel } from '../models/lookup.model';

@Injectable({ providedIn: "root" })
export class MedicationClassService {
    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getById(medicationClassId: string): Promise<LookupModel> {
        return this.http.get<LookupModel>(`${this.config.apiUrl}medicationclass/${medicationClassId}`)
            .toPromise();
    }
}