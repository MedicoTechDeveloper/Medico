import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { PatientHistoryComponent } from './patient-history.component';
import { DxTreeViewModule } from 'devextreme-angular/ui/tree-view'
import { TobaccoHistoryComponent } from './tobacco-history/tobacco-history.component';
import { TobaccoHistoryService } from './services/tobacco-history.service';
import { DxDataGridModule } from 'devextreme-angular/ui/data-grid'
import { DxPopupModule } from 'devextreme-angular/ui/popup'
import { ShareModule } from '../share/share.module';
import { DxFormModule } from 'devextreme-angular/ui/form'
import { DxTextAreaModule } from 'devextreme-angular/ui/text-area';
import { AlcoholHistoryService } from './services/alcohol-history.service';
import { AlcoholHistoryComponent } from './alcohol-history/alcohol-history.component';
import { DrugHistoryService } from './services/drug-history.service';
import { DrugHistoryComponent } from './drug-history/drug-history.component';
import { MedicalHistoryComponent } from './medical-history/medical-history.component';
import { MedicalHistoryService } from './services/medical-history.service';
import { SurgicalHistoryComponent } from './surgical-history/surgical-history.component';
import { SurgicalHistoryService } from './services/surgical-history.service';
import { FamilyHistoryComponent } from './family-history/family-history.component';
import { FamilyHistoryService } from './services/family-history.service';
import { EducationHistoryService } from './services/education-history.service';
import { EducationHistoryComponent } from './education-history/education-history.component';
import { OccupationalHistoryService } from './services/occupational-history.service';
import { OccupationalHistoryComponent } from './occupational-history/occupational-history.component';
import { AllergyService } from './services/allergy.service';
import { AllergyComponent } from './allergy/allergy.component';
import { DxSelectBoxModule } from 'devextreme-angular/ui/select-box';
import { MedicationHistoryService } from './services/medication-history.service';
import { MedicationHistoryComponent } from './medication-history/medication-history.component';

@NgModule({
    imports: [
        CommonModule,
        ShareModule,
        DxTreeViewModule,
        DxDataGridModule,
        DxPopupModule,
        DxFormModule,
        DxTextAreaModule,
        DxSelectBoxModule
    ],
    declarations: [
        PatientHistoryComponent,
        TobaccoHistoryComponent,
        AlcoholHistoryComponent,
        DrugHistoryComponent,
        MedicalHistoryComponent,
        SurgicalHistoryComponent,
        FamilyHistoryComponent,
        EducationHistoryComponent,
        OccupationalHistoryComponent,
        AllergyComponent,
        MedicationHistoryComponent
    ],
    exports: [
        PatientHistoryComponent
    ],
    providers: [
        TobaccoHistoryService,
        AlcoholHistoryService,
        DrugHistoryService,
        MedicalHistoryService,
        SurgicalHistoryService,
        FamilyHistoryService,
        EducationHistoryService,
        OccupationalHistoryService,
        AllergyService,
        MedicationHistoryService
    ]
})
export class PatientHistoryModule { }