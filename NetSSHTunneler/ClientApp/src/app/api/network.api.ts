import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { DiscoveryConfiguration } from "../models/models/DiscoveryConfiguration.model";
import { GeneralConfiguration } from "../models/responses/GeneralConfiguration.model";
import { HostInfo } from "../models/responses/HostInfo.model";
import { NetWorkPrint } from "../models/responses/NetWorkPrint.model";
import { TargetDto } from "../models/responses/TargetDto.model";

@Injectable({
    providedIn: "root"
})
export class NetworkServiceApi {

    constructor(private http: HttpClient, @Inject("API_BASE_URL") private baseUrl: string) {
    }

    async getNetworkMap(): Promise<NetWorkPrint[]> {
        return await this.http.get<NetWorkPrint[]>(this.baseUrl + 'networkmap/getmap').toPromise();
    }

    async getTargetIp(targetIp: TargetDto): Promise<HostInfo> {
        return await this.http.post<HostInfo>(this.baseUrl + 'networkmap/getTarget', targetIp).toPromise();
    }

    async saveTargetConfig(hostInfo: HostInfo): Promise<boolean> {
        return await this.http.post<boolean>(this.baseUrl + 'networkmap/saveTargetConfig', hostInfo).toPromise();
    }

    async getGlobalConfig(): Promise<GeneralConfiguration> {
        return await this.http.get<GeneralConfiguration>(this.baseUrl + 'networkmap/getGlobalConfig').toPromise();
    }

    async saveGlobalConfig(settings: GeneralConfiguration): Promise<boolean> {
        return await this.http.post<boolean>(this.baseUrl + 'networkmap/saveGlobalConfig', settings).toPromise();
    }

    async saveDiscoveryConfig(configuration: DiscoveryConfiguration): Promise<boolean> {
        return await this.http.post<boolean>(this.baseUrl + 'networkmap/saveDiscoveryConfig', configuration).toPromise();
    }

    async getDiscoveryConfigList(): Promise<DiscoveryConfiguration[]> {
        return await this.http.get<DiscoveryConfiguration[]>(this.baseUrl + 'networkmap/getDiscoveryConfigList').toPromise();
    }
}