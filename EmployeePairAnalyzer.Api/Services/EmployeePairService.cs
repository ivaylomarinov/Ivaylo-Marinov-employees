using EmployeePairAnalyzer.Api.Models;

namespace EmployeePairAnalyzer.Api.Services;

public class EmployeePairService : IEmployeePairService
{
    public List<EmployeePairResult> CalculatePairs(IEnumerable<EmployeeProjectAssignment> assignments)
    {
        var results = new List<EmployeePairResult>();
        var grouped = assignments.GroupBy(a => a.ProjectID);
        foreach (var projectGroup in grouped)
        {
            var employees = projectGroup.ToList();
            for (int i = 0; i < employees.Count; i++)
            {
                for (int j = i + 1; j < employees.Count; j++)
                {
                    var e1 = employees[i];
                    var e2 = employees[j];
                    var overlapStart = e1.DateFrom > e2.DateFrom ? e1.DateFrom : e2.DateFrom;
                    var overlapEnd = e1.DateTo < e2.DateTo ? e1.DateTo : e2.DateTo;
                    var daysWorked = (overlapEnd - overlapStart).TotalDays;
                    if (daysWorked > 0)
                    {
                        results.Add(new EmployeePairResult
                        {
                            EmployeeId1 = Math.Min(e1.EmpID, e2.EmpID),
                            EmployeeId2 = Math.Max(e1.EmpID, e2.EmpID),
                            ProjectId = projectGroup.Key,
                            DaysWorked = (int)daysWorked
                        });
                    }
                }
            }
        }
        return results;
    }
}