
import { Injectable } from "@angular/core";
import { DashboardDataSection } from "../models/models/DashboardData.model";

@Injectable({
    providedIn: "root"
})
export class DashboardDataService {
    public getDashboardData(): DashboardDataSection[] {
        const appSection = new DashboardDataSection();
        appSection.name = "Apps";
        appSection.apps = [
            {
                name: "Network Map",
                icon: "../../assets/network-map.png",
                url: "/network",
                targetNeeded: false
            },
            {
                name: "Run Discovery",
                icon: "../../assets/run-discovery.png",
                url: "/run",
                targetNeeded: true
            },
            {
                name: "SSH",
                icon: "../../assets/ssh.png",
                url: "",
                targetNeeded: true
            }
        ]

        const confSection = new DashboardDataSection();
        confSection.name = "Configuration";
        confSection.apps = [
            {
                name: "Discovery configuration",
                icon: "../../assets/discovery.png",
                url: "/discovery",
                targetNeeded: false
            },
            {
                name: "New target",
                icon: "../../assets/new-target.png",
                url: "/new",
                targetNeeded: false
            },
            {
                name: "Settings",
                icon: "../../assets/settings.png",
                url: "/settings",
                targetNeeded: false
            }
        ]
        return [appSection, confSection]
    }
}