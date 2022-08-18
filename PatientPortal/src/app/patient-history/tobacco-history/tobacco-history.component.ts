import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { DxDataGridComponent, DxFormComponent, DxPopupComponent } from 'devextreme-angular';
import { LibrarySelectableListIds } from 'src/app/core/constants/library-selectable-list-ids.const';
import { SelectableListsNames } from 'src/app/core/constants/selectable-lists-names';
import { DateHelper } from 'src/app/core/helpers/date.helper';
import { SelectableListConfigModel } from 'src/app/core/models/selectable-list-config.model';
import { AlertService } from 'src/app/core/services/alert.service';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { DevextremeAuthService } from 'src/app/core/services/devextreme-auth.service';
import { DxDataUrlService } from 'src/app/core/services/dxDataUrl.service';
import { SelectableListService } from 'src/app/core/services/selectable-list.service';
import { BaseHistoryComponent } from '../classes/base-history-component';
import { TobaccoHistoryModel } from '../models/tobacco-history.model';
import { TobaccoHistoryService } from '../services/tobacco-history.service';
import { createStore } from "devextreme-aspnet-data-nojquery";
import { DefaultValueService } from 'src/app/core/services/default-value.service';
import { PatientChartNodeType } from 'src/app/core/enums/patient-chart-node-types.enum';

@Component({
    selector: "tobacco-history",
    templateUrl: "tobacco-history.component.html"
})
export class TobaccoHistoryComponent extends BaseHistoryComponent implements OnInit, AfterViewInit {
    @ViewChild("tobaccoHistoryDataGrid", { static: false }) tobaccoHistoryDataGrid: DxDataGridComponent;
    @ViewChild("tobaccoHistoryPopup", { static: false }) tobaccoHistoryPopup: DxPopupComponent;
    @ViewChild("tobaccoHistoryForm", { static: false }) tobaccoHistoryForm: DxFormComponent;

    canRenderComponent: boolean = false;

    isTobaccoHistoryPopupOpened: boolean = false;

    isHistoryExist: boolean = false;

    selectedTobaccoHistory = [];
    tobaccoHistory: TobaccoHistoryModel = new TobaccoHistoryModel();
    lastCreatedTobaccoHistory: TobaccoHistoryModel = null;

    isNewTobaccoHistory: boolean = true;
    tobaccoHistoryDataSource: any = {};

    constructor(private alertService: AlertService,
        private tobaccoHistoryService: TobaccoHistoryService,
        private selectableListService: SelectableListService,
        private dxDataUrlService: DxDataUrlService,
        private devextremeAuthService: DevextremeAuthService,
        authenticationService: AuthenticationService,
        defaultValueService: DefaultValueService) {

        super(defaultValueService, authenticationService);

        this.init();
    }

    get isDefaultHistoryValueSelected(): boolean {
        return this.tobaccoHistory.status === this.defaultHistoryValue;
    }

    onPhraseSuggestionApplied($event) {
        this.tobaccoHistory.notes = $event;
    }

