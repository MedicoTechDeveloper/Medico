import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { DxDataGridModule } from 'devextreme-angular/ui/data-grid'
import { DxTabsModule } from 'devextreme-angular/ui/tabs';
import { ShareModule } from '../share/share.module';
import { AppointmentsComponent } from './appointments.component';
import { PatientVisitsComponent } from './patient-visits/patient-visits.component';
import { DxPopupModule } from 'devextreme-angular/ui/popup';
import { DxScrollViewModule } from 'devextreme-angular/ui/scroll-view'

@NgModule({
    imports: [
        CommonModule,
        DxTabsModule,
        DxDataGridModule,
        ShareModule,
        DxPopupModule,
        DxScrollViewModule
    ],
    declarations: [
        AppointmentsComponent,
        PatientVisitsComponent
    ],
    exports: [
        AppointmentsComponent
    ]
})
export class AppointmentsModule { }