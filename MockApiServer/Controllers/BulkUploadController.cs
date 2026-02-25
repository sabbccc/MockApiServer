using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockApiServer.Data;
using MockApiServer.Data.Entities;

[Authorize]
public class BulkUploadController : Controller
{
    private const int HeaderRowIndex = 1;
    private readonly MockApiServerDbContext _context;

    public BulkUploadController(MockApiServerDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Sample()
    {
        using var workbook = new XLWorkbook();

        var appsSheet = workbook.Worksheets.Add("Applications");
        appsSheet.Cell(1, 1).Value = "Name";
        appsSheet.Cell(1, 2).Value = "IsActive";
        appsSheet.Cell(2, 1).Value = "Payments";
        appsSheet.Cell(2, 2).Value = "TRUE";
        appsSheet.Range(1, 1, 1, 2).Style.Font.Bold = true;
        appsSheet.Columns().AdjustToContents();

        var mocksSheet = workbook.Worksheets.Add("Mocks");
        mocksSheet.Cell(1, 1).Value = "ApplicationName";
        mocksSheet.Cell(1, 2).Value = "Name";
        mocksSheet.Cell(1, 3).Value = "Path";
        mocksSheet.Cell(1, 4).Value = "Method";
        mocksSheet.Cell(1, 5).Value = "IsActive";
        mocksSheet.Cell(2, 1).Value = "Payments";
        mocksSheet.Cell(2, 2).Value = "GetInvoice";
        mocksSheet.Cell(2, 3).Value = "/api/invoices/{id}";
        mocksSheet.Cell(2, 4).Value = "GET";
        mocksSheet.Cell(2, 5).Value = "TRUE";
        mocksSheet.Range(1, 1, 1, 5).Style.Font.Bold = true;
        mocksSheet.Columns().AdjustToContents();

        var scenariosSheet = workbook.Worksheets.Add("MockScenarios");
        scenariosSheet.Cell(1, 1).Value = "ApplicationName";
        scenariosSheet.Cell(1, 2).Value = "MockName";
        scenariosSheet.Cell(1, 3).Value = "ScenarioKey";
        scenariosSheet.Cell(1, 4).Value = "StatusCode";
        scenariosSheet.Cell(1, 5).Value = "ResponseJson";
        scenariosSheet.Cell(1, 6).Value = "HeadersJson";
        scenariosSheet.Cell(1, 7).Value = "IsActive";
        scenariosSheet.Cell(2, 1).Value = "Payments";
        scenariosSheet.Cell(2, 2).Value = "GetInvoice";
        scenariosSheet.Cell(2, 3).Value = "SUCCESS";
        scenariosSheet.Cell(2, 4).Value = 200;
        scenariosSheet.Cell(2, 5).Value = "{\"id\": \"123\", \"status\": \"PAID\"}";
        scenariosSheet.Cell(2, 6).Value = "{\"x-trace-id\": \"abc-123\"}";
        scenariosSheet.Cell(2, 7).Value = "TRUE";
        scenariosSheet.Range(1, 1, 1, 7).Style.Font.Bold = true;
        scenariosSheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = "mock-api-bulk-upload-sample.xlsx";
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile excelFile)
    {
        if (excelFile == null || excelFile.Length == 0)
        {
            TempData["failed"] = "Please select a valid Excel (.xlsx) file.";
            return RedirectToAction(nameof(Index));
        }

        if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            TempData["failed"] = "Only .xlsx files are supported.";
            return RedirectToAction(nameof(Index));
        }

        var errors = new List<string>();
        var appsAdded = 0;
        var mocksAdded = 0;
        var scenariosAdded = 0;

        using var stream = new MemoryStream();
        await excelFile.CopyToAsync(stream);
        stream.Position = 0;

        using var workbook = new XLWorkbook(stream);

        var appsSheet = workbook.Worksheets.FirstOrDefault(w => w.Name.Equals("Applications", StringComparison.OrdinalIgnoreCase));
        var mocksSheet = workbook.Worksheets.FirstOrDefault(w => w.Name.Equals("Mocks", StringComparison.OrdinalIgnoreCase));
        var scenariosSheet = workbook.Worksheets.FirstOrDefault(w => w.Name.Equals("MockScenarios", StringComparison.OrdinalIgnoreCase));

        if (appsSheet == null || mocksSheet == null || scenariosSheet == null)
        {
            TempData["failed"] = "The workbook must contain the sheets: Applications, Mocks, MockScenarios.";
            return RedirectToAction(nameof(Index));
        }

        var appHeaderMap = BuildHeaderMap(appsSheet);
        var mockHeaderMap = BuildHeaderMap(mocksSheet);
        var scenarioHeaderMap = BuildHeaderMap(scenariosSheet);

        if (!appHeaderMap.ContainsKey("Name") ||
            !mockHeaderMap.ContainsKey("ApplicationName") ||
            !mockHeaderMap.ContainsKey("Name") ||
            !mockHeaderMap.ContainsKey("Path") ||
            !mockHeaderMap.ContainsKey("Method") ||
            !scenarioHeaderMap.ContainsKey("ApplicationName") ||
            !scenarioHeaderMap.ContainsKey("MockName") ||
            !scenarioHeaderMap.ContainsKey("ScenarioKey") ||
            !scenarioHeaderMap.ContainsKey("StatusCode") ||
            !scenarioHeaderMap.ContainsKey("ResponseJson"))
        {
            TempData["failed"] = "Sample headers were modified. Please re-download the sample file and keep headers intact.";
            return RedirectToAction(nameof(Index));
        }

        var existingApps = await _context.Applications.AsNoTracking().ToListAsync();
        var appLookup = existingApps
            .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var row in appsSheet.RowsUsed().Skip(HeaderRowIndex))
        {
            var name = GetCellValue(row, appHeaderMap, "Name");
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            if (appLookup.ContainsKey(name))
            {
                continue;
            }

            var isActive = ParseBool(GetCellValue(row, appHeaderMap, "IsActive")) ?? true;
            var entity = new Application
            {
                Name = name.Trim(),
                IsActive = isActive,
                CreatedAt = DateTime.Now,
                CreatedBy = "bulk-upload"
            };

            _context.Applications.Add(entity);
            appLookup[name] = entity;
            appsAdded++;
        }

