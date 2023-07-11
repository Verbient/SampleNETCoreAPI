using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyApp.Common;
using MyApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Tests
{
    public static class Settings
    {
        public static ObjectResult GetResultFromIActionResult(object o)
        {
            return (ObjectResult)o;
        }
        public static IDbConnection Get_IDbConnection_Object()
        {
            // To read from User-Secrets, uncomment the following code:
            //var config = new ConfigurationBuilder().AddUserSecrets("d6df2d78-3c9c-48c0-951b-517bbb129caa").Build();


            var config = new ConfigurationBuilder()
            .AddJsonFile(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "MyApp.API/bin/Debug/net6.0/appsettings.json")), optional: false, reloadOnChange: true).Build();
            
            string DatabasePath =  Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..","..", "MyApp.API","Database", "MyAppDatabase.mdf"));
            var connectionString = config.GetConnectionString("DefaultConnection")!.Replace("{PathProvidedIn_Program.cs}", DatabasePath);

            return new SqlConnection(connectionString);
        }

        #region Required by Service-Tests for access to IHttpContextAccessor
        private static IHttpContextAccessor GetMockHttpContextAccessor(Enums.UserRoles Role)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            int id = 5; // Customer Id in database
            switch (Role)
            {
                case Enums.UserRoles.SuperAdmin:
                    id = 1;
                    break;
                case Enums.UserRoles.Admin:
                    id = 3;
                    break;
            }

            JWTAuth appUser = new ()
            {
                Id = id,
                Email = "admin1@mail.com",
                RoleName = Role
            };
            HttpContext context = new DefaultHttpContext();
            context.Items["Account"] = appUser;

            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);


            return mockHttpContextAccessor.Object;
        }

        public static IHttpContextAccessor GetMockAccessor_SuperAdmin()
        {
            return GetMockHttpContextAccessor(Enums.UserRoles.SuperAdmin);
        }

        public static IHttpContextAccessor GetMockAccessor_Admin()
        {
            return GetMockHttpContextAccessor(Enums.UserRoles.Admin);
        }

        public static IHttpContextAccessor GetMockAccessor_Customer()
        {
            return GetMockHttpContextAccessor(Enums.UserRoles.Customer);
        }

        #endregion

        #region Required by Controller-Tests to authorize Controller
        private static dynamic AttachControllerContext(dynamic controller, Enums.UserRoles Role)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "Mock Admin Name"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim("id", "1"),
                new Claim("email", "admin1@mail.com"),
            }, "mock"));

            JWTAuth appUser = new ()
            {
                Id = 1,
                Email = "admin1@mail.com",
                RoleName = Role
            };
            HttpContext context = new DefaultHttpContext();

            context.Items["Account"] = appUser;
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = context
            };
            return controller;
        }

        public static dynamic AttachControllerContext_Admin(dynamic controller)
        {
            return AttachControllerContext(controller, Enums.UserRoles.SuperAdmin);
        }

        public static dynamic AttachControllerContext_User(dynamic controller)
        {
            return AttachControllerContext(controller, Enums.UserRoles.SuperAdmin);
        }
        #endregion

    }
}
