import { Component, Input } from "@angular/core";

@Component({
  selector: "changepassword",
  templateUrl: "./changepassword.component.html"
})
export class ChangePasswordComponent {
  @Input() email: string;
}
