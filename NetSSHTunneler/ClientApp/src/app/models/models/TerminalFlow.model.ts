export class TerminalFlow {
    public results: string[];
    public type: TerminalFlowType;
}

export enum TerminalFlowType {
    ReceivedOk = 0,
    ReceivedError = 1,
    Send = 2
}