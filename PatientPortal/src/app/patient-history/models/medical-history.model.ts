export class MedicalHistoryModel {
    id: string;
    notes: string;
    patientId: string;
    diagnosis: string;
    createDate: any;

    constructor() {
        this.createDate = new Date();
    }
}