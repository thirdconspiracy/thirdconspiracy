import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  serverElements = [
    {type: 'server', name: 'TestServer1', content: 'my content'},
    {type: 'server', name: 'TestServer2', content: 'my content'}
  ];

}
