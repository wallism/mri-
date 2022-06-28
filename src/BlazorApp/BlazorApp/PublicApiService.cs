namespace BlazorApp
{
    public interface IPublicApiService
    {
        Task SendCountMessage(string text);
    }

    public class PublicApiService : IPublicApiService
    {
        private readonly HttpClient _client;

        public PublicApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task SendCountMessage(string text)
        {
            var response = await _client.PostAsJsonAsync("health/count", new Message{Text = text});
            Console.WriteLine(response.StatusCode);
        }
    }

    public class Message
    {
        public string Text { get; set; }
    }
}
