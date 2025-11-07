using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockApiServer.Data.Entities;
using MockApiServer.Extensions;
using MockApiServer.Models.ViewModels;
using MockApiServer.Services;

[Authorize]
public class MockScenariosController : Controller
{
    private readonly IMockScenarioService _service;
    private readonly IMockService _mockService;

    public MockScenariosController(IMockScenarioService service, IMockService mockService)
    {
        _service = service;
        _mockService = mockService;
    }

    // INDEX: List all scenarios for a mock
    public async Task<IActionResult> Index(int mockId)
    {
        List<MockScenarioViewModel>? scenarios = new();
        scenarios = (mockId == 0) ?
            await _service.GetAllAsync() :
            await _service.GetByMockIdAsync(mockId);

        scenarios = (scenarios.Count() > 0) ?
            scenarios : new List<MockScenarioViewModel>();

        return View(scenarios);
    }

    // CREATE (GET)
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var scenario = new MockScenarioViewModel();
        var mocks = await _mockService.GetAllAsync(); // or from repository
        scenario.MockList = mocks.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(),
            Text = a.Name
        }).ToList();
        ViewBag.MockId = scenario.MockId;
        return PartialView("Create", scenario);
    }

    // CREATE (POST)
    [HttpPost]
    public async Task<IActionResult> Create(MockScenarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Couldn't save the Mock Scenario." });
        }

        await _service.AddAsync(model);

        var scenarios = await _service.GetAllAsync();
        var html = await this.RenderViewAsync("_ViewAll", scenarios, true);

        return Json(new { success = true, message = "Mock Scenario added successfully!", html });
    }

    // EDIT (GET)
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var scenario = await _service.GetByIdAsync(id);
        if (scenario == null) return NotFound();

        var mock = await _mockService.GetByIdAsync((int)scenario.MockId);
        ViewBag.MockId = mock?.Id;
        ViewBag.MockName = mock?.Name;
        ViewBag.ApplicationId = mock?.ApplicationId;
        ViewBag.ApplicationName = mock?.ApplicationName;

        return PartialView("Edit", scenario);
    }

    // EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MockScenarioViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var mock = await _mockService.GetByIdAsync((int)model.MockId);
            ViewBag.MockId = mock?.Id;
            ViewBag.MockName = mock?.Name;
            ViewBag.ApplicationId = mock?.ApplicationId;
            ViewBag.ApplicationName = mock?.ApplicationName;
            return Json(new { success = false, message = "Couldn't update the Mock Scenario." });
        }

        await _service.UpdateAsync(model);
        var scenarios = await _service.GetAllAsync();
        var html = await this.RenderViewAsync("_ViewAll", scenarios, true);

        return Json(new { success = true, message = "Mock Scenario updated successfully!", html });
    }

    // DELETE
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var scenario = await _service.GetByIdAsync(id);
        if (scenario == null)
        {
            TempData["failed"] = "Mock scenario not found.";
            return RedirectToAction(nameof(Index));
        }

        await _service.DeleteAsync(id);
        TempData["success"] = "Mock scenario deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

}
