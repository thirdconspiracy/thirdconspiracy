import {Component} from '@angular/core';
import {Router} from '@angular/router';
import {FormBuilder, FormGroup, Validators, ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';

@Component({
 selector: 'app-poker-lobby',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './poker.lobby.html',
  styleUrl: './poker.lobby.css'
})
export  class PokerLobby {
  public inputFieldGroup: FormGroup;

  constructor(private router: Router, private fb: FormBuilder) {
    this.inputFieldGroup = this.fb.group({
      tableName: ['', Validators.required]
    })
  }

  get tableNameHasError() {
    const control = this.inputFieldGroup.get('tableName');
    return control && control.invalid && (control.dirty || control.touched);
  }

  startNewTable() {
    const randomId = Math.floor(100000 + Math.random() * 900000).toString();
    this.router.navigate(['/poker', randomId]);
  }

  joinTable() {
    if(this.inputFieldGroup.valid){
      const tableId = this.inputFieldGroup.value.tableName.trim();
      if (tableId) {
        this.router.navigate(['/poker', tableId]);
      }
    } else {
      this.inputFieldGroup.markAllAsTouched()
    }
  }
}
