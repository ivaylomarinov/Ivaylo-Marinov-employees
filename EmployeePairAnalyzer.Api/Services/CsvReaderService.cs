using EmployeePairAnalyzer.Api.Models;
using System.Globalization;

namespace EmployeePairAnalyzer.Api.Services;

public class CsvReaderService : ICsvReaderService
{
    public IEnumerable<EmployeeProjectAssignment> ReadAssignments(Stream csvStream)
    {
        var assignments = new List<EmployeeProjectAssignment>();
        using var reader = new StreamReader(csvStream);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var parts = line.Split(',');
            if (parts.Length < 4) continue;
            if (!int.TryParse(parts[0].Trim(), out int empId)) continue;
            if (!int.TryParse(parts[1].Trim(), out int projId)) continue;
            DateTime dateFrom = ParseDate(parts[2].Trim());
            DateTime dateTo = parts[3].Trim().ToUpper() == "NULL" ? DateTime.Today : ParseDate(parts[3].Trim());
            assignments.Add(new EmployeeProjectAssignment
            {
                EmpID = empId,
                ProjectID = projId,
                DateFrom = dateFrom,
                DateTo = dateTo
            });
        }
        return assignments;
    }

    private static DateTime ParseDate(string date)
    {
        // Try multiple formats
        string[] formats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy", "dd/MM/yyyy", "yyyy/MM/dd", "M/d/yyyy", "d-M-yyyy", "d/M/yyyy" };
        foreach (var fmt in formats)
        {
            if (DateTime.TryParseExact(date, fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
        }
        if (DateTime.TryParse(date, out var fallback))
            return fallback;
        throw new FormatException($"Could not parse date: {date}");
    }
}