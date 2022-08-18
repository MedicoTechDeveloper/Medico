import { Component, OnInit, Input, AfterViewInit, ViewChild } from "@angular/core";
import { DxDataGridComponent, DxPopupComponent, DxFormComponent } from "devextreme-angular";
import { createStore } from "devextreme-aspnet-data-nojquery";
import { LibrarySelectableListIds } from 'src/app/core/constants/library-selectable-list-ids.const';
import { SelectableListsNames } from 'src/app/core/constants/selectable-lists-names';
import { PatientChartNodeType } from 'src/app/core/enums/patient-chart-node-types.enum';
import { DateHelper } from 'src/app/core/helpers/date.helper';
import { SelectableListConfigModel } from 'src/app/core/models/selectable-list-config.model';
import { AlertService } from 'src/app/core/services/alert.service';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { DefaultValueService } from 'src/app/core/services/default-value.service';
import { DevextremeAuthService } from 'src/app/core/services/devextreme-auth.service';
import { DxDataUrlService } from 'src/app/core/services/dxDataUrl.service';
import { SelectableListService } from 'src/app/core/services/selectable-list.service';
import { BaseHistoryComponent } from '../classes/base-history-component';
import { EducationHistoryModel } from '../models/education-history.model';
import { EducationHistoryService } from '../services/education-history.service';

@Component({
    templateUrl: "education-history.component.html",
    selector: "education-history"
})
export class EducationHistoryComponent extends BaseHistoryComponent implements OnInit, AfterViewInit {
    @ViewChild("educationHistoryDataGrid", { static: false }) educationHistoryDataGrid: DxDataGridComponent;
    @ViewChild("educationHistoryPopup", { static: false }) educationHistoryPopup: DxPopupComponent;
    @ViewChild("educationHistoryForm", { static: false }) educationHistoryForm: DxFormComponent;

    canRenderComponent: boolean = false;

    minCompletedYearNumber: number = 1950;
    maxCompletedYearNumber: number = new Date().getFullYear();

    isEducationHistoryPopupOpened: boolean = false;

    isHistoryExist: boolean = false;

    selectedEducationHistory: Array<any> = [];
    educationHistory: any = new EducationHistoryModel();

    isNewEducationHistory: boolean = true;

    educationHistoryDataSource: any = {};
    icdCodesDataSource: any = {};

    constructor(private alertService: AlertService,
        private educationHistoryService: EducationHistoryService,
        private selectableListService: SelectableListService,
        private dxDataUrlService: DxDataUrlService,
        defaultValueService: DefaultValueService,
        private devextremeAuthService: DevextremeAuthService,
        authenticationService: AuthenticationService) {

        super(defaultValueService, authenticationService);

        this.init();
    }

    onEducationHistoryFieldChanged($event) {
        const dataField = $event.dataField;
        const fieldValue = $event.value;

        if (dataField === "degreeSelectBoxValue" && fieldValue) {
            this.educationHistory.degree = fieldValue;
            this.educationHistory.degreeSelectBoxValue = "";
        }
    }

    onPhraseSuggestionApplied($event) {
        this.educationHistory.notes = $event;
    }

    get educationListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.educationHistory.education);
    }

    ngAfterViewInit(): void {
        this.registerEscapeBtnEventHandler(this.educationHistoryPopup);
    }

    deleteHistory(educationHistory: EducationHistoryModel, $event) {
        $event.stopPropagation();
        const educationHistoryId = educationHistory.id;

        const confirmationPopup = this.alertService
            .confirm("Are you sure you want to delete the history ?", "Confirm deletion");

        confirmationPopup.then(dialogResult => {
            if (dialogResult) {
                this.educationHistoryService.delete(educationHistoryId)
                    .then(() => {
                        this.educationHistoryDataGrid.instance.refresh();
                        this.setHistoryExistence();
                    });
            }
        });
    }

    ngOnInit(): void {
        super.ngOnInit();

        this.initSelectableLists();
        this.setHistoryExistence();
        this.initDefaultHistoryValue(PatientChartNodeType.EducationNode);
    }

    openEducationHistoryForm() {
        this.isEducationHistoryPopupOpened = true;
    }

    onEducationHistoryPopupHidden() {
        this.isNewEducationHistory = true;;
        this.selectedEducationHistory = [];
        this.educationHistory = new EducationHistoryModel();
    }

    createUpdateEducationHistory() {
        const validationResult = this.educationHistoryForm.instance
            .validate();

        if (!validationResult.isValid) {
            return;
        }

        if (this.isNewEducationHistory)
            this.educationHistory.patientId = this.patientId;

        this.educationHistoryService.save(this.educationHistory)
            .then(() => {
                if (this.educationHistoryDataGrid && this.educationHistoryDataGrid.instance) {
                    this.educationHistoryDataGrid
                        .instance.refresh();
                }

                this.isHistoryExist = true;
                this.isEducationHistoryPopupOpened = false;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    onEducationHistorySelect($event) {
        const selectedEducationHistory = $event.selectedRowsData[0];
        if (!selectedEducationHistory)
            return;

        const selectedEducationHistoryId = selectedEducationHistory.id;

        this.educationHistoryService.getById(selectedEducationHistoryId)
            .then((educationHistory) => {
                this.educationHistory = educationHistory;
                this.isEducationHistoryPopupOpened = true;
                this.isNewEducationHistory = false;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private init(): any {
        this.initEducationHistoryDataSource();
    }

    private initSelectableLists() {
        const educationListConfig =
            new SelectableListConfigModel(this.companyId,
                SelectableListsNames.educationHistory.education,
                LibrarySelectableListIds.educationHistory.education);

        const selectableLists = [educationListConfig];

        this.selectableListService
            .setSelectableListsValuesToComponent(selectableLists, this)
            .then(() => {
                this.canRenderComponent = true;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private initEducationHistoryDataSource(): any {
        const appointmentStore = createStore({
            key: "id",
            loadUrl: this.dxDataUrlService.getGridUrl("educationhistory"),
            onBeforeSend: this.devextremeAuthService
                .decorateOnBeforeSendMethod((method, jQueryAjaxSettings) => {
                    jQueryAjaxSettings.data.patientId = this.patientId;
                }, this)
        });

        this.educationHistoryDataSource.store = appointmentStore;
        this.applyDecoratorForDataSourceLoadFunc(appointmentStore)
    }

    private applyDecoratorForDataSourceLoadFunc(store: any) {
        const nativeLoadFunc = store.load;
        store.load = loadOptions => {
            return nativeLoadFunc.call(store, loadOptions)
                .then(result => {
                    result.forEach(item => {
                        item.createDate =
                            DateHelper.sqlServerUtcDateToLocalJsDate(item.createDate);
                    });
                    return result;
                });
        };
    }

    private setHistoryExistence() {
        this.educationHistoryService.isHistoryExist(this.patientId)
            .then(isHistoryExist => {
                this.isHistoryExist = isHistoryExist;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }
}