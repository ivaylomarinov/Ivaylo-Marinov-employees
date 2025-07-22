using EmployeePairAnalyzer.Api.Models;
using EmployeePairAnalyzer.Api.Services;
using System;
using System.Collections.Generic;
using Xunit;

public class EmployeePairServiceTests
{
    [Fact]
    public void CalculatePairs_ReturnsCorrectPairs()
    {
        var assignments = new List<EmployeeProjectAssignment>
        {
            new EmployeeProjectAssignment { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2024, 1, 1), DateTo = new DateTime(2024, 2, 1) },
            new EmployeeProjectAssignment { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2024, 1, 15), DateTo = new DateTime(2024, 2, 15) }
        };
        var service = new EmployeePairService();
        var result = service.CalculatePairs(assignments);

        Assert.Single(result);
        Assert.Equal(1, result[0].EmployeeId1);
        Assert.Equal(2, result[0].EmployeeId2);
        Assert.Equal(100, result[0].ProjectId);
        Assert.Equal(17, result[0].DaysWorked); // Overlap: Jan 15 - Feb 1
    }
}