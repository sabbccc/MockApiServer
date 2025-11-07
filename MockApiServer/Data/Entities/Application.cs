using System;
using System.Collections.Generic;

namespace MockApiServer.Data.Entities;

public partial class Application
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Mock> Mocks { get; set; } = new List<Mock>();
}
