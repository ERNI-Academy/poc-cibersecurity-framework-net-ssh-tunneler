export class ConectionInfo {
    targetIp: string;
    targetPort: string;
    userName: string;
    password: string;
    certificate: string;
    additionalSshParameters: string;
}

export class HostInfo {
    network: string;
    parent: string;
    ports: number[];
    conectionInfo: ConectionInfo;
}