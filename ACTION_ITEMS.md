# Action Items Checklist

This checklist provides specific actionable items derived from the pull request analysis.

## 🔴 Critical (Must Fix Before Merge)

### 1. Remove Build Artifacts from Repository
- [ ] Run: `git rm -r --cached employee-pair-analyzer-ui/.angular`
- [ ] Run: `git rm -r --cached employee-pair-analyzer-ui/dist`
- [ ] Commit and push changes
- [ ] Verify repository size reduction

### 2. Fix CORS Security Vulnerability
**File:** `EmployeePairAnalyzer.Api/Program.cs` (lines 12-19)

Replace:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});
```

With:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:4200") // Add production URL when deployed
            .AllowAnyHeader()
            .AllowAnyMethod());
});
```

- [ ] Update CORS configuration
- [ ] Test with frontend
- [ ] Add production URL to appsettings.json

### 3. Add File Upload Validation
**File:** `EmployeePairAnalyzer.Api/Controllers/EmployeePairsController.cs` (line 20)

Add validation after line 22:
```csharp
[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file)
{
    if (file == null || file.Length == 0) 
        return BadRequest("No file uploaded.");
    
    // Add these validations
    const long maxFileSize = 10 * 1024 * 1024; // 10MB
    if (file.Length > maxFileSize)
        return BadRequest($"File too large. Maximum size is {maxFileSize / 1024 / 1024}MB.");
    
    var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
    if (extension != ".csv")
        return BadRequest("Only CSV files are allowed.");
    
    // Rest of existing code...
}
```

- [ ] Add file size validation
- [ ] Add file type validation
- [ ] Test with oversized files
- [ ] Test with non-CSV files

### 4. Add Error Handling for CSV Parsing
**File:** `EmployeePairAnalyzer.Api/Controllers/EmployeePairsController.cs`

Wrap the processing in try-catch:
```csharp
try
{
    using var stream = file.OpenReadStream();
    var assignments = await Task.Run(() => _csvReader.ReadAssignments(stream));
    var results = await Task.Run(() => _pairService.CalculatePairs(assignments));
    // ... rest of code
}
catch (FormatException ex)
{
    return BadRequest($"Invalid date format in CSV: {ex.Message}");
}
catch (Exception ex)
{
    // Log the exception
    return StatusCode(500, "Error processing file. Please check the file format.");
}
```

- [ ] Add try-catch block
- [ ] Test with malformed CSV
- [ ] Test with invalid dates

## 🟡 High Priority (Should Fix Soon)

### 5. Fix Async/Await Anti-Pattern
**File:** `EmployeePairAnalyzer.Api/Controllers/EmployeePairsController.cs`

**Option A:** Remove async (recommended):
```csharp
[HttpPost("upload")]
public IActionResult UploadFile(IFormFile file)
{
    // Remove await and Task.Run
    var assignments = _csvReader.ReadAssignments(stream);
    var results = _pairService.CalculatePairs(assignments);
    // ...
}
```

**Option B:** Make services truly async:
- Make `ReadAssignments` return `IAsyncEnumerable<EmployeeProjectAssignment>`
- Make `CalculatePairs` async with true async operations

- [ ] Choose approach (A or B)
- [ ] Implement changes
- [ ] Update tests
- [ ] Verify performance

### 6. Fix Days Calculation Bug
**File:** `EmployeePairAnalyzer.Api/Services/EmployeePairService.cs` (line 24)

Change:
```csharp
var daysWorked = (overlapEnd - overlapStart).TotalDays;
```

To:
```csharp
var daysWorked = (overlapEnd - overlapStart).TotalDays + 1;
```

- [ ] Update calculation
- [ ] Update unit tests
- [ ] Test with sample data

### 7. Add Logging
**File:** `EmployeePairAnalyzer.Api/Controllers/EmployeePairsController.cs`

Add ILogger:
```csharp
private readonly ILogger<EmployeePairsController> _logger;

public EmployeePairsController(
    ICsvReaderService csvReader, 
    IEmployeePairService pairService,
    ILogger<EmployeePairsController> logger)
{
    _csvReader = csvReader;
    _pairService = pairService;
    _logger = logger;
}
```

Add logging statements:
```csharp
_logger.LogInformation("Processing CSV file upload");
_logger.LogError(ex, "Error processing CSV file");
```

- [ ] Add ILogger injection
- [ ] Add logging statements
- [ ] Configure logging in appsettings.json

### 8. Move Configuration to Settings
**File:** Create/Update `EmployeePairAnalyzer.Api/appsettings.json`

Add:
```json
{
  "FileUpload": {
    "MaxFileSizeInBytes": 10485760,
    "AllowedExtensions": [".csv"]
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  }
}
```

- [ ] Add configuration section
- [ ] Update Program.cs to read config
- [ ] Update controller to use config

## 🟢 Medium Priority (Nice to Have)

### 9. Improve Test Coverage
**Files:** `EmployeePairAnalyzer.Tests/`

Add tests for:
- [ ] Empty CSV file
- [ ] CSV with single employee
- [ ] CSV with no overlapping dates
- [ ] Different date formats
- [ ] Invalid data scenarios
- [ ] Large files (performance test)

### 10. Optimize Date Parsing
**File:** `EmployeePairAnalyzer.Api/Services/CsvReaderService.cs`

- [ ] Detect date format from first successful parse
- [ ] Cache the format for subsequent rows
- [ ] Or use CsvHelper library

### 11. Remove Magic Numbers
**File:** `EmployeePairAnalyzer.Api/Services/EmployeePairService.cs` (line 69)

Replace:
```csharp
ProjectId = 0, // 0 means "across all projects"
```

With:
```csharp
ProjectId = null, // null means "across all projects"
```

Or use a constant:
```csharp
private const int ALL_PROJECTS = 0;
ProjectId = ALL_PROJECTS,
```

- [ ] Choose approach
- [ ] Implement change
- [ ] Update model if needed

### 12. Add API Documentation
- [ ] Add XML comments to controllers
- [ ] Add XML comments to services
- [ ] Configure Swagger to use XML comments
- [ ] Add README.md with API documentation

### 13. Add Health Checks
**File:** `EmployeePairAnalyzer.Api/Program.cs`

Add:
```csharp
builder.Services.AddHealthChecks();

// After app.MapControllers():
app.MapHealthChecks("/health");
```

- [ ] Add health check endpoint
- [ ] Test health check
- [ ] Add to documentation

## Verification Steps

After completing fixes:

### Testing
- [ ] Run all unit tests: `dotnet test`
- [ ] Manual test: Upload sample-employees.csv
- [ ] Test error cases (large file, wrong type, malformed CSV)
- [ ] Test CORS from frontend
- [ ] Load test with large CSV files

### Code Quality
- [ ] Run code analysis: `dotnet build /p:RunAnalyzers=true`
- [ ] Check for compiler warnings
- [ ] Review code with team

### Security
- [ ] Verify CORS configuration
- [ ] Verify file validation works
- [ ] Check error messages don't leak sensitive info
- [ ] Run security scan (if available)

### Documentation
- [ ] Update README.md
- [ ] Document API endpoints
- [ ] Update "How to run.txt" if needed
- [ ] Add inline code comments where needed

## Completion Criteria

PR is ready to merge when:
- [x] .gitignore file added
- [ ] All build artifacts removed
- [ ] All 🔴 Critical items completed
- [ ] At least 75% of 🟡 High Priority items completed
- [ ] All tests passing
- [ ] Code review approved
- [ ] Security review passed

---

**Created:** 2025-10-24  
**Last Updated:** 2025-10-24  
**Status:** In Progress
