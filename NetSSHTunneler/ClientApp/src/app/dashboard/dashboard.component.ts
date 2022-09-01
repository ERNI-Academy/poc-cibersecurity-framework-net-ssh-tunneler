import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CommonService } from '../api/common.service';
import { DashboardDataService } from './dashboard.service';
import { DashboardDataApp } from '../models/models/DashboardData.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  public targetIp = this.commonService.lastBase;
  public showSshTerminal: boolean;

  public sections = this.dashaboarData.getDashboardData();

  constructor(public dialog: MatDialog, private router: Router, private commonService: CommonService, private _snackBar: MatSnackBar, private dashaboarData: DashboardDataService) {
    this.showSshTerminal = false;
  }

  ngOnInit() {
    this.commonService.setTarget
      .subscribe((data: string) => {
        this.targetIp = data;
      });
  }

  public onShowSshTerminal() {
    this.showSshTerminal = !this.showSshTerminal;
  }

  public onOpenApp(app: DashboardDataApp) {
    if (app.targetNeeded && this.commonService.PendingTargetIp()) {
      this.applicationCanNoBeDisplayed();
    } else if (app.name === "SSH") {
      this.onShowSshTerminal();
    }
    else {
      this.router.navigate([app.url]);
    }
  }

  private applicationCanNoBeDisplayed() {
    let msg = "No host selected. You must go to Network Map and select one host.";
    if (this.targetIp == "Pending configuration") {
      msg = "The selected host is not configured! You must go to Network Map and configure it or select another one."
    }
    this._snackBar.open(msg, null, {
      duration: 5 * 1000,
      horizontalPosition: "right",
      verticalPosition: "top",
    });
  }
}
