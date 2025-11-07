using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockApiServer.Services;

[ApiController]
[Route("{*path:regex(^(?!swagger|health|home|applications|mocks|mockscenarios|user).*$)}")]
[AllowAnonymous]
public class MockRequestsController : ControllerBase
{
    private readonly IMockRequestsService _mockRequestsService;

    public MockRequestsController(IMockRequestsService mockRequestsService)
    {
        _mockRequestsService = mockRequestsService;
    }

    [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
    public async Task<IActionResult> HandleAsync(string path)
    {
        var scenarioKey = Request.Headers["X-Mock-Scenario"].FirstOrDefault();

        var (scenario, error) = await _mockRequestsService.GetScenarioAsync(path, Request.Method, scenarioKey);
        if (error != null)
            return BadRequest(new { error });

        var (response, rawJson) = _mockRequestsService.BuildResponse(scenario!, Response);

        // ✅ Log compact JSON (no pretty-print)
        Serilog.Log.Information("[Final Response : {StatusCode}] | Body: {RawJson}", scenario!.StatusCode, rawJson);

        return response;
    }
}
