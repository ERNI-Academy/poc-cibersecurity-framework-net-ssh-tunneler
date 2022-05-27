import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NetworkServiceApi } from '../api/network.api';
import { NetWorkPrint } from '../models/responses/NetWorkPrint.model';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { CommonService } from '../api/common.service';
import { TargetDto } from '../models/responses/TargetDto.model';
import { ConectionInfo, HostInfo } from '../models/responses/HostInfo.model';
import { GeneralConfiguration } from '../models/responses/GeneralConfiguration.model';
import { DiscoveryConfiguration } from '../models/models/DiscoveryConfiguration.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommandServiceApi } from '../api/command.api';
import { ProcessDiscoveryDto } from '../models/dtos/ProcessDiscoveryDto.model';
import { DiscoveryResults } from '../models/responses/DiscoveryResponse.model';

@Component({
  selector: 'app-run-discovery',
  templateUrl: './run-discovery.component.html',
  styleUrls: ['./run-discovery.component.scss']
})
export class RunDiscoveryComponent implements OnInit {

  selectedDiscoveryConfiguration: DiscoveryConfiguration;
  discoveryConfigurationsList: DiscoveryConfiguration[];
  settings: GeneralConfiguration;
  content: string;
  loading: boolean;
  receivedContent: boolean;
  loadingMsg: string;
  discoveryResults: DiscoveryResults[];
  constructor(private networkApi: NetworkServiceApi, private commandApi: CommandServiceApi, private router: Router, private commonService: CommonService, private _snackBar: MatSnackBar) {
  };

  async ngOnInit() {
    this.discoveryResults = new Array<DiscoveryResults>();
    this.receivedContent = false;
    this.loading = false;
    this.content = "Please, launch a discovery configuration to start...";
    this.settings = new GeneralConfiguration();
    this.settings = await this.networkApi.getGlobalConfig();
    this.discoveryConfigurationsList = await this.networkApi.getDiscoveryConfigList();
  }

  goBack() {
    this.router.navigate(['/'])
  }

  async onLaunch() {
    if (this.commonService.PendingTargetIp()) {
      let msg = "No host selected. You must go to Network Map and select one host.";
      if (this.commonService.lastEvent == "Pending configuration") {
        msg = "The selected host is not configured! You must go to Network Map and configure it or select another one."
      }
      this._snackBar.open(msg, null, {
        duration: 5 * 1000,
        horizontalPosition: "right",
        verticalPosition: "top",
      });
    }
    else {
      if (this.selectedDiscoveryConfiguration) {
        this.loadingMsg = "Executing actions. Please wait until complete to see the report..."
        this.loading = true;
        const discoveryDto = new ProcessDiscoveryDto();
        discoveryDto.TargetIp = this.commonService.lastEvent;
        discoveryDto.commands = new Array<string>();
        this.selectedDiscoveryConfiguration.files.forEach(file => {
          discoveryDto.commands.push(file.path);
        })
        this.discoveryResults = await this.commandApi.processDiscovery(discoveryDto);
        this.loading = false;
        if (this.discoveryResults) {
          this.receivedContent = true;
        } else {
          this.receivedContent = false;
          this.content = "Error."
        }
      } else {
        this._snackBar.open("Please, you must select a discovery configuration to launch", null, {
          duration: 5 * 1000,
          horizontalPosition: "right",
          verticalPosition: "top",
        });
      }
    }
  }
}
