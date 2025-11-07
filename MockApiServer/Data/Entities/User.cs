using System;
using System.Collections.Generic;

namespace MockApiServer.Data.Entities;

public partial class User
{
    public long Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Name { get; set; }

    public string? MobileNo { get; set; }

    public DateTime? LastLoginTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsActive { get; set; }

    public string? Remarks { get; set; }
}
