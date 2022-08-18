import { ArrayHelper } from '../helpers/array.helper';

export class BaseSearchFilterModel {
    toQueryParams(): any {
        const queryParams = {};
        const entityFilterFields = Object.keys(this);

        if (!entityFilterFields.length)
            return queryParams;

        for (let i = 0; i < entityFilterFields.length; i++) {
            const entityFilterField = entityFilterFields[i];
            const entityFilterFieldValue = this[entityFilterField];

            queryParams[entityFilterField] = ArrayHelper.isArrayOfStrings(entityFilterFieldValue)
                ? entityFilterFieldValue.join(",")
                : entityFilterFieldValue;
        }

        return queryParams;
    }
}