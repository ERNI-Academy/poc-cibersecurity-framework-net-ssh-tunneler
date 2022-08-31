import { HttpClient } from "@angular/common/http";
import { EventEmitter, Inject, Injectable, Output } from "@angular/core";
import { NetWorkPrint } from "../models/responses/NetWorkPrint.model";

@Injectable({
    providedIn: "root"
})
export class CommonService {

  @Output() setBase = new EventEmitter<string>();
  @Output() setTarget = new EventEmitter<string>();
    public lastEvent: string;
    SendBaseEvent(msg: string) {
    this.setBase.emit(msg);
        this.lastEvent = msg;
    }
    SendTargetEvent(msg: string) {
      this.setTarget.emit(msg);
    this.lastEvent = msg;
    }

    public PendingTargetIp() {
        return !this.lastEvent || this.lastEvent == "None" || this.lastEvent == "Pending configuration";
    }
}
