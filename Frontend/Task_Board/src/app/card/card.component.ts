import { Component, Input, Output } from '@angular/core';
import { BoardList, Card, TextAndEvent } from '../app.component';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { priorityToString } from '../app.component';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { DropdownComponent } from '../dropdown/dropdown.component';
import { EventEmitter } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [NgIf, NgFor, AsyncPipe, DropdownComponent],
  templateUrl: './card.component.html',
  styleUrl: './card.component.css',
})

export class CardComponent {
  constructor(private api: ApiService) { }
  @Input() card: Card = new Card();
  @Input() lists: BoardList[] = [];

  @Output() updateEvent = new EventEmitter();
  @Output() openCardEvent = new EventEmitter<number>();

  getMoveToTextAndEvent = (): TextAndEvent[] => {
    return this.lists
      .filter(item => item.id != this.card.boardListId)
      .map(item => new TextAndEvent(item.name, async () => { await firstValueFrom(this.moveCard(this.card.id, item.id)); this.updateEvent.emit("") }));
  }

  priorityToString = priorityToString;
  moveCard = this.api.moveCard;
}

