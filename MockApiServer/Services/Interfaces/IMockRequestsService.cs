using Microsoft.AspNetCore.Mvc;
using MockApiServer.Data.Entities;

namespace MockApiServer.Services
{
    public interface IMockRequestsService
    {
        Task<(MockScenario? scenario, string? error)> GetScenarioAsync(string path, string method, string? scenarioKey);
        (ContentResult response, string rawJson) BuildResponse(MockScenario scenario, HttpResponse httpResponse);
    }
}
