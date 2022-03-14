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

  constructor(private commonService: CommonService) {
    this.targetIp = "None";
  }

  ngOnInit() {
    this.commonService.customEvent
    .subscribe((data:string) => {
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
