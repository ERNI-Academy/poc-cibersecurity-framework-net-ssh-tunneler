export class DashboardDataSection {
    name: string
    apps: DashboardDataApp[];
}

export class DashboardDataApp {
    name: string;
    icon: string;
    url: string;
    targetNeeded: boolean;
}