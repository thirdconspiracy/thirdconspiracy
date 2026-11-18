import { Injectable } from "@angular/core";
import {BehaviorSubject} from 'rxjs';
import {User} from '../models/user.model';

@Injectable({ providedIn: 'root'})
export class UserService {
  private userSubject = new BehaviorSubject<User | null>(null);
  public user$ = this.userSubject.asObservable();

  constructor() {
    this.loadStoredUser();
  }

  setUser(name: string): void {
    const user: User = {name};
    this.userSubject.next(user);
    localStorage.setItem('poker-user', JSON.stringify(user));
  }

  getCurrentUser(): User | null {
    return this.userSubject.value;
  }

  clearUser(): void {
    this.userSubject.next(null);
    localStorage.removeItem('poker-user');
  }

  private loadStoredUser(): void {
    const storedUser = localStorage.getItem('poker-user');
    if(storedUser) {
      try {
        const user: User = JSON.parse(storedUser);
        this.userSubject.next(user);
      } catch(error) {
        console.error('Error parsing stored user: ', error);
        localStorage.removeItem('poker-user');
      }
    }
  }
}
