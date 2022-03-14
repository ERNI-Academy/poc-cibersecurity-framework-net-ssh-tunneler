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

@Component({
  selector: 'app-settings-screen',
  templateUrl: './settings-screen.component.html',
  styleUrls: ['./settings-screen.component.scss']
})
export class SettingsScreenComponent implements OnInit {

  selectedCracker: string;
  crackerPrograms: string[] = ['Jon The Ripper', 'HashCat'];
  settings: GeneralConfiguration;
  constructor(private networkApi: NetworkServiceApi, private router: Router, private commonService: CommonService) {
  };

  async ngOnInit() {
    this.settings = new GeneralConfiguration();
    this.settings = await this.networkApi.getGlobalConfig();
    this.transformCrackerValue();
  }

  goBack() {
    this.router.navigate(['/'])
  }

  async onSave() {
    this.settings.cracker = this.crackerPrograms.findIndex(x => x === this.selectedCracker);
    await this.networkApi.saveGlobalConfig(this.settings);
  }

  transformCrackerValue() {
    this.selectedCracker = this.crackerPrograms[this.settings.cracker];
  }
}
