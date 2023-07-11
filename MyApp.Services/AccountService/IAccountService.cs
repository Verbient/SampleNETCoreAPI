using MyApp.DTO;
using MyApp.Models;

namespace MyApp.Services
{
    public interface IAccountService
    {
        AuthenticateDTO AuthenticateWithEmail(AuthenticateWithEmailDTO model);
        void ChangePassword(PasswordChangeDTO dto);
        bool CheckUserExistByEmail(string email);
        bool CheckUserExistByPhone(int CountryCode, string phone);
        AppUserModel Register(AppUserRequestDTO dto);
        IEnumerable<AppUserModel> SearchUserByNameContaining(string NameContains);
        IEnumerable<AppUserModel> SearchUsersByEmailContaining(string Email);
    }
}