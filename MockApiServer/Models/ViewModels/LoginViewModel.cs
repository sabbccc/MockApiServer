using System.ComponentModel.DataAnnotations;

namespace MockApiServer.Models.ViewModels
{
    public class LoginViewModel
    {
        public string? Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;

        // Audit Trail Properties
        public DateTime LastLoginTime { get; set; } = DateTime.Now;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? DeviceInfo { get; set; }
        public string? Location { get; set; }
        public bool? IsSuccessful { get; set; } = true;

        // Optional Tracking Info
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string? Remarks { get; set; }
    }
}
