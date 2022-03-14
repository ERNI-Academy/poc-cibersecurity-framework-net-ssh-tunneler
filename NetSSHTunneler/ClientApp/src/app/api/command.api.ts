import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { CommandDto } from "../models/dtos/CommandDto.model";
import { PortRedirectionCommandDto } from "../models/dtos/PortRedirectionCommandDto.model";
import { ProcessDiscoveryDto } from "../models/dtos/ProcessDiscoveryDto.model";
import { CommandResponse } from "../models/responses/CommandResponse.model";
import { DiscoveryResults, DiscoveryScriptFile, DiscoveryScriptFolder } from "../models/responses/DiscoveryResponse.model";

@Injectable({
    providedIn: "root"
})
export class CommandServiceApi {

    constructor(private http: HttpClient, @Inject("API_BASE_URL") private baseUrl: string) {

    }

    async sendCommand(command: CommandDto): Promise<CommandResponse> {
        console.log("send to target: " + command.TargetIp)
        return await this.http.post<CommandResponse>(this.baseUrl + 'command/send/', command).toPromise();
    }

    async getDiscovery(): Promise<DiscoveryScriptFolder[]> {
        return await this.http.get<DiscoveryScriptFolder[]>(this.baseUrl + 'command/getDiscoveryScripts/').toPromise();
    }

    async saveDiscoveryFile(scriptFile: DiscoveryScriptFile): Promise<boolean> {
        return await this.http.post<boolean>(this.baseUrl + 'command/saveDiscoveryScript/', scriptFile).toPromise();
    }

    async newDiscoveryScript(scriptFile: DiscoveryScriptFile): Promise<void[]> {
        return await this.http.post<void[]>(this.baseUrl + 'command/newDiscoveryScript/', scriptFile).toPromise();
    }

    async processDiscovery(processDiscovery: ProcessDiscoveryDto): Promise<DiscoveryResults[]> {
        return await this.http.post<DiscoveryResults[]>(this.baseUrl + 'command/processDiscovery/', processDiscovery).toPromise();
    }

    async redirectPort(processDiscovery: PortRedirectionCommandDto): Promise<boolean> {
        return await this.http.post<boolean>(this.baseUrl + 'command/redirectPort/', processDiscovery).toPromise();
    }
}