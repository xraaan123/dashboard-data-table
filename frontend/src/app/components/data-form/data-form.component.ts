import { ChangeDetectorRef, Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogRef,
} from '@angular/material/dialog';

@Component({
  selector: 'app-data-form',
  templateUrl: './data-form.component.html',
  styleUrls: ['./data-form.component.scss'],
})
export class DataFormComponent implements OnInit {
  dataForm: FormGroup;
  isView: boolean = false;
  maxDate: Date;
  calculatedAge: number = 0;

  constructor(
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    public dialogRef: MatDialogRef<DataFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.isView = data?.isView || false;

    this.maxDate = new Date();

    this.dataForm = fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      address: ['', [Validators.required, Validators.minLength(10)]],
      birthDate: ['', [Validators.required]],
    });
  }

  ngOnInit(): void {
    if (this.isView && this.data.record) {
      const fullNameParts = this.data.record.fullName.split(' ');
      const firstName =
        fullNameParts.slice(0, -1).join(' ') || fullNameParts[0] || '';
      const lastName = fullNameParts[fullNameParts.length - 1] || '';

      this.dataForm.patchValue({
        firstName: firstName,
        lastName: lastName,
        address: this.data.record.address,
        birthDate: this.formatDateForInput(this.data.record.birthDate),
      });
    }

    if (this.isView) {
      this.dataForm.disable();
    }

    this.dataForm.get('birthDate')?.valueChanges.subscribe((birthDate) => {
      if (birthDate) {
        // 1. Calculate age immediately
        this.calculatedAge = this.calculateAge(new Date(birthDate));

        // 2. Update the UI
        this.cdr.detectChanges();

        // 3. Log for debugging
        console.log(`Birth date: ${birthDate}, Age: ${this.calculatedAge}`);
      } else {
        // 4. Reset if no date selected
        this.calculatedAge = 0;
        this.cdr.detectChanges();
      }
    });
  }

  private formatDateForInput(date: Date): string {
    if (!date) return '';
    const d = new Date(date);
    const year = d.getFullYear();
    const month = (d.getMonth() + 1).toString().padStart(2, '0');
    const day = d.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
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

  getDisplayAge(): number {
    if (this.isView && this.data.record) {
      return this.data.record.age;
    }

    const birthDateValue = this.dataForm.get('birthDate')?.value;
    if (birthDateValue) {
      return this.calculateAge(new Date(birthDateValue));
    }

    return 0;
  }

  getFullNameDisplay(): string {
    const firstName = this.dataForm.get('firstName')?.value || '';
    const lastName = this.dataForm.get('lastName')?.value || '';
    return `${firstName} ${lastName}`.trim();
  }

  onSave(): void {
    if (this.dataForm.valid) {
      const formData = {
        ...this.dataForm.value,
        fullName: this.getFullNameDisplay(),
        birthDate: this.dataForm.value.birthDate,
      };
      this.dialogRef.close(formData);
    } else {
      this.dataForm.markAllAsTouched();
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  hasError(fieldName: string, errorType: string): boolean {
    const field = this.dataForm.get(fieldName);
    return field ? field.hasError(errorType) && field.touched : false;
  }

  getErrorMessage(fieldName: string): string {
    const field = this.dataForm.get(fieldName);
    if (!field || !field.errors || !field.touched) return '';

    if (field.hasError('required')) {
      switch (fieldName) {
        case 'firstName':
          return 'กรุณากรอกชื่อ';
        case 'lastName':
          return 'กรุณากรอกนามสกุล';
        case 'address':
          return 'กรุณากรอกที่อยู่';
        case 'birthDate':
          return 'กรุณาเลือกวันเกิด';
        default:
          return 'กรุณากรอกข้อมูล';
      }
    }

    if (field.hasError('minLength')) {
      const requiredLength = field.errors?.['minLength']?.requiredLength;
      switch (fieldName) {
        case 'firstName':
          return `ชื่อต้องมีอย่างน้อย ${requiredLength} ตัวอักษร`;
        case 'lastName':
          return `นามสกุลต้องมีอย่างน้อย ${requiredLength} ตัวอักษร`;
        case 'address':
          return `ที่อยู่ต้องมีอย่างน้อย ${requiredLength} ตัวอักษร`;
        default:
          return `ต้องมีอย่างน้อย ${requiredLength} ตัวอักษร`;
      }
    }

    return '';
  }

  getMaxDate(): string {
    const today = new Date();
    const year = today.getFullYear();
    const month = (today.getMonth() + 1).toString().padStart(2, '0');
    const day = today.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
