using System.Threading.Tasks;
using UI.Models;

namespace UI.Contracts
{
    public interface IAuthenticationRepository
    {
        public Task<bool> Register(UserRegistrationModel user);

        public Task<bool> LogIn(UserLoginModel user);

        public Task LogOut();
    }
}