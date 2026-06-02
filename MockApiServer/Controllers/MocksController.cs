using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockApiServer.Data.Entities;
using MockApiServer.Extensions;
using MockApiServer.Models.ViewModels;
using MockApiServer.Services;
using System.Linq;                // 🔸 Required for .Select()

[Authorize]
public class MocksController : Controller
{
    private readonly IMockService _service;
    private readonly IApplicationService _appService;
    private readonly ILogger<MocksController> _logger;

    public MocksController(IMockService service, IApplicationService appService, ILogger<MocksController> logger)
    {
        _service = service;
        _appService = appService;
        _logger = logger;
    }

    // INDEX: List mocks by application
    public async Task<IActionResult> Index(int appId)
    {
        List<MockViewModel>? mocks = new();

        mocks = (appId == 0) ?
            await _service.GetAllAsync() :
            await _service.GetByApplicationIdAsync(appId);

        mocks = (mocks.Count() > 0) ?
            mocks : new List<MockViewModel>();

        ViewBag.AppId = appId;

        _logger.LogInformation("Retrieved {Count} mocks for Application ID: {AppId}", mocks.Count(), appId);
        return View(mocks);
    }

    public async Task<IActionResult> Create()
    {
        var mock = new MockViewModel();
        var apps = await _appService.GetAllAsync(); // or from repository
        mock.ApplicationList = apps.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(),
            Text = a.Name
        }).ToList();
        ViewBag.ApplicationId = mock.ApplicationId;

        _logger.LogInformation("Preparing Create view with {AppCount} applications", apps.Count());
        return PartialView("Create", mock);
    }

    [HttpPost]
    public async Task<IActionResult> Create(MockViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Couldn't save the Mock." });
        }

        await _service.AddAsync(model);

        // Get filtered mocks based on the application context
        var mocks = model.ApplicationId.HasValue && model.ApplicationId.Value > 0
            ? await _service.GetByApplicationIdAsync(model.ApplicationId.Value)
            : await _service.GetAllAsync();
        var html = await this.RenderViewAsync("_ViewAll", mocks, true);

        _logger.LogInformation("Added new Mock with ID: {MockId} for Application ID: {AppId}", model.Id, model.ApplicationId);
        return Json(new { success = true, message = "Mock added successfully!", html });
    }

    // EDIT (GET)
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var mock = await _service.GetByIdAsync(id);
        if (mock == null) return NotFound();

        var apps = await _appService.GetAllAsync(); // or from repository
        mock.ApplicationList = apps.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(),
            Text = a.Name
        }).ToList();

        ViewBag.ApplicationId = mock.ApplicationId;
        ViewBag.ApplicationName = mock.ApplicationName;

        _logger.LogInformation("Preparing Edit view for Mock ID: {MockId} with Application ID: {AppId}", id, mock.ApplicationId);
        return PartialView("Edit", mock);
    }

    // EDIT (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MockViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ApplicationId = model.ApplicationId;
            ViewBag.ApplicationName = model.ApplicationName;
            return Json(new { success = false, message = "Couldn't update the Mock." });
        }

        await _service.UpdateAsync(model);
        var mocks = await _service.GetAllAsync();
        var html = await this.RenderViewAsync("_ViewAll", mocks, true);

        _logger.LogInformation("Updated Mock with ID: {MockId} for Application ID: {AppId}", model.Id, model.ApplicationId);
        return Json(new { success = true, message = "Mock updated successfully!", html });
    }

    // DELETE
    public async Task<IActionResult> Delete(int id)
    {
        var mock = await _service.GetByIdAsync(id);
        if (mock == null) return NotFound();

        await _service.DeleteAsync(id);
        TempData["success"] = "Mock deleted successfully!";

        _logger.LogInformation("Deleted Mock with ID: {MockId} for Application ID: {AppId}", id, mock.ApplicationId);
        return RedirectToAction(nameof(Index));
    }
}
