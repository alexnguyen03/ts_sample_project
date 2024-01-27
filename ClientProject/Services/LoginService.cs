namespace ClientProject.Services
{
    public class LoginService
    {
        private readonly HttpClient httpClient;
        public LoginService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public string GetBaseUrl()
        {
            return httpClient.BaseAddress.ToString();
        }
    }
}
