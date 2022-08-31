import { Component, Input, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { CommonService } from '../api/common.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  public targetIp = this.commonService.lastBase;
  public showSshTerminal: boolean;
  constructor(public dialog: MatDialog, private router: Router, private commonService: CommonService, private _snackBar: MatSnackBar) {
    this.showSshTerminal = false;
  }

  ngOnInit() {
    this.commonService.setTarget
      .subscribe((data: string) => {
        this.targetIp = data;
      });
  }

  public onSshTerminal() {
    if (this.commonService.PendingTargetIp()) {
      this.applicationCanNoBeDisplayed();
    } else {
      this.showSshTerminal = !this.showSshTerminal;
    }
  }

  private applicationCanNoBeDisplayed() {
    let msg = "No host selected. You must go to Network Map and select one host.";
    if(this.targetIp == "Pending configuration") {
      msg = "The selected host is not configured! You must go to Network Map and configure it or select another one."
    }
    this._snackBar.open(msg, null, {
      duration: 5 * 1000,
      horizontalPosition: "right",
      verticalPosition: "top",
    });
  }

  public onDiscoveryConfiguration() {
    this.router.navigate(['/discovery']);
  }

  public onNetworkMap() {
    this.router.navigate(['/network']);
  }

  public onSettings() {
    this.router.navigate(['/settings']);
  }

  public onRunDiscovery() {
    if (this.commonService.PendingTargetIp()) {
      this.applicationCanNoBeDisplayed();
    } else {
      this.router.navigate(['/run']);
    }
  }

  public onNewTarget() {
    this.router.navigate(['/new']);
  }
}
