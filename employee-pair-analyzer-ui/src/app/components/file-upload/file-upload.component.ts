import { Component, Output, EventEmitter } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-file-upload',
  template: `
    <div class="upload-container">
      <div class="file-input-container">
        <input 
          type="file" 
          (change)="onFileChange($event)" 
          accept=".csv"
          id="csvFile"
          #fileInput>
        <label for="csvFile" class="file-label">
          <span *ngIf="!selectedFile">Choose CSV File</span>
          <span *ngIf="selectedFile">{{ selectedFile.name }}</span>
        </label>
      </div>
      <button 
        mat-raised-button 
        color="primary" 
        (click)="onUpload()" 
        [disabled]="!selectedFile || isUploading"
        class="upload-button">
        <span *ngIf="!isUploading">Upload & Analyze</span>
        <span *ngIf="isUploading">Processing...</span>
      </button>
    </div>
  `,
  styles: [`
    .upload-container {
      display: flex;
      gap: 20px;
      align-items: center;
      flex-wrap: wrap;
    }
    .file-input-container {
      position: relative;
    }
    input[type="file"] {
      position: absolute;
      opacity: 0;
      width: 100%;
      height: 100%;
      cursor: pointer;
    }
    .file-label {
      display: inline-block;
      padding: 12px 20px;
      background: #f5f5f5;
      border: 2px dashed #ccc;
      border-radius: 4px;
      cursor: pointer;
      transition: all 0.3s ease;
      min-width: 200px;
      text-align: center;
    }
    .file-label:hover {
      background: #e8e8e8;
      border-color: #1976d2;
    }
    .upload-button {
      min-width: 160px;
    }
  `]
})
export class FileUploadComponent {
  selectedFile: File | null = null;
  isUploading = false;
  @Output() results = new EventEmitter<any[]>();
  @Output() loading = new EventEmitter<boolean>();

  constructor(private http: HttpClient) { }

  onFileChange(event: any) {
    this.selectedFile = event.target.files[0];
  }

  onUpload() {
    if (this.selectedFile) {
      this.isUploading = true;
      this.loading.emit(true);
      
      const formData = new FormData();
      formData.append('file', this.selectedFile);
      
      this.http.post('http://localhost:7000/api/EmployeePairs/upload', formData).subscribe({
        next: (response) => {
          this.results.emit(response as any[]);
          this.isUploading = false;
          this.loading.emit(false);
        },
        error: (error) => {
          console.error('Upload failed:', error);
          this.isUploading = false;
          this.loading.emit(false);
        }
      });
    }
  }
}