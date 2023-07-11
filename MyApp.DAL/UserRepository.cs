using Dapper;
using Microsoft.AspNetCore.Http;
using MyApp.DAL;
using MyApp.DTO;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyApp.DAL
{
    public class UserRepository: GenericRepository<AppUserModel>, IUserRepository
    {
        //private IDbConnection _dbConnection;
        public UserRepository(IDbConnection dbConnection,IHttpContextAccessor httpContextAccessor):base(dbConnection,httpContextAccessor)
        {
            _dbConnection = dbConnection;
        }
        public AppUserModel GetUserByEmail(string email)
        {
            string sql = @"SELECT * FROM AppUser WHERE Email =@Email ";
            var result = _dbConnection.QueryFirstOrDefault<AppUserModel>(sql, new
            {
                Email = email
            });
            return result;
        }

       
        public bool CheckUserExistsByEmail(string Email)
        {
            string sql = @"SELECT COUNT(*) FROM AppUser WHERE Email =@Email ";
            var result = _dbConnection.QuerySingle<int>(sql, new
            {
                Email 
            });
            return result==1;
        }

        public bool CheckUserExistsByPhone(int CountryCode, string Phone)
        {
            string sql = @"SELECT COUNT(*) FROM AppUser WHERE CountryCode =@CountryCode AND  Phone =@Phone ";
            var result = _dbConnection.QuerySingle<int>(sql, new
            {
                CountryCode,
                Phone
            });
            return result == 1;
        }
        public AppUserModel GetUserByMobile(int CountryCode, string Phone)
        {
            string sql = @"SELECT * FROM AppUser WHERE CountryCode =@CountryCode AND Phone =@Phone";
            var result = _dbConnection.QueryFirstOrDefault<AppUserModel>(sql, new
            {
                CountryCode,
                Phone
            });
            return result;
        }
      

        public async Task<AppUserModel> GetUser(int id)
        {
            string sql = "SELECT * FROM AppUser WHERE AppUserId = @AppUserId";
            var result = await _dbConnection.QueryFirstOrDefaultAsync<AppUserModel>(sql, new { AppUserId = id });
            return result;
        }

        //public string VerifyEmail(string Guid) // Returns email of the activated
        //{
        //    DateTime? verifiedDate = _dbConnection.QuerySingleOrDefault<DateTime?>($"SELECT EmailVerifiedDateUTC FROM AppUser WHERE VerificationGuid='{Guid}'");
        //    if (verifiedDate != null)
        //    {
        //        return $"Email already verified on {(verifiedDate??DateTime.Now).ToString("dd MMM yyyy ")} at {(verifiedDate ?? DateTime.Now).ToString("HH:mm")} hrs UTC";
        //    }

        //    string sql = "UPDATE appuser SET Enabled = 1, EmailVerifiedDateUTC=GetUtcDate() WHERE VerificationGuid= @VerificationGuid;";
        //    var result =  _dbConnection.Execute(sql, new
        //    {
        //        VerificationGuid = Guid
        //    });
        //    if (result==1)
        //    {
        //        //var email = _dbConnection.QuerySingle<string>($"SELECT Email From AppUser WHERE VerificationGuid='{Guid}'");
        //        return $"Your email has been successfully verified";
        //    }
        //    throw new CustomException("Incorrect verification credentials");
        //}

        public bool UpdatePassword(string email, string PasswordHash)
        {
            string sql = "UPDATE appuser SET Password = @PasswordHash WHERE Email= @Email;";
            var result = _dbConnection.Execute(sql, new
            {
                PasswordHash,
                Email = email
            }); 
            if (result == 1)
            {
                return true;
            }
            return false;
        }

        public AppUserProfileResponseDTO UpdateProfile(AppUserModel model)
        {
            string sql = $"UPDATE AppUser SET FullName='{model.FullName}', " +
                $"Phone = '{model.Phone}' OUTPUT inserted.Email, inserted.FullName, inserted.Company, inserted.Phone WHERE Email= '{model.Email}';";
            var result = _dbConnection.QuerySingleOrDefault<AppUserProfileResponseDTO>(sql);
            
            return result;
        }
    }
}
