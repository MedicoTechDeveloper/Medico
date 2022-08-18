import { BaseSearchFilterModel } from './base-search-filter.model';

export class UserIdentificationInfoModel extends BaseSearchFilterModel {
    firstName: string;
    lastName: string;
    dateOfBirth: any;

    constructor() {
        super();
    }
}