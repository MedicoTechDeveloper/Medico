import { Component } from "@angular/core";
import { AuthenticationService } from './core/services/authentication.service';

@Component({
  selector: "app-root",
  templateUrl: "./app.component.html"
})
export class AppComponent {  
  constructor(aurthenicationService: AuthenticationService) {
  }
}