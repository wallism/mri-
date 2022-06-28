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
        const string StateStore = "statestore";

        public HealthService(DaprClient dapr)
        {
            _dapr = dapr;
        }

        public async Task<int> IncrementCount()
        {
            // save to store then send to bus
            try
            {
                var current = await GetCount();
                current++;
                await _dapr.SaveStateAsync(StateStore, "counter", current);
                var message = $"{current} -> {GetType().Name}.IncrementCount";
                Console.WriteLine(message);
                // send to pub sub

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
