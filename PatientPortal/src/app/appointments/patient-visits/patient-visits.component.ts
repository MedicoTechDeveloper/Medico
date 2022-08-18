import { Component, Input } from '@angular/core';
import { PatientAppointmentModel } from 'src/app/core/models/patient-appointment.model';
import { AppointmentService } from 'src/app/core/services/appointments.service';
import { saveAs } from 'file-saver';

@Component({
    selector: "patient-visits",
    templateUrl: "patient-visits.component.html"
})
export class PatientVisitsComponent {
    @Input() appointments: PatientAppointmentModel[] = [];

    private _isReportPopupOpened: boolean = false;

    appointmentReportContent: string = "";

    get isReportPopupOpened() {
        return this._isReportPopupOpened;
    }

    set isReportPopupOpened(value: boolean) {
        this._isReportPopupOpened = value;
        if (!value)
            this.appointmentReportContent = "";
    }

    constructor(private appointmentService: AppointmentService) {
    }

    showAppointmentReport(appointmentId: string) {
        this.appointmentService.getAppointmentHtmlReport(appointmentId)
            .then(appointmentReportContent => {
                this.appointmentReportContent = appointmentReportContent;
                this.isReportPopupOpened = true;
            });
    }

    downloadAppointmentReport(appointmentId: string, startDate: string) {
        this.appointmentService.getAppointmentPdfReport(appointmentId)
            .then(response => {
                const blob = new Blob([response.body], { type: "application/pdf" });
                saveAs(blob, `${startDate}.pdf`);
            })
    }
}