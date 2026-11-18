import { Component } from '@angular/core';
import {FormsModule} from '@angular/forms';
import jsyaml from 'js-yaml';

@Component({
  selector: 'app-data',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './data.html',
  styleUrl: './data.css',
})
export class Data {
  public jsonType: string = "json";
  public yamlType: string = "yaml";
  public dataType: string = this.jsonType;
  public inText: string = "";
  public outText: string = "";

  constructor() {
    const myData = {
      version: '1.0',
      kind: 'yml',
      data: [{
        one: 1,
        msg: "first msg"
      }, {
        two: 1,
        msg: "second msg",
        meta: ["one", "two", "three"]
      }]
    };
    this.inText = JSON.stringify(myData, null, 2);
    this.transform();
  }

  transform() {
    switch (this.dataType) {
      case(this.jsonType):
        this.outText = this.jsonTransform(this.inText);
        break;
      case (this.yamlType):
        this.outText = this.yamlTransform(this.inText);
        break;
    }
  }

  jsonTransform(val: string) {
    return JSON.stringify(JSON.parse(val), null, 2);
  }
  yamlTransform(val: string) {
    return jsyaml.dump(jsyaml.load(val), {indent: 2})
  }

  onClick(val: string) {
    this.dataType = val;
    this.transform();
  }

  onInputChange(event: any){
    this.inText = event.target.value;
  }

  /* TODO:
  * * Add Encoding/Decoding (b64, url, oauth token, etc)
  * * More data formats (xml, cev, etc)
  * * Error Handling for invalid input
  * * Copy to Clipboard
  * * Better styling/layout
  */
}
