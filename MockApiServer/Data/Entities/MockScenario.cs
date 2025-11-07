using System;
using System.Collections.Generic;

namespace MockApiServer.Data.Entities;

public partial class MockScenario
{
    public int Id { get; set; }

    public int MockId { get; set; }

    public string ScenarioKey { get; set; } = null!;

    public int StatusCode { get; set; }

    public string ResponseJson { get; set; } = null!;

    public string? HeadersJson { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsActive { get; set; }

    public virtual Mock Mock { get; set; } = null!;
}
