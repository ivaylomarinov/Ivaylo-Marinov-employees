import { Component, Input, OnChanges } from '@angular/core';

@Component({
  selector: 'app-results-table',
  template: `
    <table mat-table [dataSource]="flatResults" class="mat-elevation-z8" *ngIf="flatResults.length">
      <ng-container matColumnDef="employee1">
        <th mat-header-cell *matHeaderCellDef>Employee ID #1</th>
        <td mat-cell *matCellDef="let row">{{ row.EmployeeId1 }}</td>
      </ng-container>
      <ng-container matColumnDef="employee2">
        <th mat-header-cell *matHeaderCellDef>Employee ID #2</th>
        <td mat-cell *matCellDef="let row">{{ row.EmployeeId2 }}</td>
      </ng-container>
      <ng-container matColumnDef="projectId">
        <th mat-header-cell *matHeaderCellDef>Project ID</th>
        <td mat-cell *matCellDef="let row">{{ row.ProjectId }}</td>
      </ng-container>
      <ng-container matColumnDef="daysWorked">
        <th mat-header-cell *matHeaderCellDef>Days Worked</th>
        <td mat-cell *matCellDef="let row">{{ row.DaysWorked }}</td>
      </ng-container>
      <tr mat-header-row *matHeaderRowDef="columns"></tr>
      <tr mat-row *matRowDef="let row; columns: columns;"></tr>
    </table>
    <div *ngIf="results.length && !flatResults.length" class="no-results">
      <p>No employee pairs found in the uploaded data.</p>
    </div>
  `
})
export class ResultsTableComponent implements OnChanges {
  @Input() results: any[] = [];
  columns: string[] = ['employee1', 'employee2', 'projectId', 'daysWorked'];
  
  ngOnChanges() {
    
  }
  
  get flatResults() {
    if (!this.results || this.results.length === 0) {
      return [];
    }
    
    return this.results.flatMap(r => {
      if (!r.projects || !Array.isArray(r.projects)) {
        return [];
      }
      
      return r.projects.map((p: any) => ({
        EmployeeId1: r.employeeId1,
        EmployeeId2: r.employeeId2,
        ProjectId: p.projectId,
        DaysWorked: p.daysWorked
      }));
    });
  }
}