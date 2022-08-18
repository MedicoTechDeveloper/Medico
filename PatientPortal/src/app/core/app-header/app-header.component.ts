import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { AuthenticationService } from '../services/authentication.service';

@Component({
  selector: "app-header",
  templateUrl: "./app-header.component.html",
  styles: [`.active-link {
              color: white !important;
              cursor: unset;}`]
})
export class AppHeaderComponent implements OnInit, OnDestroy {
  currentUserSubscription: Subscription;

  isAppointmentsLinkVisible: boolean = false;
  isLogoutLinkVisible: boolean = false;
  isProfileLinkVisible: boolean = false;
  isPatientHistoryLinkVisible: boolean = false;

  constructor(private authenticationService: AuthenticationService) {
  }

  ngOnInit(): void {
    this.currentUserSubscription = this.authenticationService
      .currentUser.subscribe(currentUser => {
        if (currentUser && currentUser.isAuthenticated) {
          this.isAppointmentsLinkVisible = true;
          this.isLogoutLinkVisible = true;
          this.isProfileLinkVisible = true;
          this.isPatientHistoryLinkVisible = true;
        }
      });
  }

  ngOnDestroy(): void {
    this.currentUserSubscription.unsubscribe();
  }

  logout($event): void {
    $event.preventDefault();
    this.authenticationService.logout()
      .then(() => {
        location.reload();
      });
  }
}
