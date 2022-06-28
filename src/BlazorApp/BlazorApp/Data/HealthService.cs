using System.Reflection;
using Dapr.Client;

namespace BlazorApp.Data
{
    public interface IHealthService
    {
        Task<int> IncrementCount();
        Task<int> GetCount();
        Task ResetCount();
    }

    public class HealthService : IHealthService
    {
        private readonly DaprClient _dapr;
        private readonly IPublicApiService _publicApiService;

        const string StateStore = "statestore";

        public HealthService(DaprClient dapr, IPublicApiService publicApiService)
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
                Console.WriteLine(message);

                // call public API
                await _publicApiService.SendCountMessage(message);

                return current;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
    }
}
