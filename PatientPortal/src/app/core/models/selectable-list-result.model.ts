export class SelectableListResultModel {
    values: string[];
    defaultValue: string;

    constructor(values: string[] = [], defaultValue: string = "") {
        this.values = values;
        this.defaultValue = defaultValue;
    }
}