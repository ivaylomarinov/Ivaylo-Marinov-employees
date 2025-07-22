using EmployeePairAnalyzer.Api.Models;

namespace EmployeePairAnalyzer.Api.Services;

public interface IEmployeePairService
{
    List<EmployeePairResult> CalculatePairs(IEnumerable<EmployeeProjectAssignment> assignments);
}