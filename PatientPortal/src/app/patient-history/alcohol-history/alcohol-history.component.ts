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
import { AlcoholHistoryModel } from '../models/alcohol-history.model';
import { AlcoholHistoryService } from '../services/alcohol-history.service';

@Component({
    selector: "alcohol-history",
    templateUrl: "alcohol-history.component.html"
})
export class AlcoholHistoryComponent extends BaseHistoryComponent implements OnInit, AfterViewInit {
    @ViewChild("alcoholHistoryDataGrid", { static: false }) alcoholHistoryDataGrid: DxDataGridComponent;
    @ViewChild("alcoholHistoryPopup", { static: false }) alcoholHistoryPopup: DxPopupComponent;
    @ViewChild("alcoholHistoryForm", { static: false }) alcoholHistoryForm: DxFormComponent;

    get isDefaultHistoryValueSelected(): boolean {
        return this.alcoholHistory.status === this.defaultHistoryValue;
    }

    canRenderComponent: boolean = false;

    isAlcoholHistoryPopupOpened: boolean = false;

    isHistoryExist: boolean = false;

    selectedAlcoholHistory = [];
    alcoholHistory: AlcoholHistoryModel = new AlcoholHistoryModel();
    lastCreatedAlcoholHistory: AlcoholHistoryModel = null;

    isNewAlcoholHistory: boolean = true;
    alcoholHistoryDataSource: any = {};

    constructor(private alertService: AlertService,
        private alcoholHistoryService: AlcoholHistoryService,
        private selectableListService: SelectableListService,
        private dxDataUrlService: DxDataUrlService,
        defaultValueService: DefaultValueService,
        private devextremeAuthService: DevextremeAuthService,
        authenticationService: AuthenticationService) {
        super(defaultValueService, authenticationService);

        this.init();
    }

    onPhraseSuggestionApplied($event) {
        this.alcoholHistory.notes = $event;
    }

