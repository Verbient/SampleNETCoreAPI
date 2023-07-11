using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyApp.Common;
using MyApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApp.API
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly JWTConfig _jwt;
        public JwtMiddleware(RequestDelegate next, IOptions<JWTConfig> jwt, IHttpContextAccessor contextAccessor)
        {
            _next = next;
            this.contextAccessor = contextAccessor;
            _jwt = jwt.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            //context.Request.Headers.TryGetValue("Authorization", out StringValues authString);

            if (token != null)
                attachAccountToContext(context, token);

            await _next(context);
        }

        private void attachAccountToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwt.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero,

                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                JWTAuth appUser = new()
                {
                    Id = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value),
                    Email = jwtToken.Claims.First(x => x.Type == "Email").Value,
                    FirstName = jwtToken.Claims.First(x => x.Type == "FirstName").Value,
                    RoleId = Convert.ToInt16(jwtToken.Claims.First(x => x.Type == "RoleId").Value),
                    CountryCode = jwtToken.Claims.First(x => x.Type == "CountryCode").Value,
                    Phone = jwtToken.Claims.First(x => x.Type == "Phone").Value,
                    RoleName = (Enums.UserRoles)Enum.Parse(typeof(Enums.UserRoles), jwtToken.Claims.First(x => x.Type == "Role").Value),
                };

                // TODO User.Identity.NAme returns null
                List<Claim> claims = new () {
                    new Claim(type: "Id", value: jwtToken.Claims.First(x => x.Type == "Id").Value),
                    new Claim(type: "Email", value:jwtToken.Claims.First(x => x.Type == "Email").Value),
                    new Claim(type: "RoleId", value: jwtToken.Claims.First(x => x.Type == "RoleId").Value),
                    new Claim(type: "Phone", value: jwtToken.Claims.First(x => x.Type == "Phone").Value),
                    new Claim(type: "CountryCode", value: jwtToken.Claims.First(x => x.Type == "CountryCode").Value),
                    new Claim(ClaimTypes.Role, value: jwtToken.Claims.First(x => x.Type == "Role").Value)
                    };
                ClaimsIdentity claimIdentity = new (claims, CookieAuthenticationDefaults.AuthenticationScheme);
                //ClaimsPrincipal principal = new ClaimsPrincipal(claimIdentity);
                contextAccessor.HttpContext!.User = new ClaimsPrincipal(claimIdentity); // ? TODO What is the use of this


                // attach account to context on successful jwt validation
                context.Items["Account"] = appUser;  // await _userRepository.GetUser(accountId);
                //context.Items["User"] = appUser;

            }
            catch
            {
                // do nothing if jwt validation fails
                // account is not attached to context so request won't have access to secure routes
            }
        }
    }
}
