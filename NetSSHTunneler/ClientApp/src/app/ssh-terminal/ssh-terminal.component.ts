import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CommandServiceApi } from '../api/command.api';
import { CommonService } from '../api/common.service';
import { CommandDto } from '../models/dtos/CommandDto.model';
import { TerminalFlow, TerminalFlowType } from '../models/models/TerminalFlow.model';

@Component({
  selector: 'app-ssh-terminal',
  templateUrl: './ssh-terminal.component.html',
  styleUrls: ['./ssh-terminal.component.scss']
})
export class SshTerminalComponent implements OnInit {
  @Output()
  public closeEvent = new EventEmitter<boolean>();
  public responses: TerminalFlow[];
  @ViewChild('command', null) myDiv: ElementRef;
  dragPosition = { x: 0, y: 0 };
  public path: string;
  constructor(private commandApi: CommandServiceApi, public dialog: MatDialog, private commonService: CommonService) {
    this.dragPosition = { x: -0, y: -200 };
  }

  async ngOnInit() {
    this.responses = [];
    this.path = "$";
    if (this.commonService.lastEvent && this.commonService.lastEvent !== "") {
      const initialCommand = new CommandDto();
      initialCommand.Command = "echo Sesión iniciada";
      initialCommand.TargetIp = this.commonService.lastEvent;
      const response = await this.commandApi.sendCommand(initialCommand);
      const newTerminalFlow = new TerminalFlow();
      newTerminalFlow.results = [];
      response.results.forEach(result => {
        newTerminalFlow.results.push(result);
      });
      this.path = response.path;

      if (response.error) {
        newTerminalFlow.type = TerminalFlowType.ReceivedError;
      } else {
        newTerminalFlow.type = TerminalFlowType.ReceivedOk;
      }
      this.responses.push(newTerminalFlow);
    }
  }

  async sendCommand() {
    const command = this.myDiv.nativeElement.innerText;
    this.myDiv.nativeElement.innerText = "";
    const newTerminalFlowSend = new TerminalFlow();
    newTerminalFlowSend.results = [];
    newTerminalFlowSend.results.push(`${this.path} ${command}`)
    newTerminalFlowSend.type = TerminalFlowType.Send;
    this.responses.push(newTerminalFlowSend);
    if (command === "clear\n\n") {
      this.responses = new Array<TerminalFlow>();
    } else {
      const commandToSend = new CommandDto();
      commandToSend.Command = command;
      commandToSend.TargetIp =  this.commonService.lastEvent;
      const response = await this.commandApi.sendCommand(commandToSend);
      const newTerminalFlow = new TerminalFlow();
      newTerminalFlow.results = [];
      response.results.forEach(result => {
        console.log(result);
        newTerminalFlow.results.push(result);
      });
      this.path = response.path;
      if (response.error) {
        newTerminalFlow.type = TerminalFlowType.ReceivedError;
      } else {
        newTerminalFlow.type = TerminalFlowType.ReceivedOk;
      }
      this.responses.push(newTerminalFlow);
    }
  }

  onSshTerminalClose() {
    this.closeEvent.emit(true);
    this.dragPosition = { x: -0, y: -200 };
  }
}
