using Microsoft.AspNetCore.Mvc;
using MockApiServer.Data.Entities;
using MockApiServer.Repositories;
using System.Text.Json;

namespace MockApiServer.Services
{
    public class MockRequestsService : IMockRequestsService
    {
        private readonly IMockRepository _repository;

        public MockRequestsService(IMockRepository repository)
        {
            _repository = repository;
        }

        public async Task<(MockScenario? scenario, string? error)> GetScenarioAsync(string path, string method, string? scenarioKey)
        {
            if (string.IsNullOrWhiteSpace(scenarioKey))
                return (null, "Missing X-Mock-Scenario header");

            var mock = await _repository.GetMockWithScenariosAsync(path, method);
            if (mock == null)
                return (null, "Mock not found");

            var scenario = mock.MockScenarios.FirstOrDefault(s => s.ScenarioKey == scenarioKey && s.IsActive == true);
            if (scenario == null)
                return (null, "Scenario not found");

            return (scenario, null);
        }

        public (ContentResult response, string rawJson) BuildResponse(MockScenario scenario, HttpResponse httpResponse)
        {
            // Raw JSON (exactly as stored in DB, for logging)
            var rawJson = scenario.ResponseJson;

            // Pretty JSON (for client response)
            string prettyJson;
            try
            {
                using var doc = JsonDocument.Parse(rawJson);
                prettyJson = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                prettyJson = rawJson; // fallback if invalid JSON
            }

            // Optional headers
            if (!string.IsNullOrEmpty(scenario.HeadersJson))
            {
                try
                {
                    var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(scenario.HeadersJson);
                    if (headers != null)
                    {
                        foreach (var kv in headers)
                            httpResponse.Headers[kv.Key] = kv.Value;
                    }
                }
                catch
                {
                    // skip malformed headers
                }
            }

            var response = new ContentResult
            {
                Content = prettyJson,
                ContentType = "application/json",
                StatusCode = scenario.StatusCode
            };

            return (response, rawJson);
        }
    }
}