    get tobaccoUseStatusListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.tobaccoHistory.tobaccoUseStatus);
    }

    get tobaccoTypeListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.tobaccoHistory.tobaccoType);
    }

    get tobaccoUseListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.tobaccoHistory.tobaccoUse);
    }

    get durationListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.application.duration);
    }

    get frequencyListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.application.frequency);
    }

    get quit(): boolean {
        return this.tobaccoHistory.quit;
    }

    set quit(quitValue: boolean) {
        this.tobaccoHistory.quit = quitValue;

        if (!quitValue) {
            this.tobaccoHistory.statusLength = null;
            this.tobaccoHistory.statusLengthType = null;
        }
    }

    onTobaccoHistoryFieldChanged($event) {
        const dataField = $event.dataField;
        const fieldValue = $event.value;

        const defaultHistoryStatus = this.selectableListService
            .getSelectableListDefaultValueFromComponent(this, SelectableListsNames.tobaccoHistory.tobaccoUseStatus);

        if (dataField === "status" && fieldValue === defaultHistoryStatus) {
            this.resetTobaccoHistory();
        }
    }

    ngAfterViewInit(): void {
        this.registerEscapeBtnEventHandler(this.tobaccoHistoryPopup);
    }

    deleteHistory(tobaccoHistory: TobaccoHistoryModel, $event) {
        $event.stopPropagation();
        const tobaccoHistoryId = tobaccoHistory.id;

        const confirmationPopup = this.alertService
            .confirm("Are you sure you want to delete the history ?", "Confirm deletion");

        confirmationPopup.then(dialogResult => {
            if (dialogResult) {
                this.tobaccoHistoryService.delete(tobaccoHistoryId)
                    .then(() => {
                        this.setLatestTobaccoHistoryIfExists();
                        this.tobaccoHistoryDataGrid.instance.refresh();
                    });
            }
        });
    }

    ngOnInit(): void {
        super.ngOnInit();

        this.initSelectableLists();
        this.setLatestTobaccoHistoryIfExists();
    }

    openTobaccoHistoryForm() {
        this.isTobaccoHistoryPopupOpened = true;
        this.copyFromLastCreatedTobaccoHistory();
    }

    onTobaccoHistoryPopupHidden() {
        this.isNewTobaccoHistory = true;;
        this.selectedTobaccoHistory = [];
        this.tobaccoHistory = new TobaccoHistoryModel();
    }

    createUpdateTobaccoHistory() {
        const validationResult = this.tobaccoHistoryForm.instance
            .validate();

        if (!validationResult.isValid) {
            return;
        }

        if (this.isNewTobaccoHistory)
            this.tobaccoHistory.patientId = this.patientId;

        this.tobaccoHistoryService.save(this.tobaccoHistory)
            .then(() => {
                if (this.tobaccoHistoryDataGrid && this.tobaccoHistoryDataGrid.instance) {
                    this.tobaccoHistoryDataGrid
                        .instance.refresh();
                }
                this.isHistoryExist = true;
                this.isNewTobaccoHistory = true;
                this.isTobaccoHistoryPopupOpened = false;

                this.setLatestTobaccoHistoryIfExists();
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    onTobaccoHistorySelect($event) {
        const selectedTobaccoHistory = $event.selectedRowsData[0];
        if (!selectedTobaccoHistory)
            return;

        const selectedTobaccoHistoryId = selectedTobaccoHistory.id;

        this.tobaccoHistoryService.getById(selectedTobaccoHistoryId)
            .then((tobaccoHistory) => {
                this.tobaccoHistory = tobaccoHistory;
                this.isTobaccoHistoryPopupOpened = true;
                this.isNewTobaccoHistory = false;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private init(): any {
        this.initTobaccoHistoryDataSource();
        this.initDefaultHistoryValue(PatientChartNodeType.TobaccoHistoryNode);
    }

    private initSelectableLists() {
        const tobaccoUseStatusConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.tobaccoHistory.tobaccoUseStatus,
            LibrarySelectableListIds.tobaccoHistory.tobaccoUseStatus);

        const tobaccoTypeListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.tobaccoHistory.tobaccoType,
            LibrarySelectableListIds.tobaccoHistory.tobaccoType);

        const tobaccoUseListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.tobaccoHistory.tobaccoUse,
            LibrarySelectableListIds.tobaccoHistory.tobaccoUse);

        const durationListConfig = new SelectableListConfigModel(this.companyId,
            SelectableListsNames.application.duration,
            LibrarySelectableListIds.application.duration);

        const frequencyListConfig = new SelectableListConfigModel(this.companyId, SelectableListsNames.application.frequency,
            LibrarySelectableListIds.application.frequency);

        const selectableLists = [
            tobaccoUseStatusConfig,
            tobaccoTypeListConfig,
            tobaccoUseListConfig,
            durationListConfig,
            frequencyListConfig
        ];

        this.selectableListService.setSelectableListsValuesToComponent(selectableLists, this)
            .then(() => {
                this.canRenderComponent = true;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private copyFromLastCreatedTobaccoHistory() {
        if (this.lastCreatedTobaccoHistory) {
            this.tobaccoHistory.type = this.lastCreatedTobaccoHistory.type;
            this.tobaccoHistory.status = this.lastCreatedTobaccoHistory.status;
            this.tobaccoHistory.amount = this.lastCreatedTobaccoHistory.amount;
            this.tobaccoHistory.use = this.lastCreatedTobaccoHistory.use;
            this.tobaccoHistory.frequency = this.lastCreatedTobaccoHistory.frequency;
            this.tobaccoHistory.length = this.lastCreatedTobaccoHistory.length;
            this.tobaccoHistory.duration = this.lastCreatedTobaccoHistory.duration;
            this.tobaccoHistory.quit = this.lastCreatedTobaccoHistory.quit;
            this.tobaccoHistory.statusLength = this.lastCreatedTobaccoHistory.statusLength;
            this.tobaccoHistory.statusLengthType = this.lastCreatedTobaccoHistory.statusLengthType;
            this.tobaccoHistory.notes = this.lastCreatedTobaccoHistory.notes;
        }
    }

    private resetTobaccoHistory() {
        this.tobaccoHistory.type = null;
        this.tobaccoHistory.amount = null;
        this.tobaccoHistory.use = null;
        this.tobaccoHistory.frequency = null;
        this.tobaccoHistory.length = null;
        this.tobaccoHistory.duration = null;
        this.tobaccoHistory.quit = false;
        this.tobaccoHistory.statusLength = null;
        this.tobaccoHistory.statusLengthType = null;
    }

    private setLatestTobaccoHistoryIfExists() {
        this.tobaccoHistoryService.getLastCreated(this.patientId)
            .then(tobaccoHistory => {
                this.lastCreatedTobaccoHistory = tobaccoHistory
                    ? tobaccoHistory
                    : new TobaccoHistoryModel();

                this.isHistoryExist = !!tobaccoHistory;
            });
    }

    private initTobaccoHistoryDataSource(): any {
        const appointmentStore = createStore({
            key: "id",
            loadUrl: this.dxDataUrlService.getGridUrl("tobaccohistory"),
            onBeforeSend: this.devextremeAuthService
                .decorateOnBeforeSendMethod((method, jQueryAjaxSettings) => {
                    jQueryAjaxSettings.data.patientId = this.patientId;
                }, this)
        });

        this.tobaccoHistoryDataSource.store = appointmentStore;
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