import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { throwError } from 'rxjs/internal/observable/throwError';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { catchError, map } from 'rxjs/operators';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  register(model: any) {
    console.log('model', model);
    return this.http
      .post<User>(this.baseUrl + 'accounts/register', model)
      .subscribe((user: User) => {
        console.log('456');
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      });
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'accounts/login', model).pipe(
      map((response: User) => {
        console.log('789');
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user.token));
          //thêm dữ liệu cho observable
          this.currentUserSource.next(user);
        }
      })
    );
  }
  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null!);
  }
}
