import { Injectable } from "@angular/core";
import { ConfigService } from './config.service';
import { HttpClient } from '@angular/common/http';
import { IcdCodeModel } from '../models/icd-code.model';

@Injectable({ providedIn: 'root' })
export class IcdCodeService {
    constructor(private http: HttpClient,
        private config: ConfigService) { }

    getById(id: string): Promise<IcdCodeModel> {
        return this.http.get<IcdCodeModel>(`${this.config.apiUrl}icdcode/${id}`)
            .toPromise();
    }
}