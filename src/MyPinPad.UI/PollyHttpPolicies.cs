using Polly;
using Polly.Extensions.Http;

namespace MyPinPad.UI
{
    public static class PollyHttpPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            // Retries on HTTP 5xx, 408 and network failures
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), 
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
        }
    }
}
