namespace MockApiServer.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<ApplicationViewModel>? ApplicationViewModels { get; set; }
        public List<MockViewModel>? MockViewModels { get; set; }
        public List<MockScenarioViewModel>? MockScenarioViewModels { get; set; }
    }
}
