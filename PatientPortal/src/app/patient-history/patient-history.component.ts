import { Component } from '@angular/core';
import { PATIENT_HISTORY_TREE_VIEW_ITEM_NAMES } from './constants/patient-history-tree-view-item-names.const';

@Component({
    selector: "patient-history",
    templateUrl: "patient-history.component.html"
})
export class PatientHistoryComponent {
    patientHistoryTreeViewItemNames = PATIENT_HISTORY_TREE_VIEW_ITEM_NAMES;

    currentlyOpenedTreeViewItem: string =
        this.patientHistoryTreeViewItemNames.TOBACCO_HISTORY;

    patientHistoryTreeViewItems = [];

    constructor() {
        this.initPatientHistoryTreeViewItems();
    }

    isTreeViewItemOpened(patientHistoryTreeViewItemName: string): boolean {
        return patientHistoryTreeViewItemName === this.currentlyOpenedTreeViewItem;
    }

    selectPatientHistoryTreeViewItem($event) {
        if (!$event || !$event.itemData || !$event.itemData.name)
            return;

        this.currentlyOpenedTreeViewItem = $event.itemData.name;
    }

    private initPatientHistoryTreeViewItems() {
        const patientHistoryTreeViewItems = [];

        for (let patientHistoryTreeViewItemName in this.patientHistoryTreeViewItemNames) {
            const patientHistoryTreeViewItemNameValue =
                this.patientHistoryTreeViewItemNames[patientHistoryTreeViewItemName];

            patientHistoryTreeViewItems.push({
                text: patientHistoryTreeViewItemNameValue,
                name: patientHistoryTreeViewItemNameValue
            })
        }

        this.patientHistoryTreeViewItems = patientHistoryTreeViewItems;
    }
}