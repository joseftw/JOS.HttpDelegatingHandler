using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace JOS.HttpDelegatingHandler.Infra.Http
{
    public class DefaultHttpDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger<DefaultHttpDelegatingHandler> _logger;

        public DefaultHttpDelegatingHandler(ILogger<DefaultHttpDelegatingHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sending {RequestMethod} request towards {Request}", request.Method, request?.RequestUri?.ToString());
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage response = null;
            try
            {
                response = await base.SendAsync(request, cancellationToken);
                stopwatch.Stop();
                _logger.LogInformation("Response took {ElapsedMilliseconds}ms {StatusCode}", response.StatusCode, stopwatch.ElapsedMilliseconds, response.StatusCode);
                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (Exception exception) when (exception is TaskCanceledException || (exception is TimeoutException))
            {
                stopwatch.Stop();
                _logger.LogError(exception,
                    "Timeout during {RequestMethod} to {RequestUri} after {ElapsedMilliseconds}ms {StatusCode}",
                    request.Method,
                    request.RequestUri?.ToString(),
                    stopwatch.ElapsedMilliseconds,
                    response?.StatusCode);
                throw;
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                _logger.LogError(exception,
                    "Exception during {RequestMethod} to {RequestUri} after {ElapsedMilliseconds}ms {StatusCode}",
                    request.Method,
                    request.RequestUri?.ToString(),
                    stopwatch.ElapsedMilliseconds,
                    response?.StatusCode);
                throw;
            }
        }
    }
}
