using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace MockApiServer.Services
{
    public sealed class UrlBrowsingHostedService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<UrlBrowsingHostedService> _logger;
        private readonly IServer _server;

        public UrlBrowsingHostedService(
            IConfiguration configuration,
            IHostApplicationLifetime applicationLifetime,
            ILogger<UrlBrowsingHostedService> logger,
            IServer server)
        {
            _configuration = configuration;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _server = server;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_configuration.GetValue<bool>("ApplicationConfiguration:EnableUrlBrowsing"))
            {
                return Task.CompletedTask;
            }

            _applicationLifetime.ApplicationStarted.Register(OpenBrowser);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private void OpenBrowser()
        {
            var browserUrl = _configuration["ApplicationConfiguration:appUrl"];

            if (string.IsNullOrWhiteSpace(browserUrl))
            {
                browserUrl = _server.Features.Get<IServerAddressesFeature>()?.Addresses.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(browserUrl))
            {
                _logger.LogWarning("URL browsing is enabled, but no application URL is available.");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = browserUrl,
                    UseShellExecute = true
                });

                _logger.LogInformation("Opened browser at {BrowserUrl}", browserUrl);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to open browser at {BrowserUrl}", browserUrl);
            }
        }
    }
}
