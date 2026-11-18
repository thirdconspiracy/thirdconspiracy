import { Injectable, signal } from '@angular/core';

export interface User {
  name: string;
}

const STORAGE_KEY = 'poker-user';

@Injectable({ providedIn: 'root' })
export class UserSession {
  private readonly _user = signal<User | null>(readStoredUser());
  readonly user = this._user.asReadonly();

  login(name: string): void {
    const trimmed = name.trim();
    if (!trimmed) return;
    const user: User = { name: trimmed };
    localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    this._user.set(user);
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this._user.set(null);
  }
}

function readStoredUser(): User | null {
  try {
    const stored: unknown = JSON.parse(localStorage.getItem(STORAGE_KEY) ?? 'null');
    const name = (stored as Partial<User> | null)?.name;
    return typeof name === 'string' && name.trim() ? { name: name.trim() } : null;
  } catch {
    return null;
  }
}
