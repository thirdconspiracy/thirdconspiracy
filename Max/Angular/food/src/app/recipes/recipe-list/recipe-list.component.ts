import { Component, OnInit } from '@angular/core';
import { Recipe } from '../recipe.model';

@Component({
  selector: 'app-recipe-list',
  templateUrl: './recipe-list.component.html',
  styleUrls: ['./recipe-list.component.css']
})
export class RecipeListComponent implements OnInit {
  recipes: Recipe[] = [
    new Recipe('Pizza Test 1', 'This is simply a test', 'http://normalcooking.com/wp-content/uploads/2014/04/Mini-Biscuit-Pizza-1.jpg'),
    new Recipe('Pizza Test 2', 'This is simply a test', 'http://normalcooking.com/wp-content/uploads/2014/04/Mini-Biscuit-Pizza-1.jpg'),
  ];

  constructor() { }

  ngOnInit() {
  }

}
