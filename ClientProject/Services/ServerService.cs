namespace ClientProject.Services
{
    public class ServerService
    {
        private readonly HttpClient httpClient;
        public ServerService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public string GetBaseUrl()
        {
            return httpClient.BaseAddress.ToString();
        }
    }
}
