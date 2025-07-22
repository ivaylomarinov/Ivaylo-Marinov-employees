using EmployeePairAnalyzer.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePairAnalyzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeePairsController : ControllerBase
{
    private readonly ICsvReaderService _csvReader;
    private readonly IEmployeePairService _pairService;

    public EmployeePairsController(ICsvReaderService csvReader, IEmployeePairService pairService)
    {
        _csvReader = csvReader;
        _pairService = pairService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
        using var stream = file.OpenReadStream();
        // Offload CPU-bound work to a background thread to justify async/await usage
        var assignments = await Task.Run(() => _csvReader.ReadAssignments(stream));
        var results = await Task.Run(() => _pairService.CalculatePairs(assignments));
        // Return grouped data: pairs and their projects/days
        var grouped = results
            .GroupBy(r => (r.EmployeeId1, r.EmployeeId2))
            .Select(g => new
            {
                EmployeeId1 = g.Key.EmployeeId1,
                EmployeeId2 = g.Key.EmployeeId2,
                Projects = g.Select(x => new { x.ProjectId, x.DaysWorked }).ToList(),
                TotalDaysWorked = g.Sum(x => x.DaysWorked)
            })
            .OrderByDescending(g => g.TotalDaysWorked)
            .ToList();
        return Ok(grouped);
    }
}