import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { Component, OnInit } from '@angular/core';
import { MatDialog, MatSnackBar } from '@angular/material';
import { Router } from '@angular/router';
import { CommandServiceApi } from '../api/command.api';
import { NetworkServiceApi } from '../api/network.api';
import { LoadDiscoveryConfigurationDialog } from '../dialogs/discovery-configuration-load/discovery-configuration-load.component';
import { CommandConfig, CommandContainer, Crack, Discovery, Output } from '../models/models/CommandContainer.model';
import { DiscoveryConfiguration, FileItem } from '../models/models/DiscoveryConfiguration.model';
import { DiscoveryScriptFile, DiscoveryScriptFolder } from '../models/responses/DiscoveryResponse.model';

@Component({
  selector: 'app-discovery',
  templateUrl: './discovery.component.html',
  styleUrls: ['./discovery.component.scss']
})
export class DiscoveryComponent implements OnInit {

  // done = ['Get up', 'Brush teeth', 'Take a shower', 'Check e-mail', 'Walk dog'];
  configuredScripts: DiscoveryScriptFile[];
  discoveryFolders: DiscoveryScriptFolder[];
  preview: DiscoveryScriptFile;
  isNewCommand: boolean;
  color = "#41ff00";
  durationInSeconds = 5;
  filename: string;
  constructor(private commandApi: CommandServiceApi, private networkApi: NetworkServiceApi, private router: Router, public dialog: MatDialog, private _snackBar: MatSnackBar) {
  };

  async ngOnInit() {
    this.filename = "";
    this.newPreviewItem("New command");
    // this.preview.content = "The content will be displayed here."
    this.configuredScripts = [];
    await this.getCommandList();
  }

  goBack() {
    this.router.navigate(['/'])
  }

  newPreviewItem(name: string) {
    this.preview = new DiscoveryScriptFile();
    this.preview.name = name;
    this.preview.content = new CommandContainer();
    this.preview.content.commands = [""];
    this.preview.content.commandConfig = new CommandConfig();
    this.preview.content.commandConfig.output = new Output();
    this.preview.content.commandConfig.discovery = new Discovery();
    this.preview.content.commandConfig.crack = new Crack();
    this.isNewCommand = true;
  }

  async getCommandList() {
    this.discoveryFolders = [];
    this.discoveryFolders = await this.commandApi.getDiscovery();
    if (this.discoveryFolders) {
      this.discoveryFolders.forEach(df => {
        df.scripts.forEach(script => {
          script.content.commansLineTransformation = script.content.commands.join("^");
        })
      });
    }
  }

  async onSaveConfiguration() {
    let valid = true;
    let msg = "";

    if (this.filename == "") {
      valid = false;
      msg = "Please, you must add a name for saving the discovery configuration.";
    }

    if (this.configuredScripts.length == 0) {
      valid = false;
      msg = "Please, you must add at least one item to the discovery configuration.";
    }

    if (valid) {
      let discoveryConfiguration = new DiscoveryConfiguration();
      discoveryConfiguration.configurationName = this.filename;
      discoveryConfiguration.files = new Array<FileItem>();
      this.configuredScripts.forEach(script => {
        const newFileItem = new FileItem();
        newFileItem.name = script.name;
        newFileItem.path = script.path;
        discoveryConfiguration.files.push(newFileItem);
      })
      const saved = await this.networkApi.saveDiscoveryConfig(discoveryConfiguration);

      if (saved) {
        msg = `Configuration ${this.filename} saved!`
      } else {
        msg = `Error saving configuration ${this.filename} :(`
      }
    }

    this._snackBar.open(msg, null, {
      duration: this.durationInSeconds * 1000,
      horizontalPosition: "right",
      verticalPosition: "top",
    });
  }

  async onLoadConfiguration() {
    const discoveryConfigurationList = await this.networkApi.getDiscoveryConfigList();
    const loadConfDialog = this.dialog.open(LoadDiscoveryConfigurationDialog, {
      data: { title: "Load", discoveryConfigurationList: discoveryConfigurationList }
    });

    loadConfDialog.afterClosed().subscribe(result => {
      if (result) {
        this.configuredScripts = [];
        this.filename = result.configurationName;
        result.files.forEach((command: FileItem) => {
          const discoveryFile = new DiscoveryScriptFile();
          discoveryFile.name = command.name;
          discoveryFile.path = command.path;
          this.configuredScripts.push(discoveryFile);
        });
      }
    });
  }

  newCommand() {
    this.newPreviewItem("New command");
  }

  drop(event: CdkDragDrop<string[]>) {
    console.log(event);
    if (event.item.data) {
      this.configuredScripts.push(event.item.data);
    }
    moveItemInArray(this.configuredScripts, event.previousIndex, event.currentIndex);
  }

  async onSave() {
    this.preview.content.commands = this.preview.content.commansLineTransformation.split("^");
    const saved = await this.commandApi.saveDiscoveryFile(this.preview);
    if (saved) {
      this._snackBar.open("Command saved!", null, {
        duration: 5 * 1000,
        horizontalPosition: "right",
        verticalPosition: "top",
      });
      this.getCommandList();
    } else {
      this._snackBar.open("Error saving the command :(", null, {
        duration: 5 * 1000,
        horizontalPosition: "right",
        verticalPosition: "top",
      });
    }
  }

  deleteScriptItem(index: number) {
    console.log(index);
    this.configuredScripts.splice(index, 1)
  }

  onPreview(script: DiscoveryScriptFile) {
    this.preview = script;
    this.isNewCommand = false;
  }
}
