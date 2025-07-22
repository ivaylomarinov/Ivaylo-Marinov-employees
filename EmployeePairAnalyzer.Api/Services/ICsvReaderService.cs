using EmployeePairAnalyzer.Api.Models;

namespace EmployeePairAnalyzer.Api.Services;

public interface ICsvReaderService
{
    IEnumerable<EmployeeProjectAssignment> ReadAssignments(Stream csvStream);
}