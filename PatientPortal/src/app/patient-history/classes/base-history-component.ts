import { OnInit } from '@angular/core';
import { DxPopupComponent } from 'devextreme-angular/ui/popup';
import { MaskList } from 'src/app/core/constants/mask-list';
import { PopupConfiguration } from 'src/app/core/constants/popup-configuration';
import { RegexRuleList } from 'src/app/core/constants/regex-rule-list';
import { SearchConfiguration } from 'src/app/core/constants/search-configuration';
import { StateList } from 'src/app/core/constants/state-list';
import { PatientChartNodeType } from 'src/app/core/enums/patient-chart-node-types.enum';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { DefaultValueService } from 'src/app/core/services/default-value.service';

export class BaseHistoryComponent implements OnInit {
    states: any[] = StateList.values;
    validationMasks: MaskList = new MaskList();
    regexRuleList: RegexRuleList = new RegexRuleList();
    searchConfiguration: SearchConfiguration = new SearchConfiguration();
    popupConfiguration: PopupConfiguration = new PopupConfiguration();

    protected patientId: string;
    protected companyId: string;

    defaultHistoryValue: string = "";

    protected constructor(private defaultValueService: DefaultValueService,
        private authenticationService: AuthenticationService) {
    }

    ngOnInit(): void {
        this.initPatientAndCompanyId();
    }
    private initPatientAndCompanyId() {
        const patientUser =
            this.authenticationService.currentUserValue;

        this.patientId = patientUser.patientId;
        this.companyId = patientUser.companyId;
    }

    protected registerEscapeBtnEventHandler(popup: DxPopupComponent): void {
        popup.instance.registerKeyHandler("escape", (event) => {
            event.stopPropagation();
        });
    }

    protected initDefaultHistoryValue(patientChartNodeType: PatientChartNodeType): void {
        this.defaultValueService.getByPatientChartNodeType(patientChartNodeType)
            .then(defaultHistoryValue => {
                this.defaultHistoryValue = defaultHistoryValue.value
                    ? defaultHistoryValue.value
                    : "";
            })
    }
}