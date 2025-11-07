using System;
using System.Collections.Generic;

namespace MockApiServer.Data.Entities;

public partial class Mock
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public string? Name { get; set; }

    public string Path { get; set; } = null!;

    public string Method { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual ICollection<MockScenario> MockScenarios { get; set; } = new List<MockScenario>();
}
