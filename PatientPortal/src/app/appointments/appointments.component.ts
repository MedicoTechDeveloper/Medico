import { Component } from '@angular/core';
import { PatientAppointmentModel } from '../core/models/patient-appointment.model';
import { AppointmentService } from '../core/services/appointments.service';

@Component({
    selector: "appointments",
    templateUrl: "appointments.component.html"
})
export class AppointmentsComponent {
    appointmentsTabs = [];
    selectedTabIndex: number = 0;

    previousPatientAppointments: PatientAppointmentModel[] = [];
    upcomingPatientAppointments: PatientAppointmentModel[] = [];

    constructor(private appointmentsService: AppointmentService) {
        this.initAppointmentsTabs();
        this.initPatientAppointments();
    }

    onTabSelect($event) {
        if (this.selectedTabIndex !== $event.itemIndex)
            this.selectedTabIndex = $event.itemIndex
    }

    isTabVisible(tabId: number) {
        return this.selectedTabIndex === tabId;
    }

    private initPatientAppointments() {
        this.appointmentsService.getByPatientAppointments()
            .then(patientAppointments => {
                const currentDate = new Date();

                this.previousPatientAppointments =
                    patientAppointments.filter(a => a.startDate < currentDate);

                this.upcomingPatientAppointments =
                    patientAppointments.filter(a => a.startDate > currentDate);
            });
    }

    private initAppointmentsTabs() {
        this.appointmentsTabs = [
            {
                id: 0,
                text: "Upcoming"
            },
            {
                id: 1,
                text: "Previous"
            }
        ];
    }
}