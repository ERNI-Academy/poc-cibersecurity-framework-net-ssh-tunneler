import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NetworkServiceApi } from '../api/network.api';
import { NetWorkPrint } from '../models/responses/NetWorkPrint.model';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { CommonService } from '../api/common.service';
import { TargetDto } from '../models/responses/TargetDto.model';
import { ConectionInfo, HostInfo } from '../models/responses/HostInfo.model';
import { MatSnackBar } from '@angular/material';
import { CommandServiceApi } from '../api/command.api';
import { PortRedirectionCommandDto } from '../models/dtos/PortRedirectionCommandDto.model';

@Component({
  selector: 'app-network-map',
  templateUrl: './network-map.component.html',
  styleUrls: ['./network-map.component.scss']
})
export class NetworkMapComponent implements OnInit {

  networkMap: NetWorkPrint[];
  treeControl = new NestedTreeControl<NetWorkPrint>(node => node.netWork);
  dataSource = new MatTreeNestedDataSource<NetWorkPrint>();
  selectedHost: HostInfo;
  isSelectedHost: boolean;
  isSelectedPort: boolean;
  selectedPort: string;
  destinationHost: string;
  destinationPort: string;
  constructor(private networkApi: NetworkServiceApi, private router: Router, private commandApi: CommandServiceApi, private commonService: CommonService, private _snackBar: MatSnackBar) {
  };

  async ngOnInit() {
    this.isSelectedHost = false;
    this.selectedPort = "";
    this.selectedHost = new HostInfo();
    this.selectedHost.conectionInfo = new ConectionInfo();
    this.networkMap = await this.networkApi.getNetworkMap();
    this.dataSource.data = this.networkMap;
  }

  goBack() {
    this.router.navigate(['/'])
  }

  async selectHost(host: NetWorkPrint) {
    this.isSelectedHost = false;
    this.selectedPort = "";
    const target = new TargetDto();
    target.IP = host.name;
    this.selectedHost = await this.networkApi.getTargetIp(target);
    this.isSelectedHost = true;
    this.isSelectedPort = false;
    if (this.selectedHost.conectionInfo.targetIp && this.selectedHost.conectionInfo.targetIp !== "") {
      this.commonService.SendCustomEvent(host.name);
    } else {
      this.commonService.SendCustomEvent("Pending configuration");
    }
  }

  async saveConfiguration() {
    const saved = await this.networkApi.saveTargetConfig(this.selectedHost);
    if (saved) {
      this._snackBar.open(`Network configuration [${this.selectedHost.conectionInfo.targetIp}] saved!!!`, null, {
        duration: 5000,
        horizontalPosition: "right",
        verticalPosition: "top",
      });
    }
  }

  selectPort(port: NetWorkPrint) {
    if (this.isSelectedHost && !this.commonService.PendingTargetIp()) {
      this.isSelectedPort = true;
      this.selectedPort = `${port.parent}:${port.name}`;
    } else {
      this._snackBar.open("For forwarding a port, first you must select a configured host!", null, {
        duration: 5 * 1000,
        horizontalPosition: "right",
        verticalPosition: "top",
      });
    }
    // this.isSelectedPort = true;
    // this.selectedPort = `${port.parent}:${port.name}`;
    //this.commonService.SendCustomEvent(`${port.parent}:${port.name}`);
  }

  async forwardPort(portNode: NetWorkPrint) {
    if (!this.commonService.PendingTargetIp()) {
      const portRedirection = new PortRedirectionCommandDto();
      portRedirection.TargetIP = this.commonService.lastEvent;
      portRedirection.destinationIp = this.destinationHost;
      portRedirection.destinationPort = this.destinationPort;
      portRedirection.originIp = portNode.parent;
      portRedirection.originPort = portNode.name;
      const forwareded = await this.commandApi.redirectPort(portRedirection);

      if (forwareded) {
        this.destinationHost = "";
        this.destinationPort = "";
        this._snackBar.open(`Port ${portNode.name} forwarded! `, null, {
          duration: 5 * 1000,
          horizontalPosition: "right",
          verticalPosition: "top",
        });
      } else {
        this._snackBar.open(`Error forwarding Port ${portNode.name} :(`, null, {
          duration: 5 * 1000,
          horizontalPosition: "right",
          verticalPosition: "top",
        });
      }
    }
  }

  isThisSelectedPort(port: NetWorkPrint) {
    return this.selectedPort == `${port.parent}:${port.name}`;
  }

  hasChild = (_: number, node: NetWorkPrint) => !!node.netWork && node.netWork.length > 0;
}
