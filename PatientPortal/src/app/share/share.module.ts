import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LocaDatePipe, DatePipe, TimePipe, AgePipe } from './pipes/date.pipe';
import { DxPopupModule, DxListModule, DxTextAreaModule, DxButtonModule, DxNumberBoxModule, DxTextBoxModule, DxSelectBoxModule, DxFormModule } from 'devextreme-angular';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';
import { NotSetPipe } from './pipes/not-set.pipe';
import { DebounceClickDirective } from './directives/debounce-click.directive';
import { ValueComponent } from './components/value-component/value.component';

@NgModule({
    imports: [
        CommonModule,
        DxPopupModule,
        DxListModule,
        DxTextAreaModule,
        DxButtonModule,
        DxNumberBoxModule,
        DxTextBoxModule,
        DxSelectBoxModule,
        DxFormModule
    ],
    declarations: [
        LocaDatePipe,
        DatePipe,
        SafeHtmlPipe,
        TimePipe,
        AgePipe,
        NotSetPipe,
        DebounceClickDirective,
        ValueComponent
    ],
    exports: [
        AgePipe,
        TimePipe,
        DatePipe,
        LocaDatePipe,
        NotSetPipe,
        SafeHtmlPipe,
        DebounceClickDirective,
        ValueComponent
    ]
})
export class ShareModule { }