import { Injectable } from '@angular/core';
import { environment } from './../environments/environment';


import { BrowserModule } from "@angular/platform-browser";
import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";
import { ReactiveFormsModule, FormControl, FormsModule } from "@angular/forms";

import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { BoardList, Card } from './app.component';


import {
  map,
  debounceTime,
  distinctUntilChanged,
  switchMap,
  tap
} from "rxjs/operators";



@Injectable({
  providedIn: 'root'
})

@Injectable()
export class ApiService {
  readonly endpoint: string = `http://${environment.server}:${environment.port}/`;

  constructor(private http: HttpClient) { }

  getCards(): Observable<Card[]> {
    let url = `${this.endpoint}api/Cards`;
    return this.http
      .get<Card[]>(url)
      .pipe(map(res => {
        return res.map(item => {
          return new Card(
            item.id,
            item.name,
            item.description,
            new Date(item.dueDate),
            item.priority,
            item.boardListId
          );
        })
      }))

      .pipe(catchError(this.handleError<Card[]>('cards', [])));
  }

  moveCard = (cardId: number, listId: number) => {
    let url = `${this.endpoint}api/Cards/${cardId}`;
    return this.http
      .patch(url, { boardListId: listId });
    //.pipe(catchError(this.handleError<Card[]>('cards', [])));
  }

  getLists(): Observable<BoardList[]> {
    let url = `${this.endpoint}api/Lists`;
    return this.http
      .get<BoardList[]>(url)
      .pipe(map(res => {
        return res.map(item => {
          return new BoardList(
            item.id,
            item.name
          );
        })
      }))
      .pipe(catchError(this.handleError<Card[]>('cards', [])));
  }

  createList = (name:string) => {
    let url = `${this.endpoint}api/Lists`;
    return this.http
      .post(url,{Name : name});
  }

renameList =(listId: number,name:string) => {
  let url = `${this.endpoint}api/Lists/${listId}`;
  return this.http
    .patch(url,{Name : name});
}

  deleteList = (listId: number) => {
    let url = `${this.endpoint}api/Lists/${listId}`;
    return this.http
      .delete(url);
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.log(`failed: ${error.message}`);
      return of(result as T);
    };
  }
}
