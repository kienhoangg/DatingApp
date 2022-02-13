import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Roles } from 'src/app/_models/roles';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css'],
})
export class RolesModalComponent implements OnInit {
  @Input() updateSelectedRoles = new EventEmitter();
  user: User;
  roles: Roles[];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}
  updateRoles() {
    debugger;
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }
}
