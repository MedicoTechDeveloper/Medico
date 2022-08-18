import { Component, OnInit, AfterViewInit, ViewChild } from "@angular/core";
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
import { DrugHistoryModel } from '../models/drug-history.model';
import { DrugHistoryService } from '../services/drug-history.service';

@Component({
    selector: "drug-history",
    templateUrl: "drug-history.component.html"
})
export class DrugHistoryComponent extends BaseHistoryComponent implements OnInit, AfterViewInit {
    @ViewChild("drugHistoryDataGrid", { static: false }) drugHistoryDataGrid: DxDataGridComponent;
    @ViewChild("drugHistoryPopup", { static: false }) drugHistoryPopup: DxPopupComponent;
    @ViewChild("drugHistoryForm", { static: false }) drugHistoryForm: DxFormComponent;

    get isDefaultHistoryValueSelected(): boolean {
        return this.drugHistory.status === this.defaultHistoryValue;
    }

    canRenderComponent: boolean = false;

    isDrugHistoryPopupOpened: boolean = false;

    isHistoryExist: boolean = false;

    selectedDrugHistory = [];
    drugHistory: DrugHistoryModel = new DrugHistoryModel();

    lastCreatedDrugHistory: DrugHistoryModel = null;

    isNewDrugHistory: boolean = true;
    drugHistoryDataSource: any = {};

    constructor(private alertService: AlertService,
        private drugHistoryService: DrugHistoryService,
        private selectableListService: SelectableListService,
        private dxDataUrlService: DxDataUrlService,
        defaultValueService: DefaultValueService,
        private devextremeAuthService: DevextremeAuthService,
        authenticationService: AuthenticationService) {
        super(defaultValueService, authenticationService);

        this.init();
    }

    onPhraseSuggestionApplied($event) {
        this.drugHistory.notes = $event;
    }

