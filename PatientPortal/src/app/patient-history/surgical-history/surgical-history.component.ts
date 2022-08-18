import { Component, OnInit, Input, AfterViewInit, ViewChild } from "@angular/core";
import { DxDataGridComponent, DxPopupComponent, DxFormComponent } from "devextreme-angular";
import { createStore } from "devextreme-aspnet-data-nojquery";
import { PatientChartNodeType } from 'src/app/core/enums/patient-chart-node-types.enum';
import { DateHelper } from 'src/app/core/helpers/date.helper';
import { AlertService } from 'src/app/core/services/alert.service';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { CptCodeService } from 'src/app/core/services/cpt-code.service';
import { DefaultValueService } from 'src/app/core/services/default-value.service';
import { DevextremeAuthService } from 'src/app/core/services/devextreme-auth.service';
import { DxDataUrlService } from 'src/app/core/services/dxDataUrl.service';
import { BaseHistoryComponent } from '../classes/base-history-component';
import { SurgicalHistoryModel } from '../models/surgical-history.model';
import { SurgicalHistoryService } from '../services/surgical-history.service';

@Component({
    templateUrl: "surgical-history.component.html",
    selector: "surgical-history"
})
export class SurgicalHistoryComponent extends BaseHistoryComponent implements OnInit, AfterViewInit {
    @ViewChild("surgicalHistoryDataGrid", { static: false }) surgicalHistoryDataGrid: DxDataGridComponent;
    @ViewChild("surgicalHistoryPopup", { static: false }) surgicalHistoryPopup: DxPopupComponent;
    @ViewChild("surgicalHistoryForm", { static: false }) surgicalHistoryForm: DxFormComponent;

    isSurgicalHistoryPopupOpened: boolean = false;

    isHistoryExist: boolean = false;

    selectedSurgicalHistory = [];
    surgicalHistory: any = new SurgicalHistoryModel();

    isNewSurgicalHistory: boolean = true;
    surgicalHistoryDataSource: any = {};

    icdCodesDataSource: any = {};

    constructor(private alertService: AlertService,
        private surgicalHistoryService: SurgicalHistoryService,
        private dxDataUrlService: DxDataUrlService,
        private cptCodeService: CptCodeService,
        defaultValueService: DefaultValueService,
        private devextremeAuthService: DevextremeAuthService,
        authenticationService: AuthenticationService,) {
        super(defaultValueService, authenticationService);

        this.init();
    }

    onPhraseSuggestionApplied($event) {
        this.surgicalHistory.notes = $event;
    }

    onSurgicalHistoryFieldChanged($event) {
        const dataField = $event.dataField;
        const fieldValue = $event.value;

        if (dataField === "cptCode" && fieldValue) {
            this.cptCodeService.getById(fieldValue)
                .then(cptCode => {
                    this.surgicalHistory.diagnosis = cptCode.description;
                    this.surgicalHistory.cptCode = "";
                })
                .catch(error => this.alertService.error(error.message ? error.message : error));
        }
    }

    ngAfterViewInit(): void {
        this.registerEscapeBtnEventHandler(this.surgicalHistoryPopup);
    }

    deleteHistory(surgicalHistory: SurgicalHistoryModel, $event) {
        $event.stopPropagation();
        const surgicalHistoryId = surgicalHistory.id;

        const confirmationPopup = this.alertService
            .confirm("Are you sure you want to delete the history ?", "Confirm deletion");

        confirmationPopup.then(dialogResult => {
            if (dialogResult) {
                this.surgicalHistoryService.delete(surgicalHistoryId)
                    .then(() => {
                        this.surgicalHistoryDataGrid.instance.refresh();
                        this.setHistoryExistence();
                    });
            }
        });
    }

    ngOnInit(): void {
        super.ngOnInit();
        this.setHistoryExistence();
    }

    openSurgicalHistoryForm() {
        this.isSurgicalHistoryPopupOpened = true;
    }

    onSurgicalHistoryPopupHidden() {
        this.isNewSurgicalHistory = true;;
        this.selectedSurgicalHistory = [];
        this.surgicalHistory = new SurgicalHistoryModel();
    }

    createUpdateSurgicalHistory() {
        const validationResult = this.surgicalHistoryForm.instance
            .validate();

        if (!validationResult.isValid) {
            return;
        }

        this.surgicalHistory.createDate = DateHelper
            .jsLocalDateToSqlServerUtc(this.surgicalHistory.createDate);

        if (this.isNewSurgicalHistory)
            this.surgicalHistory.patientId = this.patientId;

        this.surgicalHistoryService.save(this.surgicalHistory)
            .then(() => {
                if (this.surgicalHistoryDataGrid && this.surgicalHistoryDataGrid.instance) {
                    this.surgicalHistoryDataGrid
                        .instance.refresh();
                }

                this.isHistoryExist = true;
                this.isSurgicalHistoryPopupOpened = false;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    onSurgicalHistorySelect($event) {
        const selectedSurgicalHistory = $event.selectedRowsData[0];
        if (!selectedSurgicalHistory)
            return;

        const selectedSurgicalHistoryId = selectedSurgicalHistory.id;

        this.surgicalHistoryService.getById(selectedSurgicalHistoryId)
            .then((surgicalHistory) => {
                this.surgicalHistory = surgicalHistory;
                this.isSurgicalHistoryPopupOpened = true;
                this.isNewSurgicalHistory = false;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private init(): any {
        this.initSurgicalHistoryDataSource();
        this.initCptCodeDataSource();
        this.initDefaultHistoryValue(PatientChartNodeType.PreviousSurgicalHistoryNode);
    }

    private setHistoryExistence() {
        this.surgicalHistoryService.isHistoryExist(this.patientId)
            .then(isHistoryExist => {
                this.isHistoryExist = isHistoryExist;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private initSurgicalHistoryDataSource(): any {
        const appointmentStore = createStore({
            key: "id",
            loadUrl: this.dxDataUrlService.getGridUrl("surgicalhistory"),
            onBeforeSend: this.devextremeAuthService
                .decorateOnBeforeSendMethod((method, jQueryAjaxSettings) => {
                    jQueryAjaxSettings.data.patientId = this.patientId;
                }, this)
        });

        this.surgicalHistoryDataSource.store = appointmentStore;
        this.applyDecoratorForDataSourceLoadFunc(appointmentStore)
    }

    private applyDecoratorForDataSourceLoadFunc(store: any) {
        const nativeLoadFunc = store.load;
        store.load = loadOptions => {
            return nativeLoadFunc.call(store, loadOptions)
                .then(result => {
                    result.forEach(item => {
                        item.createDate = DateHelper.sqlServerUtcDateToLocalJsDate(item.createDate);
                    });
                    return result;
                });
        };
    }

    private initCptCodeDataSource(): void {
        this.icdCodesDataSource.store = createStore({
            loadUrl: this.dxDataUrlService.getLookupUrl("cptcode"),
            key: "Id",
            onBeforeSend: this.devextremeAuthService
                .decorateOnBeforeSendMethod((method, jQueryAjaxSettings) => { }, this)
        });
    }
}