        if (appsAdded > 0)
        {
            await _context.SaveChangesAsync();
        }

        var appsWithIds = await _context.Applications.AsNoTracking().ToListAsync();
        appLookup = appsWithIds
            .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var existingMocks = await _context.Mocks.AsNoTracking().ToListAsync();
        var mockLookup = existingMocks.ToDictionary(
            m => BuildMockKey(m.ApplicationId, m.Name ?? "", m.Path, m.Method),
            m => m,
            StringComparer.OrdinalIgnoreCase);

        foreach (var row in mocksSheet.RowsUsed().Skip(HeaderRowIndex))
        {
            var applicationName = GetCellValue(row, mockHeaderMap, "ApplicationName");
            var name = GetCellValue(row, mockHeaderMap, "Name");
            var path = GetCellValue(row, mockHeaderMap, "Path");
            var method = (GetCellValue(row, mockHeaderMap, "Method") ?? "GET").ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(applicationName) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(path))
            {
                continue;
            }

            if (!appLookup.TryGetValue(applicationName, out var app))
            {
                errors.Add($"Mocks: Application '{applicationName}' not found for mock '{name}'.");
                continue;
            }

            var key = BuildMockKey(app.Id, name, path, method);
            if (mockLookup.ContainsKey(key))
            {
                continue;
            }

            var isActive = ParseBool(GetCellValue(row, mockHeaderMap, "IsActive")) ?? true;
            var entity = new Mock
            {
                ApplicationId = app.Id,
                Name = name.Trim(),
                Path = path.Trim(),
                Method = method,
                IsActive = isActive,
                CreatedAt = DateTime.Now,
                CreatedBy = "bulk-upload"
            };

