import { Component, OnInit } from '@angular/core';
import { HomeServiceApi } from '../api/home.api';
import { DashboardStatusResponse } from '../models/responses/DashboardStatusResponse.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  public dashboardConfigured: boolean;
  public loading: boolean;
  public loadingMessage: string;
  public dashboardInfo: string;
  public dashboardConfig: DashboardStatusResponse;
  constructor(private homeServiceApi: HomeServiceApi) {
   };
  async ngOnInit() {
    this.loadingMessage = "Loading information...";
    const delay = (ms: number) => new Promise(res => setTimeout(res, ms));
    this.loading = true;
    this.dashboardConfig = await this.homeServiceApi.checkdashboard();
    this.dashboardConfigured = this.dashboardConfig.configured;

    if (this.dashboardConfigured) {
      await delay(300);
      this.loadingMessage = "Configuration detected...";
      await delay(300);
      this.loadingMessage = "Loading dashboard...";
      await delay(300);
      this.dashboardInfo = `Dashboard Configured - IP: ${this.dashboardConfig.targetIp}`
    }

    this.loading = false;
  }

}
