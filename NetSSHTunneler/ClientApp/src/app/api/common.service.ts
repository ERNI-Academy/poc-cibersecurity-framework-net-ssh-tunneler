import { HttpClient } from "@angular/common/http";
import { EventEmitter, Inject, Injectable, Output } from "@angular/core";
import { NetWorkPrint } from "../models/responses/NetWorkPrint.model";

@Injectable({
    providedIn: "root"
})
export class CommonService {

    @Output() customEvent = new EventEmitter<string>();
    public lastEvent: string;
    SendCustomEvent(msg: string) {
        this.customEvent.emit(msg);
        this.lastEvent = msg;
    }

    public PendingTargetIp() {
        return !this.lastEvent || this.lastEvent == "None" || this.lastEvent == "Pending configuration";
    }
}