            _context.Mocks.Add(entity);
            mockLookup[key] = entity;
            mocksAdded++;
        }

        if (mocksAdded > 0)
        {
            await _context.SaveChangesAsync();
        }

        var mocksWithIds = await _context.Mocks.AsNoTracking().ToListAsync();
        var scenarioMockLookup = mocksWithIds
            .GroupBy(m => BuildScenarioMockKey(m.ApplicationId, m.Name ?? ""), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var existingScenarios = await _context.MockScenarios.AsNoTracking().ToListAsync();
        var scenarioLookup = existingScenarios.ToDictionary(
            s => BuildScenarioKey(s.MockId, s.ScenarioKey),
            s => s,
            StringComparer.OrdinalIgnoreCase);

        foreach (var row in scenariosSheet.RowsUsed().Skip(HeaderRowIndex))
        {
            var applicationName = GetCellValue(row, scenarioHeaderMap, "ApplicationName");
            var mockName = GetCellValue(row, scenarioHeaderMap, "MockName");
            var scenarioKey = GetCellValue(row, scenarioHeaderMap, "ScenarioKey");
            var statusCodeText = GetCellValue(row, scenarioHeaderMap, "StatusCode");
            var responseJson = GetCellValue(row, scenarioHeaderMap, "ResponseJson");
            var headersJson = GetCellValue(row, scenarioHeaderMap, "HeadersJson");

            if (string.IsNullOrWhiteSpace(applicationName) ||
                string.IsNullOrWhiteSpace(mockName) ||
                string.IsNullOrWhiteSpace(scenarioKey) ||
                string.IsNullOrWhiteSpace(statusCodeText) ||
                string.IsNullOrWhiteSpace(responseJson))
            {
                continue;
            }

            if (!int.TryParse(statusCodeText, out var statusCode))
            {
                errors.Add($"MockScenarios: Invalid status code '{statusCodeText}' for scenario '{scenarioKey}'.");
                continue;
            }

            if (!appLookup.TryGetValue(applicationName, out var app))
            {
                errors.Add($"MockScenarios: Application '{applicationName}' not found for scenario '{scenarioKey}'.");
                continue;
            }

            var mockKey = BuildScenarioMockKey(app.Id, mockName);
            if (!scenarioMockLookup.TryGetValue(mockKey, out var mock))
            {
                errors.Add($"MockScenarios: Mock '{mockName}' not found for scenario '{scenarioKey}'.");
                continue;
            }

            var scenarioLookupKey = BuildScenarioKey(mock.Id, scenarioKey);
            if (scenarioLookup.ContainsKey(scenarioLookupKey))
            {
                continue;
            }

            var isActive = ParseBool(GetCellValue(row, scenarioHeaderMap, "IsActive")) ?? true;
            var entity = new MockScenario
            {
                MockId = mock.Id,
                ScenarioKey = scenarioKey.Trim(),
                StatusCode = statusCode,
                ResponseJson = responseJson.Trim(),
                HeadersJson = string.IsNullOrWhiteSpace(headersJson) ? null : headersJson.Trim(),
                IsActive = isActive,
                CreatedAt = DateTime.Now,
                CreatedBy = "bulk-upload"
            };

            _context.MockScenarios.Add(entity);
            scenarioLookup[scenarioLookupKey] = entity;
            scenariosAdded++;
        }

        if (scenariosAdded > 0)
        {
            await _context.SaveChangesAsync();
        }

        var summary = $"Upload complete. Added: Applications {appsAdded}, Mocks {mocksAdded}, MockScenarios {scenariosAdded}.";
        TempData["success"] = summary;

        if (errors.Count > 0)
        {
            TempData["warning"] = $"Completed with {errors.Count} warning(s). First: {errors[0]}";
        }

        return RedirectToAction(nameof(Index));
    }

    private static Dictionary<string, int> BuildHeaderMap(IXLWorksheet sheet)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var headerRow = sheet.Row(HeaderRowIndex);

        foreach (var cell in headerRow.CellsUsed())
        {
            var value = cell.GetString().Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                map[value] = cell.Address.ColumnNumber;
            }
        }

        return map;
    }

    private static string? GetCellValue(IXLRow row, Dictionary<string, int> map, string header)
    {
        if (!map.TryGetValue(header, out var column))
        {
            return null;
        }

        var cell = row.Cell(column);
        var value = cell.GetString();
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static bool? ParseBool(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (bool.TryParse(value, out var parsed))
        {
            return parsed;
        }

        if (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("y", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (value.Equals("0", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("no", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("n", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return null;
    }

    private static string BuildMockKey(int applicationId, string name, string path, string method)
    {
        return $"{applicationId}|{name.Trim()}|{path.Trim()}|{method.Trim()}";
    }

    private static string BuildScenarioMockKey(int applicationId, string mockName)
    {
        return $"{applicationId}|{mockName.Trim()}";
    }

    private static string BuildScenarioKey(int mockId, string scenarioKey)
    {
        return $"{mockId}|{scenarioKey.Trim()}";
    }
}
