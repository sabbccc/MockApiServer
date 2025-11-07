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

    public MocksController(IMockService service, IApplicationService appService)
    {
        _service = service;
        _appService = appService;
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

        var mocks = await _service.GetAllAsync();
        var html = await this.RenderViewAsync("_ViewAll", mocks, true);

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

        return Json(new { success = true, message = "Mock updated successfully!", html });
    }

    // DELETE
    public async Task<IActionResult> Delete(int id)
    {
        var mock = await _service.GetByIdAsync(id);
        if (mock == null) return NotFound();

        await _service.DeleteAsync(id);
        TempData["success"] = "Mock deleted successfully!";

        return RedirectToAction(nameof(Index));
    }
}
