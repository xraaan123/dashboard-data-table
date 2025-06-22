import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { DataService } from 'src/app/services/data.service';
import { DataFormComponent } from '../data-form/data-form.component';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  selector: 'app-data-table',
  templateUrl: './data-table.component.html',
  styleUrls: ['./data-table.component.scss'],
})
export class DataTableComponent implements OnInit {
  displayedColumns: string[] = [
    'id',
    'fullName',
    'address',
    'birthDate',
    'age',
    'actions',
  ];

  dataSource = new MatTableDataSource<any>([]);
  isLoading = false;

  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;
  pageSizeOptions = [5, 10, 20, 50];

  searchTerm = '';
  private searchSubject = new Subject<string>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;


  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private dataService: DataService,
  ) {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(searchTerm => {
      this.searchTerm = searchTerm;
      this.pageNumber = 1;
      this.loadData();
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      if (this.paginator) {
        this.paginator.pageSize = this.pageSize;
        this.paginator.pageIndex = this.pageNumber - 1;

        this.paginator.page.subscribe((event: PageEvent) => {
          this.onPageChange(event);
        });
      }

      if (this.sort) {
        this.dataSource.sort = this.sort;
      }
    })
  }

  loadData(): void {
    this.isLoading = true;

    this.dataService.getPersons(this.pageNumber, this.pageSize, this.searchTerm)
      .subscribe({
        next: (response: any) => {
          if (response.success && response.data) {
            this.dataSource.data = response.data.data;
            this.totalCount = response.data.totalCount;

            if (this.paginator) {
              this.paginator.length = this.totalCount;
              this.paginator.pageSize = this.pageSize;
              this.paginator.pageIndex = this.pageNumber - 1;
            }
          } else {
            this.showSnackBar(response.message || 'ไม่สามารถโหลดข้อมูลได้');
          }
          this.isLoading = false;
        },
        error: (error: any) => {
          console.error('Error loading data:', error);
          this.showSnackBar(error);
          this.isLoading = false;
        }
      });
  }

  calculateAge(birthDate: Date): number {
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();

    if (
      monthDiff < 0 ||
      (monthDiff === 0 && today.getDate() < birthDate.getDate())
    ) {
      age--;
    }

    return age;
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.searchSubject.next(filterValue);
  }

  onPageChange(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    this.loadData();
  }

  viewRecord(record: any): void {
    const dialogRef = this.dialog.open(DataFormComponent, {
      width: '70%',
      data: {
        isView: true,
        record: record
      }
    })
  }

  addRecord(): void {
    const dialogRef = this.dialog.open(DataFormComponent, {
      width: '70%',
      data: { isEdit: false },
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const age = this.calculateAge(new Date(result.birthDate));

        const createRequest = {
          firstName: result.firstName,
          lastName: result.lastName,
          address: result.address,
          birthDate: result.birthDate,
          age: age
        };

        this.dataService.createPerson(createRequest).subscribe({
          next: (response: any) => {
            if (response.success) {
              this.showSnackBar(response.message || 'เพิ่มข้อมูลสำเร็จ');
              this.loadData();
            } else {
              this.showSnackBar(response.message || 'ไม่สามารถเพิ่มข้อมูลได้');
            }
          },
          error: (error: any) => {
            console.error('Error creating person:', error);
            this.showSnackBar(error);
          }
        });
      }
    })
  }

  private showSnackBar(message: string): void {
    this.snackBar.open(message, 'ปิด', {
      duration: 3000,
      horizontalPosition: 'right',
      verticalPosition: 'top'
    })
  }
}
