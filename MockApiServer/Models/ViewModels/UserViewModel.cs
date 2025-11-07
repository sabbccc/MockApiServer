namespace MockApiServer.Models.ViewModels
{
    public class UserViewModel
    {
        public long Id { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Name { get; set; }

        public string? MobileNo { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        public bool? IsActive { get; set; }

        public string? Remarks { get; set; }
    }
}
