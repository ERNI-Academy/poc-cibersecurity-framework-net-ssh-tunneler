import { Component, OnInit } from '@angular/core';
import { CommonService } from '../api/common.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  targetIp: string;
  baseIp: string;

  constructor(private commonService: CommonService) {
    this.targetIp = "None";
    this.baseIp = "None";
  }

  ngOnInit() {
    this.commonService.setBase
    .subscribe((data:string) => {
      this.baseIp = data;
    });
    this.commonService.setTarget
      .subscribe((data: string) => {
        this.targetIp = data;
      });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
