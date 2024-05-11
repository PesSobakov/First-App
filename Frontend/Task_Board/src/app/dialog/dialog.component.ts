import { NgIf } from '@angular/common';
import {
  Component,
  OnInit,
  Input,
  Output,
  OnChanges,
  EventEmitter
} from '@angular/core';

@Component({
  selector: 'app-dialog',
  standalone: true,
  imports: [NgIf],
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.css'
})

export class DialogComponent implements OnInit {
  @Input() closable = true;
  @Input() visible: boolean = false;
  @Output() visibleChange: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() {}

  ngOnInit() {}

  close() {
    this.visible = false;
    this.visibleChange.emit(this.visible);
  }
}




