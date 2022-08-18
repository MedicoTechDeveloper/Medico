import { Injectable } from "@angular/core";
import { ConfigService } from './config.service';
import { HttpClient } from '@angular/common/http';
import { CptCodeModel } from '../models/cpt-code.model';

@Injectable({ providedIn: 'root' })
export class CptCodeService {
    constructor(private http: HttpClient,
        private config: ConfigService) { }

    getById(id: string): Promise<CptCodeModel> {
        return this.http.get<CptCodeModel>(`${this.config.apiUrl}cptcode/${id}`).toPromise();
    }
}