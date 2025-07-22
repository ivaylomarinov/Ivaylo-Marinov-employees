using EmployeePairAnalyzer.Api.Services;
using Xunit;
using System.Text;
using System.IO;
using System.Linq;

public class CsvReaderServiceTests
{
    [Theory]
    [InlineData("1,100,2024-01-01,2024-01-31\n2,100,2024-01-15,NULL", 2)]
    [InlineData("1,100,01/01/2024,31/01/2024\n2,100,15/01/2024,NULL", 2)]
    public void ReadAssignments_ParsesMultipleDateFormats(string csv, int expectedCount)
    {
        var service = new CsvReaderService();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var assignments = service.ReadAssignments(stream).ToList();

        Assert.Equal(expectedCount, assignments.Count);
        Assert.All(assignments, a => Assert.Equal(100, a.ProjectID));
    }
}