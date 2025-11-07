using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockApiServer.Models.ViewModels
{
    public class MockScenarioViewModel
    {
        public int Id { get; set; }
        public int? MockId { get; set; } = 0;
        public string? MockName { get; set; } = string.Empty;
        public string? ScenarioKey { get; set; } = string.Empty;
        public int? StatusCode { get; set; }
        public string? ResponseJson { get; set; } = "{}";
        public string? HeadersJson { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<SelectListItem>? MockList { get; set; }
    }
}
