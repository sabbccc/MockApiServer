using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockApiServer.Models.ViewModels;
using MockApiServer.Services;

namespace MockApiServer.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IApplicationService _applicationService;
        private readonly IMockService _mockService;
        private readonly IMockScenarioService _mockScenarioService;
        public HomeController(IApplicationService applicationService, IMockService mockService, IMockScenarioService mockScenarioService)
        {
            _applicationService = applicationService;
            _mockService = mockService;
            _mockScenarioService = mockScenarioService;
        }


        public async Task<IActionResult> Index()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();

            var apps = await _applicationService.GetAllAsync();
            var mocks = await _mockService.GetAllAsync();
            var scenarios = await _mockScenarioService.GetAllAsync();

            dashboardViewModel.ApplicationViewModels = apps;
            dashboardViewModel.MockViewModels = mocks;
            dashboardViewModel.MockScenarioViewModels = scenarios;
            //TempData["success"] = "Welcome to Mock API Server!";
            //TempData["info"] = "Welcome to Mock API Server!";
            //TempData["warning"] = "Welcome to Mock API Server!";
            //TempData["failed"] = "Welcome to Mock API Server!";
            return View(dashboardViewModel);
        }
    }
}
