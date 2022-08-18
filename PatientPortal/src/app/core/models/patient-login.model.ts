import { UserIdentificationInfoModel } from './user-identification-info.model';

export class PatientLoginModel extends UserIdentificationInfoModel {
    companyId: string;
    password: string;

    constructor() {
        super();    
    }
}