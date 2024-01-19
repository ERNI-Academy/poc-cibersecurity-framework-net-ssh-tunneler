import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CommandServiceApi } from '../api/command.api';
import { CommonService } from '../api/common.service';
import { CommandDto } from '../models/dtos/CommandDto.model';
import { TerminalFlow, TerminalFlowType } from '../models/models/TerminalFlow.model';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import * as signalR from '@microsoft/signalr';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-ssh-terminal',
  templateUrl: './ssh-terminal.component.html',
  styleUrls: ['./ssh-terminal.component.scss']
})
export class SshTerminalComponent implements OnInit {
  @Output()
  public closeEvent = new EventEmitter<boolean>();
  public responses: TerminalFlow[];
  @ViewChild('command') myDiv: ElementRef;
  dragPosition = { x: 0, y: 0 };
  public path: string;
  private connection: HubConnection;

  public userName = 'Pepe';
  public groupName = 'Events';
  public messageToSend = '';
  public joined = false;
  public conversation: NewMessage[] = [{
    message: 'Bienvenido',
    userName: 'Sistema'
  }];

  constructor(private commandApi: CommandServiceApi, public dialog: MatDialog, private commonService: CommonService, private _snackBar: MatSnackBar) {
    this.dragPosition = { x: -0, y: -200 };
    console.log("Connecting to events hub");
    this.connection = new HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Debug)
      .withUrl(`http://localhost:5070/events`)
      .build();

      this.connection.on("NewUser", message => this.newUser(message));
      this.connection.on("NewMessage", message => this.newMessage(message));
      this.connection.on("LeftUser", message => this.leftUser(message));
  }

  async ngOnInit() {
    this.responses = [];
    this.path = "$";
    if (this.commonService.lastBase && this.commonService.lastBase !== "") {
      const initialCommand = new CommandDto();
      initialCommand.Command = "echo Sesión iniciada";
      initialCommand.TargetIp = this.commonService.lastBase;
      if (this.commonService.lastTarget && this.commonService.lastTarget !== "") {
        initialCommand.AttackedIp = this.commonService.lastTarget;
      }
      else {
        initialCommand.AttackedIp = this.commonService.lastBase;
      }
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

    this.connection.start()
      .then(_ => {
        console.log('Connection Started');
        this.join();
      }).catch(error => {
        return console.error(error);
      });
  }

  public join() {
    this.connection.invoke('JoinGroup', this.groupName, this.userName)
      .then(_ => {
        this.joined = true;
      });
  }

  private newUser(message: string) {
    this.conversation.push({
      userName: 'Sistema',
      message: message
    });
  }

  private newMessage(message: NewMessage) {
    // console.log(message);
    // this._snackBar.open(message.message, null);
    // this.conversation.push(message);
    console.log(message);
    const newTerminalFlow = new TerminalFlow();
      newTerminalFlow.results = [];
      newTerminalFlow.results.push(message.message);
      // this.path = response.path;
      newTerminalFlow.type = TerminalFlowType.ReceivedOk;
      this.responses.push(newTerminalFlow);
  }

  private leftUser(message: string) {
    console.log(message);
    this.conversation.push({
      userName: 'Sistema',
      message: message
    });
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
      commandToSend.TargetIp = this.commonService.lastBase;
      commandToSend.AttackedIp = this.commonService.lastTarget;
       await this.commandApi.sendCommand(commandToSend);
      // const newTerminalFlow = new TerminalFlow();
      // newTerminalFlow.results = [];
      // response.results.forEach(result => {
      //   console.log(result);
      //   newTerminalFlow.results.push(result);
      // });
      // this.path = response.path;
      // if (response.error) {
      //   newTerminalFlow.type = TerminalFlowType.ReceivedError;
      // } else {
      //   newTerminalFlow.type = TerminalFlowType.ReceivedOk;
      // }
      // this.responses.push(newTerminalFlow);
    }
  }

  onSshTerminalClose() {
    this.closeEvent.emit(true);
    this.dragPosition = { x: -0, y: -200 };
  }
}


interface NewMessage {
  userName: string;
  message: string;
  groupName?: string;
}