    get statusDrugUseListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.drugHistory.drugUseStatus);
    }

    get typeDrugListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.drugHistory.drugType);
    }

    get useDrugListValues(): string[] {
        return this.selectableListService.
            getSelectableListValuesFromComponent(this, SelectableListsNames.drugHistory.drugUse);
    }

    get useDrugRouteListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.drugHistory.drugUseRoute);
    }

    get durationListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.application.duration);
    }

    get useFrequencyListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.application.frequency);
    }

    get quit(): boolean {
        return this.drugHistory.quit;
    }

    set quit(quitValue: boolean) {
        this.drugHistory.quit = quitValue;

        if (!quitValue) {
            this.drugHistory.statusLength = null;
            this.drugHistory.statusLengthType = null;
        }
    }

    onDrugHistoryFieldChanged($event) {
        const dataField = $event.dataField;
        const fieldValue = $event.value;

        const defaultHistoryStatus = this.selectableListService
            .getSelectableListDefaultValueFromComponent(this, SelectableListsNames.drugHistory.drugUse);

        if (dataField === "status" && fieldValue === defaultHistoryStatus) {
            this.resetDrugHistory();
        }
    }

    ngAfterViewInit(): void {
        this.registerEscapeBtnEventHandler(this.drugHistoryPopup);
    }

    deleteHistory(drugHistory: DrugHistoryModel, $event) {
        $event.stopPropagation();
        const drugHistoryId = drugHistory.id;

        const confirmationPopup = this.alertService
            .confirm("Are you sure you want to delete the history ?", "Confirm deletion");

        confirmationPopup.then(dialogResult => {
            if (dialogResult) {
                this.drugHistoryService.delete(drugHistoryId)
                    .then(() => {
                        this.setLatestDrugHistoryIfExists();
                        this.drugHistoryDataGrid.instance.refresh();
                    });

            }
        });
    }

    ngOnInit(): void {
        super.ngOnInit();

        this.initSelectableLists();
        this.setLatestDrugHistoryIfExists();
    }

    openDrugHistoryForm() {
        this.isDrugHistoryPopupOpened = true;
        this.copyFromLastCreatedDrugHistory();
    }

    onDrugHistoryPopupHidden() {
        this.isNewDrugHistory = true;;
        this.selectedDrugHistory = [];
        this.drugHistory = new DrugHistoryModel();
    }

    createUpdateDrugHistory() {
        const validationResult = this.drugHistoryForm.instance
            .validate();

        if (!validationResult.isValid) {
            return;
        }

        if (this.isNewDrugHistory)
            this.drugHistory.patientId = this.patientId;

        this.drugHistoryService.save(this.drugHistory)
            .then(() => {
                if (this.drugHistoryDataGrid && this.drugHistoryDataGrid.instance) {
                    this.drugHistoryDataGrid
                        .instance.refresh();
                }
                this.isHistoryExist = true;
                this.isNewDrugHistory = true;
                this.isDrugHistoryPopupOpened = false;

                this.setLatestDrugHistoryIfExists();
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    onDrugHistorySelect($event) {
        const selectedDrugHistory = $event.selectedRowsData[0];
        if (!selectedDrugHistory)
            return;

        const selectedDrugHistoryId = selectedDrugHistory.id;

        this.drugHistoryService.getById(selectedDrugHistoryId)
            .then((drugHistory) => {
                this.drugHistory = drugHistory;
                this.isDrugHistoryPopupOpened = true;
                this.isNewDrugHistory = false;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private init(): any {
        this.initDrugHistoryDataSource();
        this.initDefaultHistoryValue(PatientChartNodeType.DrugHistoryNode);
    }

    private initSelectableLists() {
        const drugUseStatusListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.drugHistory.drugUseStatus,
            LibrarySelectableListIds.drugHistory.drugUseStatus);

        const drugTypeListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.drugHistory.drugType,
            LibrarySelectableListIds.drugHistory.drugType);

        const drugUseListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.drugHistory.drugUse,
            LibrarySelectableListIds.drugHistory.drugUse);

        const drugUseRouteListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.drugHistory.drugUseRoute,
            LibrarySelectableListIds.drugHistory.drugUseRoute);

        const durationListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.application.duration,
            LibrarySelectableListIds.application.duration);

        const frequencyListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.application.frequency,
            LibrarySelectableListIds.application.frequency);

        const selectableLists = [
            drugUseStatusListConfig,
            drugTypeListConfig,
            drugUseListConfig,
            durationListConfig,
            frequencyListConfig,
            drugUseRouteListConfig
        ];

        this.selectableListService.setSelectableListsValuesToComponent(selectableLists, this)
            .then(() => {
                this.canRenderComponent = true;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private copyFromLastCreatedDrugHistory() {
        if (this.lastCreatedDrugHistory) {
            this.drugHistory.type = this.lastCreatedDrugHistory.type;
            this.drugHistory.route = this.lastCreatedDrugHistory.route;
            this.drugHistory.status = this.lastCreatedDrugHistory.status;
            this.drugHistory.amount = this.lastCreatedDrugHistory.amount;
            this.drugHistory.use = this.lastCreatedDrugHistory.use;
            this.drugHistory.frequency = this.lastCreatedDrugHistory.frequency;
            this.drugHistory.length = this.lastCreatedDrugHistory.length;
            this.drugHistory.duration = this.lastCreatedDrugHistory.duration;
            this.drugHistory.quit = this.lastCreatedDrugHistory.quit;
            this.drugHistory.statusLength = this.lastCreatedDrugHistory.statusLength;
            this.drugHistory.statusLengthType = this.lastCreatedDrugHistory.statusLengthType;
            this.drugHistory.notes = this.lastCreatedDrugHistory.notes;
        }
    }

    private resetDrugHistory() {
        this.drugHistory.type = null;
        this.drugHistory.amount = null;
        this.drugHistory.use = null;
        this.drugHistory.frequency = null;
        this.drugHistory.length = null;
        this.drugHistory.duration = null;
        this.drugHistory.quit = false;
        this.drugHistory.statusLength = null;
        this.drugHistory.statusLengthType = null;
        this.drugHistory.route = null;
    }

    private setLatestDrugHistoryIfExists() {
        this.drugHistoryService.getLastCreated(this.patientId)
            .then(drugHistory => {
                this.lastCreatedDrugHistory = drugHistory
                    ? drugHistory
                    : new DrugHistoryModel();

                this.isHistoryExist = !!drugHistory;
            });
    }

    private initDrugHistoryDataSource(): any {
        const appointmentStore = createStore({
            key: "id",
            loadUrl: this.dxDataUrlService.getGridUrl("drughistory"),
            onBeforeSend: this.devextremeAuthService
                .decorateOnBeforeSendMethod((method, jQueryAjaxSettings) => {
                    jQueryAjaxSettings.data.patientId = this.patientId;
                }, this)
        });

        this.drugHistoryDataSource.store = appointmentStore;
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
}