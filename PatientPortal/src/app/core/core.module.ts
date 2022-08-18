import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgxSpinnerModule } from 'ngx-spinner';
import { AppHeaderComponent } from './app-header/app-header.component';
import { AppLoaderComponent } from './app-loader/app-loader.component';

@NgModule({
    imports: [
        CommonModule,
        NgxSpinnerModule,
        RouterModule
    ],
    declarations: [
        AppHeaderComponent,
        AppLoaderComponent
    ],
    exports: [
        AppHeaderComponent,
        AppLoaderComponent
    ]
})
export class CoreModule { }