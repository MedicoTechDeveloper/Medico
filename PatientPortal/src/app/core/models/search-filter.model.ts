import { BaseSearchFilterModel } from './base-search-filter.model';;

export class SearchFilterModel extends BaseSearchFilterModel {
    take: number;
    companyId: string | null;
    isActive: boolean | null;
    title: string;
}