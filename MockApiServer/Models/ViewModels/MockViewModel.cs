using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockApiServer.Models.ViewModels
{
    public class MockViewModel
    {
        public int Id { get; set; }
        public int? ApplicationId { get; set; } = 0;
        public string? ApplicationName { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Path { get; set; } = string.Empty;
        public string? Method { get; set; } = "GET";
        public bool? IsActive { get; set; }
        public List<SelectListItem>? ApplicationList { get; set; }
    }
}
