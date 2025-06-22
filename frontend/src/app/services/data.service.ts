import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private apiUrl = 'https://localhost:44330/api/Persons';

  constructor(private http: HttpClient) { }

  getPersons(pageNumber: number = 1, pageSize: number = 10, search?: string): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    if (search && search.trim()) {
      params = params.set('search', search.trim());
    }

    return this.http.get<any>(this.apiUrl, { params })
      .pipe(
        catchError(this.handleError)
      );
  }

  createPerson(request: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, request)
      .pipe(
        catchError(this.handleError)
      );
  }

  updatePerson(id: number, request: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, request)
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'เกิดข้อผิดพลาดในการเชื่อมต่อ';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      // Server-side error
      if (error.error && error.error.message) {
        errorMessage = error.error.message;
      } else if (error.status === 0) {
        errorMessage = 'ไม่สามารถเชื่อมต่อกับเซิร์ฟเวอร์ได้';
      } else {
        errorMessage = `Server Error: ${error.status} - ${error.message}`;
      }
    }

    console.error('API Error:', error);
    return throwError(() => errorMessage);
  }

  // getAll(): Observable<any[]> {
  //   return this.http.get<any[]>(this.apiUrl);
  // }

  // getById(id: number): Observable<any> {
  //   return this.http.get<any>(`${this.apiUrl}/${id}`)
  // }

  // create(record: any): Observable<any> {
  //   return this.http.post<any>(this.apiUrl, record);
  // }

  // update(id: number, record: any): Observable<any> {
  //   return this.http.put<any>(`${this.apiUrl}/${id}`, record);
  // }

  // delete(id: number): Observable<any> {
  //   return this.http.delete<any>(`${this.apiUrl}/${id}`);
  // }
}
