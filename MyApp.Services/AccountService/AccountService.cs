
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyApp.Common;
using MyApp.DAL;
using MyApp.DTO;
using MyApp.Models;
using MyApp.Util;
using static MyApp.Common.Enums;

namespace MyApp.Services
{
    public class AccountService : IAccountService
    {

        private readonly IUserRepository userRepository;
        private readonly IAdhocRepository adhocRepository;

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger logger;
        private readonly JWTConfig jwtConfig;
        private readonly JWTAuth account;

        const string USERSQL = "SELECT TOP 5 A.Id, A.RoleId, A.FullName, A.Email,CONCAT('(',A.CountryCode,') ' ,A.Phone) as Phone FROM AppUser A JOIN UserRole R ON A.RoleId = R.Id";
        public AccountService(IUserRepository repository, IAdhocRepository adhocRepository,
             IOptions<JWTConfig> jwt, IHttpContextAccessor httpContextAccessor, ILogger<AccountService> logger)
        {
            jwtConfig = jwt.Value;
            userRepository = repository;
            this.adhocRepository = adhocRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            account = (JWTAuth)httpContextAccessor.HttpContext.Items["Account"];
        }

        public AppUserModel Register(AppUserRequestDTO dto)
        {
            bool validPassword = ValidationUtil.ValidatePassword(dto.Password, out string errorMessage);
            if (!validPassword)
            {
                throw new CustomException(errorMessage);
            }
            string sql = $"SELECT * FROM AppUser WHERE Email like '{dto.Email}' ";
            var data = adhocRepository.QuerySingleOrDefault<AppUserModel>(sql);
            if (data != null)
            {
                //throw new Exception ("Email is duplicate");
                throw new CustomException("Email already exists");
            }
            AppUserModel userEntity = new ();
            ObjectMapper.MapSourceObjectToTarget(dto, userEntity);
            userEntity.Password = HashUtil.HashPassword(dto.Password);
            userEntity.RoleId = (int)UserRoles.Customer;
            var user = userRepository.Create(userEntity);

            return user;
        }

        public AuthenticateDTO AuthenticateWithEmail(AuthenticateWithEmailDTO model)
        {
            string sql = $"SELECT * FROM AppUser WHERE Email like '{model.Email}' ";
            AppUserModel user = adhocRepository.QuerySingleOrDefault<AppUserModel>(sql);

            if (user == null)
            {
                throw new CustomException("Email does not exist");
            }

            if (!HashUtil.MatchSaltedHash(model.Password, user.Password!))
                throw new CustomException("Incorrect password");

            return GenerateAuthenticationAndToken(user);
        }

        public IEnumerable<AppUserModel> SearchUsersByEmailContaining(string Email)
        {
            string sql = USERSQL + $" WHERE Email like '%{Email}%' ORDER BY A.CreateDateTimeUtc ASC";
            return adhocRepository.Query<AppUserModel>(sql);
        }

        public IEnumerable<AppUserModel> SearchUserByNameContaining(string NameContains)
        {
            string sql = USERSQL + $" WHERE FullName like '%{NameContains}%' ORDER BY A.CreateDateTimeUtc ASC";
            return adhocRepository.Query<AppUserModel>(sql);
        }

        private AuthenticateDTO GenerateAuthenticationAndToken(AppUserModel user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtConfig.Secret);

            List<Claim> claims = new () {
                new Claim(type: "Id", value: user.Id.ToString()),
                new Claim(type: "Email", value: user.Email),
                new Claim(type: "FirstName", value: user.FullName.Split(' ')[0]),
                new Claim(type: "RoleId", value: user.RoleId.ToString()!),
                new Claim(type: "CountryCode", value: user.CountryCode.ToString()),
                new Claim(type: "Phone", value: user.Phone.ToString()),
                new Claim(type: "Role", value: ((UserRoles)user.RoleId!).ToString())
            };

            ClaimsIdentity claimIdentity = new (claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimIdentity,
                Expires = DateTime.UtcNow.AddDays(15), // 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var getDTO = new AuthenticateDTO();
            getDTO.JwtToken = tokenHandler.WriteToken(token);
            getDTO.AppUserId = user.Id;
            getDTO.FullName = user.FullName;
            getDTO.RoleName = ((UserRoles)user.RoleId).ToString();
            getDTO.Email = user.Email;
            getDTO.Phone = user.Phone;
            getDTO.CountryCode = user.CountryCode;
            return getDTO;
        }

        public bool CheckUserExistByEmail(string email)
        {
            bool result = userRepository.CheckUserExistsByEmail(email);
            return result;
        }

        public bool CheckUserExistByPhone(int CountryCode, string phone)
        {
            bool result = userRepository.CheckUserExistsByPhone(CountryCode, phone);
            return result;
        }

        public void ChangePassword(PasswordChangeDTO dto)
        {
            try
            {
                var user = AuthenticateWithEmail(new AuthenticateWithEmailDTO { Email = account.Email, Password = dto.OldPassword });
                if (user == null)
                {
                    throw new CustomException("Incorrect email or password");
                }

                bool validPassword = ValidationUtil.ValidatePassword(dto.Password, out string errorMessage);
                if (!validPassword)
                {
                    throw new CustomException(errorMessage);
                }

                bool result = userRepository.UpdatePassword(account.Email, HashUtil.HashPassword(dto.Password));
                if (result == true)
                {
                    string name = string.IsNullOrEmpty(user.FullName) ? account.Email : user.FullName;
                    logger.LogInformation($"Password changed: {account.Email}");
                }
                else
                {
                    throw new CustomException("Password could not be updated due to server error");
                }
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message);
            }
        }
    }
}