    get statusEtohUseListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.alcoholHistory.alcoholUseStatus);
    }

    get typeAlcoholListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.alcoholHistory.alcoholType);
    }

    get useAlcoholListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.alcoholHistory.alcoholUse);
    }

    get durationListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.application.duration);
    }

    get useFrequencyListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.application.frequency)
    }

    get quit(): boolean {
        return this.alcoholHistory.quit;
    }

    set quit(quitValue: boolean) {
        this.alcoholHistory.quit = quitValue;

        if (!quitValue) {
            this.alcoholHistory.statusLength = null;
            this.alcoholHistory.statusLengthType = null;
        }
    }

    onAlcoholHistoryFieldChanged($event) {
        const dataField = $event.dataField;
        const fieldValue = $event.value;

        const defaultHistoryStatus = this.selectableListService
            .getSelectableListDefaultValueFromComponent(this, SelectableListsNames.alcoholHistory.alcoholUseStatus);

        if (dataField === "status" && fieldValue === defaultHistoryStatus) {
            this.resetAlcoholHistory();
        }
    }

    ngAfterViewInit(): void {
        this.registerEscapeBtnEventHandler(this.alcoholHistoryPopup);
    }

    deleteHistory(alcoholHistory: AlcoholHistoryModel, $event) {
        $event.stopPropagation();
        const alcoholHistoryId = alcoholHistory.id;

        const confirmationPopup = this.alertService
            .confirm("Are you sure you want to delete the history ?", "Confirm deletion");

        confirmationPopup.then(dialogResult => {
            if (dialogResult) {
                this.alcoholHistoryService.delete(alcoholHistoryId)
                    .then(() => {
                        this.setLatestAlcoholHistoryIfExists();
                        this.alcoholHistoryDataGrid.instance.refresh();
                    });

            }
        });
    }

    ngOnInit(): void {
        super.ngOnInit();

        this.initSelectableLists();
        this.setLatestAlcoholHistoryIfExists();
    }

    openAlcoholHistoryForm() {
        this.isAlcoholHistoryPopupOpened = true;
        this.copyFromLastCreatedAlcoholHistory();
    }

    onAlcoholHistoryPopupHidden() {
        this.isNewAlcoholHistory = true;;
        this.selectedAlcoholHistory = [];
        this.alcoholHistory = new AlcoholHistoryModel();
    }

    createUpdateAlcoholHistory() {
        const validationResult = this.alcoholHistoryForm.instance
            .validate();

        if (!validationResult.isValid) {
            return;
        }

        if (this.isNewAlcoholHistory)
            this.alcoholHistory.patientId = this.patientId;

        this.alcoholHistoryService.save(this.alcoholHistory)
            .then(() => {
                if (this.alcoholHistoryDataGrid && this.alcoholHistoryDataGrid.instance) {
                    this.alcoholHistoryDataGrid
                        .instance.refresh();
                }
                this.isHistoryExist = true;
                this.isNewAlcoholHistory = true;
                this.isAlcoholHistoryPopupOpened = false;

                this.setLatestAlcoholHistoryIfExists();
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    onAlcoholHistorySelect($event) {
        const selectedAlcoholHistory = $event.selectedRowsData[0];
        if (!selectedAlcoholHistory)
            return;

        const selectedAlcoholHistoryId = selectedAlcoholHistory.id;

        this.alcoholHistoryService.getById(selectedAlcoholHistoryId)
            .then((alcoholHistory) => {
                this.alcoholHistory = alcoholHistory;
                this.isAlcoholHistoryPopupOpened = true;
                this.isNewAlcoholHistory = false;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private init(): any {
        this.initAlcoholHistoryDataSource();
        this.initDefaultHistoryValue(PatientChartNodeType.AlcoholHistoryNode);
    }

    private initSelectableLists() {
        const statusAlcoholUseListConfig =
            new SelectableListConfigModel(this.companyId,
                SelectableListsNames.alcoholHistory.alcoholUseStatus,
                LibrarySelectableListIds.alcoholHistory.alcoholUseStatus);

        const typeAlcoholListConfig =
            new SelectableListConfigModel(this.companyId,
                SelectableListsNames.alcoholHistory.alcoholType,
                LibrarySelectableListIds.alcoholHistory.alcoholType);

        const useAlcoholListConfig =
            new SelectableListConfigModel(this.companyId,
                SelectableListsNames.alcoholHistory.alcoholUse,
                LibrarySelectableListIds.alcoholHistory.alcoholUse);

        const durationListConfig =
            new SelectableListConfigModel(this.companyId,
                SelectableListsNames.application.duration,
                LibrarySelectableListIds.application.duration);

        const frequencyListConfig =
            new SelectableListConfigModel(this.companyId,
                SelectableListsNames.application.frequency,
                LibrarySelectableListIds.application.frequency);

        const selectableLists = [
            statusAlcoholUseListConfig,
            typeAlcoholListConfig,
            useAlcoholListConfig,
            durationListConfig,
            frequencyListConfig
        ];

        this.selectableListService.setSelectableListsValuesToComponent(selectableLists, this)
            .then(() => {
                this.canRenderComponent = true;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private copyFromLastCreatedAlcoholHistory() {
        if (this.lastCreatedAlcoholHistory) {
            this.alcoholHistory.type = this.lastCreatedAlcoholHistory.type;
            this.alcoholHistory.status = this.lastCreatedAlcoholHistory.status;
            this.alcoholHistory.amount = this.lastCreatedAlcoholHistory.amount;
            this.alcoholHistory.use = this.lastCreatedAlcoholHistory.use;
            this.alcoholHistory.frequency = this.lastCreatedAlcoholHistory.frequency;
            this.alcoholHistory.length = this.lastCreatedAlcoholHistory.length;
            this.alcoholHistory.duration = this.lastCreatedAlcoholHistory.duration;
            this.alcoholHistory.quit = this.lastCreatedAlcoholHistory.quit;
            this.alcoholHistory.statusLength = this.lastCreatedAlcoholHistory.statusLength;
            this.alcoholHistory.statusLengthType = this.lastCreatedAlcoholHistory.statusLengthType;
            this.alcoholHistory.notes = this.lastCreatedAlcoholHistory.notes;
        }
    }

    private resetAlcoholHistory() {
        this.alcoholHistory.type = null;
        this.alcoholHistory.amount = null;
        this.alcoholHistory.use = null;
        this.alcoholHistory.frequency = null;
        this.alcoholHistory.length = null;
        this.alcoholHistory.duration = null;
        this.alcoholHistory.quit = false;
        this.alcoholHistory.statusLength = null;
        this.alcoholHistory.statusLengthType = null;
    }

    private setLatestAlcoholHistoryIfExists() {
        this.alcoholHistoryService.getLastCreated(this.patientId)
            .then(alcoholHistory => {
                this.lastCreatedAlcoholHistory = alcoholHistory
                    ? alcoholHistory
                    : new AlcoholHistoryModel();

                this.isHistoryExist = !!alcoholHistory;
            });
    }

    private initAlcoholHistoryDataSource(): any {
        const appointmentStore = createStore({
            key: "id",
            loadUrl: this.dxDataUrlService.getGridUrl("alcoholhistory"),
            onBeforeSend: this.devextremeAuthService
                .decorateOnBeforeSendMethod((method, jQueryAjaxSettings) => {
                    jQueryAjaxSettings.data.patientId = this.patientId;
                }, this)
        });

        this.alcoholHistoryDataSource.store = appointmentStore;
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