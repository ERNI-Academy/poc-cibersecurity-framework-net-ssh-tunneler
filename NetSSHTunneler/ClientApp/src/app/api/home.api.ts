import { HttpClient } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { SshConnectionDto } from "../models/dtos/SshConnectionDto.model";
import { ConnectionStatusResponse } from "../models/responses/ConnectionStatusResponse.model";
import { DashboardStatusResponse } from "../models/responses/DashboardStatusResponse.model";

@Injectable({
    providedIn: "root"
})
export class HomeServiceApi {

    constructor(private http: HttpClient, @Inject("API_BASE_URL") private baseUrl: string) {

    }

    async checkdashboard(): Promise<DashboardStatusResponse> {
        return await this.http.get<DashboardStatusResponse>(this.baseUrl + 'home/checkdashboard').toPromise();
    }

    async checkconnection(sshConnectionDto: SshConnectionDto): Promise<ConnectionStatusResponse> {
        // await this.http.post<ConnectionStatusResponse>(this.baseUrl + 'ssh/check', sshConnectionDto).subscribe(result => {
        //     console.log("Connection check: " + result.message + result.status);
        //     return result;
        // }, error => {
        //     const errorResponse = new ConnectionStatusResponse();
        //     errorResponse.status = false;
        //     errorResponse.message = error;
        //     return errorResponse;
        // });

        return await this.http.post<ConnectionStatusResponse>(this.baseUrl + 'home/checkconnection', sshConnectionDto).toPromise();
    }
}