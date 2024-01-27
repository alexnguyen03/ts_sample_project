using Microsoft.AspNetCore.Components;
using ServerProject.Models;
namespace ClientProject.Services
{
    public class FontendService : IFontendService
    {
        private readonly HttpClient _httpClient = null;
        public FontendService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }
        public async Task<List<Customer>> GetAllCustomer()
        {
            var data = await _httpClient.GetJsonAsync<List<Customer>>("api/Customer/getCustomer");
            Console.WriteLine(data.ToArray().Length);
            return data;
        }
    }
}
