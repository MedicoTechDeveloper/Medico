import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { ConfigService } from './config.service';
import { PatientAppointmentModel } from '../models/patient-appointment.model';
import { AuthenticationService } from './authentication.service';
import { DateHelper } from '../helpers/date.helper';
import { AppointmentReportModel } from '../models/appointment-report.model';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
    constructor(private http: HttpClient,
        private config: ConfigService,
        private authenticationService: AuthenticationService) {
    }

    getAppointmentHtmlReport(appointmentId: string): Promise<string> {
        const utcOffset = DateHelper.getUtcOffset();
        return this.http.get<AppointmentReportModel>(`${this.config.apiUrl}report/appointment/${appointmentId}/offset/${utcOffset}/view`)
            .toPromise()
            .then(appointmentReportModel => {
                return appointmentReportModel.reportContent;
            });
    }

    getAppointmentPdfReport(appointmentId: string): Promise<HttpResponse<Blob>> {
        const utcOffset = DateHelper.getUtcOffset();
        return this.http.get(`${this.config.apiUrl}report/appointment/${appointmentId}/offset/${utcOffset}/pdf`,
            { observe: 'response', responseType: 'blob' })
            .toPromise()
    }

    getByPatientAppointments(): Promise<PatientAppointmentModel[]> {
        const patientUser = this.authenticationService.currentUserValue;

        return this.http.get<PatientAppointmentModel[]>(`${this.config.apiUrl}appointment/patient/${patientUser.patientId}/company/${patientUser.companyId}`)
            .toPromise()
            .then(appointments => {
                for (let i = 0; i < appointments.length; i++) {
                    const appointment = appointments[i];

                    appointment.startDate =
                        DateHelper.sqlServerUtcDateToLocalJsDate(appointment.startDate);

                    appointment.endDate =
                        DateHelper.sqlServerUtcDateToLocalJsDate(appointment.endDate);
                }

                return appointments;
            });
    }
}