using ServerProject.Models;

namespace ClientProject.Services
{
    public interface IFontendService
    {
        Task<List<Customer>> GetAllCustomer();

    }
}
