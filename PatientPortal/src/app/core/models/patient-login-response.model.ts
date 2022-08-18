import { PatientUserModel } from './patient-user.model';

export class PatientLoginResponseModel {
    patientUser: PatientUserModel;
    errors: string[];
}