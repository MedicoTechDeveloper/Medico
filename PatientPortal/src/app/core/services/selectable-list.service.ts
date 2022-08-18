import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from '@angular/common/http';
import { ConfigService } from './config.service';
import { ApiBaseUrls } from '../constants/api-base-urls';
import { SelectableListConfigModel } from '../models/selectable-list-config.model';
import { SelectableListSearchFilterModel } from '../models/selectable-list-search-filter.model';
import { SelectableListResultModel } from '../models/selectable-list-result.model';
import { ControlDefaultValues } from '../constants/control-default-values';
import { SelectableListModel } from '../models/selectable-list.model';

@Injectable({ providedIn: "root" })
export class SelectableListService {
    baseSelectableListUrl: string = ApiBaseUrls.selectableList;

    constructor(private http: HttpClient, private config: ConfigService) {
    }

    getSelectableListDefaultValueFromComponent(component: any, selectableListName: string): string {
        const selectabelList: SelectableListResultModel = component[selectableListName];
        return selectabelList && selectabelList.defaultValue
            ? selectabelList.defaultValue
            : ""
    }

    getSelectableListValuesFromComponent(component: any, selectableListName: string): any[] {
        const selectabelList = component[selectableListName];
        return selectabelList && selectabelList.values && selectabelList.values.length
            ? component[selectableListName].values
            : [];
    }

    setSelectableListsValuesToComponent(selectableListsConfigs: SelectableListConfigModel[], component: any): Promise<void> {
        const companyId = selectableListsConfigs[0].companyId;

        const filter = new SelectableListSearchFilterModel();
        filter.companyId = companyId;
        filter.librarySelectableListIds = selectableListsConfigs
            .map(c => c.librarySelectableListId);

        return this.getByFilter(filter)
            .then(selectableLists => {
                for (let i = 0; i < selectableLists.length; i++) {
                    const selectableList = selectableLists[i];
                    const librarySelectableListId = selectableList.librarySelectableListId;

                    const selectableListResult = new SelectableListResultModel();

                    const selectabelListConfig = selectableListsConfigs
                        .find(c => c.librarySelectableListId === librarySelectableListId);

                    const selectabelListName = selectabelListConfig.name;
                    const includeNotSetValue = selectabelListConfig.includeNotSetValue;

                    const selectableListValues = selectableList.selectableListValues;

                    if (selectableListValues.length) {
                        const stringValues = selectableListValues.map(s => s.value)

                        if (includeNotSetValue) {
                            stringValues.push(ControlDefaultValues.selectBox);
                        }

                        const defaultSelectableListValue = selectableListValues
                            .find(s => s.isDefault);

                        //every selectable list has to have default value
                        //todo: create patcher to set default value for selectable list if doesn't exist
                        selectableListResult.defaultValue = defaultSelectableListValue
                            ? defaultSelectableListValue.value
                            : selectableListValues[0].value;

                        selectableListResult.values = stringValues;
                    }

                    component[selectabelListName] = selectableListResult;
                }
            });
    }

    private getByFilter(searchFilter: SelectableListSearchFilterModel): Promise<SelectableListModel[]> {
        const queryParams = new HttpParams({
            fromObject: searchFilter.toQueryParams()
        });
        return this.http.get<SelectableListModel[]>(`${this.config.apiUrl}${this.baseSelectableListUrl}`, { params: queryParams })
            .toPromise();
    }
}