import { Component } from '@angular/core';
import { MatSnackBar } from '@angular/material';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { NetworkServiceApi } from '../api/network.api';
import { DialogSimpleMessage } from '../dialogs/dialog-simple-message/dialog-simple-message.component';
import { HostInfo, ConectionInfo } from '../models/responses/HostInfo.model';

@Component({
  selector: 'app-connection-form',
  templateUrl: './connection-form.component.html',
  styleUrls: ['./connection-form.component.scss']
})
export class ConnectionFormComponent {
  public targetIp: string;
  public targetPort: string;
  public userName: string;
  public password: string;
  public certificate: string;
  public aditionalSshParameters: string;

  constructor(private networkApi: NetworkServiceApi, public dialog: MatDialog, private router: Router, private _snackBar: MatSnackBar) {

  }

  goBack() {
    this.router.navigate(['/'])
  }

  public async checkconnection() {
    const newTarget = new HostInfo();
    newTarget.network = "";
    newTarget.parent = "";
    newTarget.ports = new Array<number>();
    newTarget.conectionInfo = new ConectionInfo()
    newTarget.conectionInfo.targetIp = this.targetIp;
    newTarget.conectionInfo.targetPort = this.targetPort;
    newTarget.conectionInfo.userName = this.userName;
    newTarget.conectionInfo.password = this.password;
    newTarget.conectionInfo.certificate = this.certificate;
    newTarget.conectionInfo.additionalSshParameters = this.aditionalSshParameters;
    const saved = await this.networkApi.saveTargetConfig(newTarget);

    if (saved) {
      this._snackBar.open(`Target [${newTarget.conectionInfo.targetIp}] saved!`, null, {
        duration: 5 * 1000,
        horizontalPosition: "right",
        verticalPosition: "top",
      });
      this.router.navigate(['/']);
    }
  }
}
