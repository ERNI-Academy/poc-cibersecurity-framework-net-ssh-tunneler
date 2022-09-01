import { EventEmitter, Injectable, Output } from "@angular/core";

@Injectable({
  providedIn: "root"
})
export class CommonService {

  @Output() setBase = new EventEmitter<string>();
  @Output() setTarget = new EventEmitter<string>();

  public lastBase: string;
  public lastTarget: string;
  SendBaseEvent(msg: string) {
    this.setBase.emit(msg);
    this.lastBase = msg;
  }
  SendTargetEvent(msg: string) {
    this.setTarget.emit(msg);
    this.lastTarget = msg;
  }

  public PendingTargetIp() {
    return !this.lastBase || this.lastBase == "None" || this.lastBase == "Pending configuration";
  }
}
