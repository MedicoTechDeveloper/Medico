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
import { OccupationalHistoryModel } from '../models/occupational-history.model';
import { OccupationalHistoryService } from '../services/occupational-history.service';

@Component({
    templateUrl: "occupational-history.component.html",
    selector: "occupational-history"
})
export class OccupationalHistoryComponent extends BaseHistoryComponent implements OnInit, AfterViewInit {
    @ViewChild("occupationalHistoryDataGrid", { static: false }) occupationalHistoryDataGrid: DxDataGridComponent;
    @ViewChild("occupationalHistoryPopup", { static: false }) occupationalHistoryPopup: DxPopupComponent;
    @ViewChild("occupationalHistoryForm", { static: false }) occupationalHistoryForm: DxFormComponent;

    currentDate = new Date();
    minOccupationalDate = new Date(1900, 1, 1);

    canRenderComponent: boolean = false;

    isOccupationalHistoryPopupOpened: boolean = false;

    isHistoryExist: boolean = false;

    selectedOccupationalHistory: any[] = [];
    occupationalHistory: any = new OccupationalHistoryModel();

    isNewOccupationalHistory: boolean = true;

    occupationalHistoryDataSource: any = {};
    icdCodesDataSource: any = {};

    constructor(private alertService: AlertService,
        private occupationalHistoryService: OccupationalHistoryService,
        private selectableListService: SelectableListService,
        private dxDataUrlService: DxDataUrlService,
        defaultValueService: DefaultValueService,
        private devextremeAuthService: DevextremeAuthService,
        authenticationService: AuthenticationService) {

        super(defaultValueService, authenticationService);

        this.init();
    }

    onStartDateBoxFocusOut() {
        this.occupationalHistoryForm
            .instance.validate();
    }

    onEndDateBoxFocusOut() {
        this.occupationalHistoryForm
            .instance.validate();
    }

    validateStartDate = (params) => {
        const startDate = params.value;
        if (!startDate)
            return true;

        const endDate = this.occupationalHistory.end;
        if (!endDate)
            return true;

        return startDate < endDate;
    }

    validateEndDate = (params) => {
        const endDate = params.value;
        if (!endDate)
            return true;

        const startDate = this.occupationalHistory.start;
        if (!startDate)
            return true;

        return startDate < endDate;
    }

    onPhraseSuggestionApplied($event) {
        this.occupationalHistory.notes = $event;
    }

