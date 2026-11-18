import { Component } from '@angular/core';
import { CommonModule} from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule} from '@angular/forms';
import { UserService} from './user.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  loginForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private userService: UserService
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  get usernameHasError(): boolean {
    const control = this.loginForm.get('username');
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onLogin(): void {
    if(this.loginForm.valid) {
      const username = this.loginForm.value.username.trim();
      if (username) {
        this.userService.setUser(username);
      }
    } else {
      this.loginForm.markAllAsTouched();
    }
  }
}
