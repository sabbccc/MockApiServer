using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockApiServer.Extensions;
using MockApiServer.Models.ViewModels;
using MockApiServer.Services;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly IApplicationService _service;
    public ApplicationsController(IApplicationService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        List<ApplicationViewModel> apps = new();
        apps = await _service.GetAllAsync();

        apps = (apps.Count() > 0) ? apps : new List<ApplicationViewModel>();
        return View(apps);
    }

    public IActionResult Create() => View(new ApplicationViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(ApplicationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Couldn't save the Application." });
        }
        await _service.AddAsync(model);
        var apps = await _service.GetAllAsync();

        var html = await this.RenderViewAsync("_ViewAll", apps, true);
        return Json(new { success = true, message = "Application added successfully!", html });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var model = await _service.GetByIdAsync(id);
        return PartialView("Edit", model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ApplicationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["failed"] = "Couldn't update the Application.";
            return PartialView("Edit", model);
        }
        await _service.UpdateAsync(model);
        var apps = await _service.GetAllAsync();

        var html = await this.RenderViewAsync("_ViewAll", apps, true);
        return Json(new { success = true, message = "Application updated successfully!", html });
    }

    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        TempData["success"] = "Application deleted successfully!";
        return RedirectToAction(nameof(Index));
    }
}