    get occupationListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.occupationalHistory.occupation)
    }

    get employmentStatusListValues(): string[] {
        return this.selectableListService
            .getSelectableListValuesFromComponent(this, SelectableListsNames.occupationalHistory.employmentStatus)
    }

    ngAfterViewInit(): void {
        this.registerEscapeBtnEventHandler(this.occupationalHistoryPopup);
    }

    deleteHistory(occupationalHistory: OccupationalHistoryModel, $event) {
        $event.stopPropagation();
        const occupationalHistoryId = occupationalHistory.id;

        const confirmationPopup = this.alertService
            .confirm("Are you sure you want to delete the history ?", "Confirm deletion");

        confirmationPopup.then(dialogResult => {
            if (dialogResult) {
                this.occupationalHistoryService.delete(occupationalHistoryId)
                    .then(() => {
                        this.occupationalHistoryDataGrid.instance.refresh();
                        this.setHistoryExistence();
                    });

            }
        });
    }

    onOccupationalHistoryFieldChanged($event) {
        const dataField = $event.dataField;
        const fieldValue = $event.value;

        if (dataField === "hasDisabilityClaim" && !fieldValue) {
            this.occupationalHistory.disabilityClaimDetails = null;
        }

        if (dataField === "hasWorkersCompensationClaim" && !fieldValue) {
            this.occupationalHistory.workersCompensationClaimDetails = null;
        }

        if (dataField === "occupationalTypeSelectBoxValue" && fieldValue) {
            this.occupationalHistory.occupationalType = fieldValue;
            this.occupationalHistory.occupationalTypeSelectBoxValue = "";
        }
    }

    ngOnInit(): void {
        super.ngOnInit();

        this.initSelectableLists();
        this.setHistoryExistence();
    }

    openOccupationalHistoryForm() {
        this.isOccupationalHistoryPopupOpened = true;
    }

    onOccupationalHistoryPopupHidden() {
        this.isNewOccupationalHistory = true;;
        this.selectedOccupationalHistory = [];
        this.occupationalHistory = new OccupationalHistoryModel();

        this.occupationalHistory.hasDisabilityClaim = false;
        this.occupationalHistory.hasWorkersCompensationClaim = false;
    }

    createUpdateOccupationalHistory() {
        const validationResult = this.occupationalHistoryForm.instance
            .validate();

        if (!validationResult.isValid) {
            return;
        }

        if (this.isNewOccupationalHistory)
            this.occupationalHistory.patientId = this.patientId;

        this.occupationalHistoryService.save(this.occupationalHistory)
            .then(() => {
                if (this.occupationalHistoryDataGrid && this.occupationalHistoryDataGrid.instance) {
                    this.occupationalHistoryDataGrid
                        .instance.refresh();
                }

                this.isHistoryExist = true;
                this.isOccupationalHistoryPopupOpened = false;

            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    getOccupationalDays(gridItem) {
        if (!gridItem.start || !gridItem.end)
            return "";

        const startDate = new Date(gridItem.start);
        const endDate = gridItem.end
            ? new Date(gridItem.end)
            : new Date();

        return DateHelper.getDaysBetween(startDate, endDate);
    }

    onOccupationalHistorySelect($event) {
        const selectedOccupationalHistory = $event.selectedRowsData[0];
        if (!selectedOccupationalHistory)
            return;

        const selectedOccupationalHistoryId = selectedOccupationalHistory.id;

        this.occupationalHistoryService.getById(selectedOccupationalHistoryId)
            .then((occupationalHistory) => {
                this.occupationalHistory = occupationalHistory;

                this.occupationalHistory.hasDisabilityClaim =
                    !!this.occupationalHistory.disabilityClaimDetails;

                this.occupationalHistory.hasWorkersCompensationClaim =
                    !!this.occupationalHistory.workersCompensationClaimDetails;

                this.isOccupationalHistoryPopupOpened = true;
                this.isNewOccupationalHistory = false;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private init(): any {
        this.initOccupationalHistoryDataSource();
        this.initDefaultHistoryValue(PatientChartNodeType.OccupationalHistoryNode);

        this.occupationalHistory.hasDisabilityClaim = false;
        this.occupationalHistory.hasWorkersCompensationClaim = false;
    }

    private initSelectableLists() {
        const occupationListConfig =
            new SelectableListConfigModel(this.companyId,
                SelectableListsNames.occupationalHistory.occupation,
                LibrarySelectableListIds.occupationalHistory.occupation);

        const employmentStatusListConfig =
            new SelectableListConfigModel(this.companyId,
                SelectableListsNames.occupationalHistory.employmentStatus,
                LibrarySelectableListIds.occupationalHistory.employmentStatus);

        const selectableLists = [
            employmentStatusListConfig,
            occupationListConfig
        ];

        this.selectableListService
            .setSelectableListsValuesToComponent(selectableLists, this)
            .then(() => {
                this.canRenderComponent = true;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }

    private initOccupationalHistoryDataSource(): any {
        const appointmentStore = createStore({
            key: "id",
            loadUrl: this.dxDataUrlService.getGridUrl("occupationalhistory"),
            onBeforeSend: this.devextremeAuthService
                .decorateOnBeforeSendMethod((method, jQueryAjaxSettings) => {
                    jQueryAjaxSettings.data.patientId = this.patientId;
                }, this)
        });

        this.occupationalHistoryDataSource.store = appointmentStore;
        this.applyDecoratorForDataSourceLoadFunc(appointmentStore)
    }

    private applyDecoratorForDataSourceLoadFunc(store: any) {
        const nativeLoadFunc = store.load;
        store.load = loadOptions => {
            return nativeLoadFunc.call(store, loadOptions)
                .then(result => {
                    result.forEach(item => {
                        item.createDate = DateHelper.sqlServerUtcDateToLocalJsDate(item.createDate);
                        item.start = DateHelper.sqlServerUtcDateToLocalJsDate(item.start);

                        if (item.end)
                            item.end = DateHelper.sqlServerUtcDateToLocalJsDate(item.end);
                    });
                    return result;
                });
        };
    }

    private setHistoryExistence() {
        this.occupationalHistoryService.isHistoryExist(this.patientId)
            .then(isHistoryExist => {
                this.isHistoryExist = isHistoryExist;
            })
            .catch(error => this.alertService.error(error.message ? error.message : error));
    }
}