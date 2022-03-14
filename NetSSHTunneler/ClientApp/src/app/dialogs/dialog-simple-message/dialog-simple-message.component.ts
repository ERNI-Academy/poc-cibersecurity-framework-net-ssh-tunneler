import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'dialog-simple-message',
  templateUrl: 'dialog-simple-message.component.html',
  styleUrls: ['./dialog-simple-message.component.scss']
})
export class DialogSimpleMessage {
  constructor(
    public dialogRef: MatDialogRef<DialogSimpleMessage>,
    @Inject(MAT_DIALOG_DATA) public data) { }
}