import { Component, inject } from '@angular/core';
import { UserSession} from './user-session';

@Component({
  selector: 'app-user-menu',
  template: `
    @if (session.user(); as user) {
      <span class="user">
        Hi {{ user.name }}
        <button type="button" class="btn-soft" (click)="session.logout()">Logout</button>
      </span>
    }
  `,
  styles: `
    :host {
      display: contents;
    }

    .user {
      align-self: center;
    }
  `
})
export class UserMenu {
  protected readonly session = inject(UserSession);
}
