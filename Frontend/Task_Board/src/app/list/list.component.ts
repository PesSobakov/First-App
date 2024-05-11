import { Component, Input, Output } from '@angular/core';
import { CardComponent } from '../card/card.component';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { Observable, firstValueFrom } from 'rxjs';
import { BoardList, Card, TextAndEvent } from '../app.component';
import { EventEmitter } from '@angular/core';
import { DropdownComponent } from '../dropdown/dropdown.component';
import { ApiService } from '../api.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CardComponent, NgFor, AsyncPipe, NgIf, DropdownComponent,FormsModule],
  templateUrl: './list.component.html',
  styleUrl: './list.component.css'
})

export class ListComponent {
  constructor(private api: ApiService) { }
  @Input() cards: Card[] = [];
  @Input() lists: BoardList[] = [];
  @Input() list: BoardList = new (BoardList);
  @Output() updateEvent = new EventEmitter();
  @Output() OpenCardEvent = new EventEmitter<number>();

  count = () => { return this.cards.filter((item) => { return item.boardListId == this.list.id }).length };
  menu: TextAndEvent[] = [new TextAndEvent('<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-6 h-6">  <path stroke-linecap="round" stroke-linejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L10.582 16.07a4.5 4.5 0 0 1-1.897 1.13L6 18l.8-2.685a4.5 4.5 0 0 1 1.13-1.897l8.932-8.931Zm0 0L19.5 7.125M18 14v4.75A2.25 2.25 0 0 1 15.75 21H5.25A2.25 2.25 0 0 1 3 18.75V8.25A2.25 2.25 0 0 1 5.25 6H10" /></svg>Edit', () => { }), new TextAndEvent('Delete', () => { })];
  menuOpen: boolean = false;
  isEdit: boolean = false;
  deleteList = async () => { await firstValueFrom(this.api.deleteList(this.list.id)); this.updateEvent.emit('') };
  listRenaming = false;
  newName ='';
  renameList = async () => { await firstValueFrom(this.api.renameList(this.list.id,this.newName)); this.updateEvent.emit('') };
}
