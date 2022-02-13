import { environment } from './../../environments/environment';
import { HttpClient, HttpBackend } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { throwError } from 'rxjs/internal/observable/throwError';
import { ReplaySubject } from 'rxjs/internal/ReplaySubject';
import { catchError, map } from 'rxjs/operators';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, handler: HttpBackend) {
    this.http = new HttpClient(handler);
  }

  register(model: any) {
    console.log('model', model);
    return this.http.post<User>(this.baseUrl + 'accounts/register', model).pipe(
      map((user: User) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'accounts/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }
  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user));
    //thêm dữ liệu cho observable
    this.currentUserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null!);
  }

  getDecodedToken(token: string) {
    var a = token.split('.')[1];
    var b = atob(token.split('.')[1]);
    return JSON.parse(atob(token.split('.')[1]));
  }
}
