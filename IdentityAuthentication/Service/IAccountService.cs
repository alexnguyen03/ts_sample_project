using IdentityAuthentication.Model;
namespace IdentityAuthentication.Service
{
    public interface IAccountService
    {
        //public Task<IdentityResult> SignUpAsync(SignUpModel model);
        public Task<string> SignInAsync(SignInModel model);
        public Task<object> Register(SignUpModel model, string role);
    }
}
