using System.Net.Http.Headers;

namespace Orchestrator.Rest.Http
{
    public class PropagationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PropagationDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader))
                {
                    request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
                }

                var correlationId = httpContext.Request.Headers["X-Correlation-Id"].FirstOrDefault();
                var operationId = httpContext.Request.Headers["X-Operation-Id"].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(correlationId))
                    request.Headers.TryAddWithoutValidation("X-Correlation-Id", correlationId);
                if (!string.IsNullOrWhiteSpace(operationId))
                    request.Headers.TryAddWithoutValidation("X-Operation-Id", operationId);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
