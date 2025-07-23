using EmployeePairAnalyzer.Api.Models;

namespace EmployeePairAnalyzer.Api.Services;

public class EmployeePairService : IEmployeePairService
{
    public List<EmployeePairResult> CalculatePairs(IEnumerable<EmployeeProjectAssignment> assignments)
    {
        var pairDays = new Dictionary<(int, int), int>();
        var pairProjectDays = new Dictionary<(int, int, int), int>(); // (Emp1, Emp2, ProjectId) -> days

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
                        var key = (Math.Min(e1.EmpID, e2.EmpID), Math.Max(e1.EmpID, e2.EmpID));
                        var projectKey = (key.Item1, key.Item2, projectGroup.Key);

                        if (!pairDays.ContainsKey(key))
                            pairDays[key] = 0;
                        pairDays[key] += (int)daysWorked;

                        if (!pairProjectDays.ContainsKey(projectKey))
                            pairProjectDays[projectKey] = 0;
                        pairProjectDays[projectKey] += (int)daysWorked;
                    }
                }
            }
        }

        // Find the pair with the maximum total days
        var maxPair = pairDays.OrderByDescending(p => p.Value).FirstOrDefault();

        if (maxPair.Key == default)
            return new List<EmployeePairResult>();

        // Return all project details for the max pair
        var results = new List<EmployeePairResult>();
        foreach (var kvp in pairProjectDays)
        {
            if (kvp.Key.Item1 == maxPair.Key.Item1 && kvp.Key.Item2 == maxPair.Key.Item2)
            {
                results.Add(new EmployeePairResult
                {
                    EmployeeId1 = kvp.Key.Item1,
                    EmployeeId2 = kvp.Key.Item2,
                    ProjectId = kvp.Key.Item3,
                    DaysWorked = kvp.Value
                });
            }
        }

        // Optionally, add a summary row:
        results.Add(new EmployeePairResult
        {
            EmployeeId1 = maxPair.Key.Item1,
            EmployeeId2 = maxPair.Key.Item2,
            ProjectId = 0, // 0 means "across all projects"
            DaysWorked = maxPair.Value
        });

        return results;
    }
}