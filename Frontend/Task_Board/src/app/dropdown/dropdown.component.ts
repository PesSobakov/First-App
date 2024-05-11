import { NgFor, NgIf } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TextAndEvent } from '../app.component';

@Component({
  selector: 'app-dropdown',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf],
  templateUrl: './dropdown.component.html',
  styleUrl: './dropdown.component.css'
})
export class DropdownComponent {
  @Input() textAndEvent: TextAndEvent[] = [];
   isOpen: boolean = false;
}
