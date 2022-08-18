import { MedicalHistoryModel } from './medical-history.model';

export class FamilyHistoryModel extends MedicalHistoryModel {
    familyMember: string;
    familyStatus: string;
}