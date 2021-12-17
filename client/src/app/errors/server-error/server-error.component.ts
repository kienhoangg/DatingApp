import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css'],
})
export class ServerErrorComponent implements OnInit {
  error: any;
  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    console.log('nav', navigation);
    this.error = navigation?.extras?.state?.['error'];
    console.log('error', this.error);
  }

  ngOnInit(): void {}
}
