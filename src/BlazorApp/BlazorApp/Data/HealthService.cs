using Dapr.Client;
using Serilog;

namespace BlazorApp.Data
{
    public interface ISampleService
    {
        Task<int> IncrementCount();
        Task<int> GetCount();
        Task ResetCount();
        Task<string> GetSecret();
    }

    public class SampleService : ISampleService
    {
        private readonly DaprClient _dapr;
        private readonly IPublicApiService _publicApiService;

        const string StateStore = "statestore";

        public SampleService(DaprClient dapr, IPublicApiService publicApiService)
        {
            _dapr = dapr;
            _publicApiService = publicApiService;
        }

        public async Task<int> IncrementCount()
        {
            try
            {
                var current = await GetCount();
                current++;
                // save to store then send to publicAPI
                await _dapr.SaveStateAsync(StateStore, "counter", current);
                var message = $"{current} -> Blazor";
                Log.Warning(message);

                // call public API
                await _publicApiService.SendCountMessage(message);

                return current;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "increment error");
                return -1;
            }

        }

        public async Task<int> GetCount()
        {
            return await _dapr.GetStateAsync<int>(StateStore, "counter");
        }

        public async Task ResetCount()
        {
            await _dapr.SaveStateAsync(StateStore, "counter", 0);
        }

        public async Task<string> GetSecret()
        {
            var secretStoreName = Environment.GetEnvironmentVariable("SECRET_STORE_NAME") ?? "SecretStore";
            const string secretName = "connectionStrings:sql";
            var secret = await _dapr.GetSecretAsync(secretStoreName, secretName);
            return secret[secretName];
        }
    }
}
