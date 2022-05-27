import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DiscoveryConfiguration } from 'src/app/models/models/DiscoveryConfiguration.model';

@Component({
  selector: 'discovery-configuration-load',
  templateUrl: 'discovery-configuration-load.component.html',
  styleUrls: ['./discovery-configuration-load.component.scss']
})
export class LoadDiscoveryConfigurationDialog {
  selectedConfiguration: DiscoveryConfiguration;
  constructor(
    public dialogRef: MatDialogRef<LoadDiscoveryConfigurationDialog>,
    @Inject(MAT_DIALOG_DATA) public data) { 
      console.log(data.discoveryConfigurationList);
    }

    onLoad() {
      this.dialogRef.close(this.selectedConfiguration);
    }
}
