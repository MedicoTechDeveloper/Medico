import { SelectableListValueModel } from './selectable-list-value.model';

export class SelectableListModel {
    id: string;
    companyId: string;
    title: string;
    selectableListValues: SelectableListValueModel[];
    isActive: boolean;
    categoryId: string;
    isPredefined: boolean;
    version: number | null;
    librarySelectableListId: string;

    constructor() {
        this.isActive = true;
        this.selectableListValues = [];  
    }
}