export class CommandContainer {
    commands: string[];
    interactive: boolean;
    commandConfig: CommandConfig;
    commansLineTransformation: string;
}

export class CommandConfig {
    timeout: number;
    output: Output;
    crack: Crack;
    discovery: Discovery;
}

export class Output {
    saveOutput: boolean;
    filename: string;
}

export class Crack {
    doCrack: boolean;
    hashFile: string;
    dictionary: string;
}

export class Discovery {
    runDiscovery: boolean;
    hostCommand: string;
    networkCommand: string;
    portCommand: string;
}