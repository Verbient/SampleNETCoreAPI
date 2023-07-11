using Dapper;
using MyApp.DTO;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Xatocode.DTO;
//using Xatocode.Models;
//using static Xatocode.Models.DataTables;

namespace MyApp.DAL
{
    public interface IUserRepository:IGenericRepository<AppUserModel>
    {
        bool CheckUserExistsByEmail(string Email);
        bool CheckUserExistsByPhone(int CountryCode, string Phone);
        AppUserModel GetUserByEmail(string email);
        AppUserModel GetUserByMobile(int CountryCode, string phone);
       

        //string VerifyEmail(string Guid);

        //bool UpdateVerificationGuid(string Email,string guid);
        bool UpdatePassword(string email, string PasswordHash);

        AppUserProfileResponseDTO UpdateProfile(AppUserModel model);

    }
}
