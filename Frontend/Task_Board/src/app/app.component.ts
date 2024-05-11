import { Component, Output } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DropdownComponent } from './dropdown/dropdown.component';
import { CardComponent } from './card/card.component';
import { ListComponent } from './list/list.component';
import { OpenedCardComponent } from './opened-card/opened-card.component';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Observable, firstValueFrom } from 'rxjs';
import { ApiService } from './api.service';
import { FormsModule } from '@angular/forms';
import { DialogComponent } from './dialog/dialog.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [DropdownComponent, CardComponent, ListComponent, OpenedCardComponent, NgFor, NgIf, AsyncPipe, FormsModule, DialogComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent {
  constructor(private api: ApiService) { }

  title = 'Task_Board';

  card: Card = new Card;
  card$: Observable<Card[]> = this.api.getCards();
  list$: Observable<BoardList[]> = this.api.getLists();

  update = () => {
    this.card$ = this.api.getCards();
    this.list$ = this.api.getLists();
  }

  openCard = async (id: number) => {
    this.openedCard = (await this.card$.toPromise())?.find(item => item.id == id);
    this.isCardOpened = true;
    this.openedCardId = id;
  }

  createList = async () => { await firstValueFrom(this.api.createList(this.newList)); this.update() };
  listCreating = false;
  newList = '';

  isCardOpened = false;
  openedCardId = 0;
  openedCard?: Card = new Card();
}

export function priorityToString(number: number): string {
  switch (number) {
    case 0: return 'Low';
    case 1: return 'Medium';
    case 2: return 'High';
    default: return '';
  }
}

export class Card {
  constructor(id?: number, name?: string, description?: string, dueDate?: Date, priority?: number, listId?: number) {
    this.id = id ?? 0;
    this.name = name ?? '';
    this.description = description ?? '';
    this.dueDate = dueDate ?? new Date(Date.now());
    this.priority = priority ?? 0;
    this.boardListId = listId ?? 0;
  }

  id: number;
  name: string;
  description: string;
  dueDate: Date;
  priority: number;
  boardListId: number;
}

export class BoardList {
  constructor(id?: number, name?: string) {
    this.id = id ?? 0;
    this.name = name ?? '';
  }

  id: number;
  name: string;
}

export class TextAndEvent {
  constructor(text: string, event: Function) {
    this.text = text;
    this.event = event;
  }
  text;
  event;
}