import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <div class="app-container">
      <header class="app-header">
        <h1>Employee Pair Analyzer</h1>
        <p>Identify employees who worked together on common projects for the longest periods</p>
      </header>
      
      <main class="app-content">
        <section class="upload-section">
          <h2>Upload CSV File</h2>
          <app-file-upload (results)="onResults($event)" (loading)="onLoading($event)"></app-file-upload>
        </section>
        
        <section class="results-section" *ngIf="pairResults.length || isLoading">
          <h2>Analysis Results</h2>
          <div *ngIf="isLoading" class="loading">Analyzing employee pairs...</div>
          <app-results-table [results]="pairResults" *ngIf="!isLoading"></app-results-table>
        </section>
      </main>
    </div>
  `,
  styles: [`
    .app-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }
    .app-header {
      text-align: center;
      margin-bottom: 40px;
    }
    .app-header h1 {
      color: #1976d2;
      margin-bottom: 10px;
    }
    .app-header p {
      color: #666;
      font-size: 16px;
    }
    .upload-section, .results-section {
      margin-bottom: 40px;
    }
    .upload-section h2, .results-section h2 {
      color: #333;
      border-bottom: 2px solid #1976d2;
      padding-bottom: 10px;
    }
    .loading {
      text-align: center;
      padding: 20px;
      color: #666;
      font-style: italic;
    }
  `]
})
export class AppComponent {
  pairResults: any[] = [];
  isLoading = false;

  onResults(results: any[]) {
    this.pairResults = results;
    this.isLoading = false;
  }

  onLoading(loading: boolean) {
    this.isLoading = loading;
  }
}